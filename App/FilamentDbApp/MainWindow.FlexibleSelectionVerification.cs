using FilamentDbApp.Models;
using System.Collections;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;

namespace FilamentDbApp;

public partial class MainWindow
{
    // Synthetic controls only: never constructs MainWindow or opens a database.
    // Exercises real parent-selection helpers and WPF event ordering. The live
    // edit/validation and persistence paths remain separate acceptance checks.
    private static bool VerifyFlexibleSelectionIsolation()
    {
        var passed = false;
        var thread = new Thread(() =>
        {
            try { passed = VerifyFlexibleSelectionIsolationOnSta(); }
            catch (Exception) { passed = false; }
        }) { IsBackground = true, Name = "Flexible selection verification" };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return thread.Join(TimeSpan.FromSeconds(10)) && passed;
    }

    private static bool VerifyFlexibleSelectionIsolationOnSta()
    {
        var sessions = new[]
        {
            new FlexibleTestSessionRecord { FlexibleTestSessionId = "SESSION-A", SessionLabel = "Test Session 1" },
            new FlexibleTestSessionRecord { FlexibleTestSessionId = "SESSION-B", SessionLabel = "Test Session 2" }
        };
        var specimens = sessions.Select((session, index) => new FlexibleTestSpecimenRecord
        {
            FlexibleTestSessionId = session.FlexibleTestSessionId,
            SpecimenId = "SPECIMEN-" + index,
            SpecimenLabel = "Shore Specimen 1", IntendedTest = "Shore"
        }).ToArray();
        var readings = specimens.Select((specimen, index) => new ShoreHardnessReadingRecord
        {
            SpecimenId = specimen.SpecimenId, ShoreReadingId = "READING-" + index,
            HardnessValue = index == 0 ? "41" : "81", ShoreScale = "A",
            ReadingTimeSeconds = "10", SpecimenThicknessMm = "8"
        }).ToArray();
        var originalValues = readings.Select(row => (row.ShoreReadingId, row.SpecimenId, row.HardnessValue)).ToArray();
        static DataGrid Grid(string property)
        {
            var grid = new DataGrid
            {
                AutoGenerateColumns = false, CanUserAddRows = false,
                SelectionMode = DataGridSelectionMode.Single,
                SelectionUnit = DataGridSelectionUnit.CellOrRowHeader
            };
            grid.Columns.Add(new DataGridTextColumn { Binding = new Binding(property) });
            return grid;
        }
        var sessionGrid = Grid(nameof(FlexibleTestSessionRecord.SessionLabel));
        var specimenGrid = Grid(nameof(FlexibleTestSpecimenRecord.SpecimenLabel));
        var readingGrid = Grid(nameof(ShoreHardnessReadingRecord.HardnessValue));
        sessionGrid.ItemsSource = sessions;
        FlexibleTestSessionRecord? selectedSession = null;
        FlexibleTestSpecimenRecord? selectedSpecimen = null;
        var binding = false;
        var transitions = 0;

        void Apply(FlexibleTestSessionRecord session)
        {
            if (binding || ReferenceEquals(selectedSession, session)) return;
            if (++transitions > 30) throw new InvalidOperationException("Selection reentered without a guard.");
            binding = true;
            try
            {
                selectedSession = session;
                var visible = specimens.Where(row => BelongsToFlexibleSession(row, session)).ToList();
                selectedSpecimen = visible.Single();
                specimenGrid.ItemsSource = visible;
                SetFlexibleParentSelection(sessionGrid, session);
                SetFlexibleParentSelection(specimenGrid, selectedSpecimen);
                readingGrid.ItemsSource = readings.Where(row => row.SpecimenId == selectedSpecimen.SpecimenId).ToList();
            }
            finally { binding = false; }
        }
        sessionGrid.SelectionChanged += (_, args) =>
        {
            if (!binding && ResolveFlexibleParentSelection<FlexibleTestSessionRecord>(sessionGrid, args) is { } session)
                Apply(session);
        };
        sessionGrid.CurrentCellChanged += (_, _) =>
        {
            if (!binding && sessionGrid.CurrentCell.Item is FlexibleTestSessionRecord session) Apply(session);
        };
        bool Aligned(int index) =>
            ReferenceEquals(selectedSession, sessions[index]) &&
            ReferenceEquals(sessionGrid.SelectedItem, sessions[index]) &&
            ReferenceEquals(sessionGrid.CurrentCell.Item, sessions[index]) &&
            ReferenceEquals(selectedSpecimen, specimens[index]) &&
            ReferenceEquals(specimenGrid.CurrentCell.Item, specimens[index]) &&
            BelongsToFlexibleSession(specimens[index], selectedSession) &&
            !BelongsToFlexibleSession(specimens[1 - index], selectedSession) &&
            readingGrid.Items.Cast<ShoreHardnessReadingRecord>().SequenceEqual([readings[index]]);

        Apply(sessions[0]);
        if (!Aligned(0)) return false;
        // New selected row must win while CurrentCell still identifies the old row.
        var added = new SelectionChangedEventArgs(DataGrid.SelectionChangedEvent,
            (IList)new[] { sessions[0] }, (IList)new[] { sessions[1] });
        if (!ReferenceEquals(ResolveFlexibleParentSelection<FlexibleTestSessionRecord>(sessionGrid, added), sessions[1]))
            return false;
        sessionGrid.SelectedItem = sessions[1];
        if (!Aligned(1)) return false;
        // Keyboard movement changes CurrentCell without a whole-row click.
        sessionGrid.CurrentCell = new DataGridCellInfo(sessions[0], sessionGrid.Columns[0]);
        if (!Aligned(0)) return false;
        for (var index = 0; index < 6; index++)
        {
            var target = (index + 1) % 2;
            sessionGrid.CurrentCell = new DataGridCellInfo(sessions[target], sessionGrid.Columns[0]);
            if (!Aligned(target)) return false;
        }
        // Null selection must clear the old current-cell identity as well.
        binding = true;
        SetFlexibleParentSelection(specimenGrid, null);
        if (specimenGrid.SelectedItem is not null || specimenGrid.CurrentCell.Item is FlexibleTestSpecimenRecord)
            return false;
        return readings.Select(row => (row.ShoreReadingId, row.SpecimenId, row.HardnessValue)).SequenceEqual(originalValues);
    }
}
