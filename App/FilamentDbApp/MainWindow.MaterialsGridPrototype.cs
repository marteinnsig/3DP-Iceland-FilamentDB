using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FilamentDbApp.Services;

namespace FilamentDbApp;

public partial class MainWindow
{
    private static List<MaterialsPrototypeColumn> BuildFastMeasurementIdentityColumns() =>
    [
        FastMeasurementColumn("Material ID", 90, "MaterialID", true),
        FastMeasurementColumn("Manufacturer", 130, "Manufacturer", true),
        FastMeasurementColumn("Product Line", 130, "ProductLine", true),
        FastMeasurementColumn("Marketing Name", 140, "MarketingName", true),
        FastMeasurementColumn("Base Material", 110, "BaseMaterial", true),
        FastMeasurementColumn("Category", 110, "MaterialCategory", true),
        FastMeasurementColumn("Variant / Finish", 120, "VariantFinish", true),
        FastMeasurementColumn("Reinforcement", 110, "Reinforcement", true),
        FastMeasurementColumn("Color", 90, "Color", true)
    ];

    private static MaterialsPrototypeColumn FastMeasurementColumn(
        string header,
        double width,
        string? propertyName,
        bool isReadOnly,
        FastGridCellKind cellKind = FastGridCellKind.Standard) =>
        new(
            header,
            width,
            propertyName,
            isReadOnly,
            MaterialsPrototypeEditorKind.Text,
            Array.Empty<string>(),
            cellKind);

    private static readonly bool FastMaterialsViewDefaultEnabled = true;
    private MaterialsRenderingPrototypeView? _embeddedMaterialsPrototypeView;
    private bool _fastMaterialsCloseGuardAttached;

    private void ToggleFastMaterialsView_Click(object sender, RoutedEventArgs e)
    {
        if (FastMaterialsViewMenuItem.IsChecked)
        {
            ActivateFastMaterialsView();
            return;
        }

        if (_embeddedMaterialsPrototypeView is not null &&
            !_embeddedMaterialsPrototypeView.ConfirmCanClose())
        {
            FastMaterialsViewMenuItem.IsChecked = true;
            return;
        }

        CloseEmbeddedFastMaterialsView();
    }

    private void ActivateDefaultFastMaterialsView()
    {
        if (!FastMaterialsViewDefaultEnabled) return;
        FastMaterialsViewMenuItem.IsChecked = true;
        ActivateFastMaterialsView();
    }

    private void ActivateFastMaterialsView()
    {
        if (_embeddedMaterialsPrototypeView is not null) return;
        _embeddedMaterialsPrototypeView = CreateMaterialsRenderingPrototypeView(directCanonicalEditing: true);
        FastMaterialsViewHost.Content = _embeddedMaterialsPrototypeView;
        FastMaterialsViewHost.Visibility = Visibility.Visible;
        NativeMaterialsGrid.Visibility = Visibility.Collapsed;
        if (!_fastMaterialsCloseGuardAttached)
        {
            Closing += MainWindow_FastMaterialsViewClosing;
            _fastMaterialsCloseGuardAttached = true;
        }
    }

    private void ResetFastMaterialsColumns_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = MessageBox.Show(
            this,
            "Reset Materials column widths and order to the application defaults?",
            "Reset Materials Columns",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);
        if (confirmation != MessageBoxResult.Yes) return;

        _workflowPreferencesService.ClearFastMaterialsGridLayout();
        _embeddedMaterialsPrototypeView?.ResetLayout(BuildFastMaterialsColumns());
        ShowTransientStatus("Materials columns reset to defaults.");
    }

    private void MainWindow_FastMaterialsViewClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_embeddedMaterialsPrototypeView is not null &&
            !_embeddedMaterialsPrototypeView.ConfirmCanClose())
        {
            e.Cancel = true;
        }
    }

    private void CloseEmbeddedFastMaterialsView()
    {
        FastMaterialsViewHost.Content = null;
        FastMaterialsViewHost.Visibility = Visibility.Collapsed;
        NativeMaterialsGrid.Visibility = Visibility.Visible;
        _embeddedMaterialsPrototypeView = null;
    }

    private void ShowMaterialsRenderingPrototype_Click(object sender, RoutedEventArgs e)
    {
        var view = CreateMaterialsRenderingPrototypeView(directCanonicalEditing: false);
        var window = new Window
        {
            Title = "Materials Rendering Prototype — snapshot-only editing",
            Width = 1500,
            Height = 850,
            MinWidth = 900,
            MinHeight = 500,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = this,
            Content = view
        };
        window.Closing += (_, args) =>
        {
            if (!view.ConfirmCanClose())
            {
                args.Cancel = true;
            }
        };
        window.Show();
    }

    private MaterialsRenderingPrototypeView CreateMaterialsRenderingPrototypeView(bool directCanonicalEditing)
    {
        var columns = BuildFastMaterialsColumns();
        columns = ApplyFastMaterialsLayout(columns, _workflowPreferencesService.GetFastMaterialsGridLayout());

        var rows = BuildMaterialsPrototypeRows(columns);

        return new MaterialsRenderingPrototypeView(
            columns,
            rows,
            ApplyMaterialsPrototypeChanges,
            layout => _workflowPreferencesService.SetFastMaterialsGridLayout(layout),
            BuildMaterialsPrototypeRows,
            SelectMaterialsPrototypeRow,
            directCanonicalEditing);
    }

    private static List<MaterialsPrototypeColumn> BuildFastMaterialsColumns() =>
        AssignStablePrototypeLayoutKeys(
        [
            FastMaterialsColumn("Material ID", 90, "MaterialID", true),
            FastMaterialsColumn("Public reports", 95, "PublishPublicReports", false, MaterialsPrototypeEditorKind.CheckBox),
            FastMaterialsColumn("Public test details", 115, "PublishPublicTestDetails", false, MaterialsPrototypeEditorKind.CheckBox),
            FastMaterialsColumn("Manufacturer", 140, "Manufacturer", false),
            FastMaterialsColumn("Product Line", 140, "ProductLine", false),
            FastMaterialsColumn("Marketing Name", 160, "MarketingName", false),
            FastMaterialsColumn("Base Material", 110, "BaseMaterial", false),
            FastMaterialsColumn("Category", 120, "MaterialCategory", true),
            FastMaterialsColumn("Variant / Finish", 130, "VariantFinish", false),
            FastMaterialsColumn("Reinforcement", 120, "Reinforcement", false),
            FastMaterialsColumn("Color", 100, "Color", false),
            FastMaterialsColumn("Manufacturer Website", 260, "ManufacturerWebsite", false),
            FastMaterialsColumn("YouTube Review URL", 260, "YouTubeReviewUrl", false),
            FastMaterialsColumn("Video", 80, "Video", true),
            FastMaterialsColumn("Tested Status", 120, "TestedStatus", true),
            FastMaterialsColumn("In Tensile", 90, "InTensile", true),
            FastMaterialsColumn("In Impact", 90, "InImpact", true),
            FastMaterialsColumn("In Stiffness", 95, "InStiffness", true),
            FastMaterialsColumn("Notes", 220, "Notes", false),
            FastMaterialsColumn("Spool Weight g / spool", 110, "SpoolWeightG", false),
            FastMaterialsColumn("Manufacturer SKU", 140, "ManufacturerSku", false),
            FastMaterialsColumn("Inventory ID", 120, "InventoryId", false),
            FastMaterialsColumn("Purchase ID", 120, "PurchaseId", false),
            FastMaterialsColumn("Purchased From", 150, "PurchasedFrom", false),
            FastMaterialsColumn("Supplier URL", 190, "SupplierUrl", false),
            FastMaterialsColumn("Purchase Date", 110, "PurchaseDate", false),
            FastMaterialsColumn("Order Number", 130, "OrderNumber", false),
            FastMaterialsColumn("Batch Number", 120, "BatchNumber", false),
            FastMaterialsColumn("Storage Location", 140, "StorageLocation", false),
            FastMaterialsColumn("Inventory Status", 120, "InventoryStatus", false, MaterialsPrototypeEditorKind.ComboBox,
                ["Unopened", "Opened", "Empty"]),
            FastMaterialsColumn("Inventory Qty", 90, "Quantity", true),
            FastMaterialsColumn("Remaining Weight g / spool", 130, "RemainingWeightG", false),
            FastMaterialsColumn("Purchase Price", 110, "PurchasePriceAmount", false),
            FastMaterialsColumn("Currency", 90, "PurchaseCurrency", false, MaterialsPrototypeEditorKind.ComboBox,
                ["ISK", "USD", "EUR", "GBP", "DKK", "SEK", "NOK"]),
            FastMaterialsColumn("Shipping", 100, "ShippingAmount", false),
            FastMaterialsColumn("VAT", 90, "VatAmount", false),
            FastMaterialsColumn("MSRP Amount", 105, "MsrpAmount", false),
            FastMaterialsColumn("MSRP Currency", 105, "MsrpCurrency", false, MaterialsPrototypeEditorKind.ComboBox,
                ["USD", "ISK", "EUR", "GBP"]),
            FastMaterialsColumn("MSRP USD", 95, "MsrpUsd", true),
            FastMaterialsColumn("Landed Cost", 105, "LandedCostAmount", false),
            FastMaterialsColumn("Landed Currency", 115, "LandedCostCurrency", false, MaterialsPrototypeEditorKind.ComboBox,
                ["USD", "ISK", "EUR", "GBP"]),
            FastMaterialsColumn("Landed USD", 95, "LandedCostUsd", true),
            FastMaterialsColumn("MSRP USD/kg", 105, "MsrpUsdPerKg", true),
            FastMaterialsColumn("Landed USD/kg", 115, "LandedCostUsdPerKg", true),
            FastMaterialsColumn("Price Checked", 110, "PriceCheckedDate", false),
            FastMaterialsColumn("Thumbnail Filename", 170, "ThumbnailFilename", false),
            FastMaterialsColumn("Sort Order", 90, "SortOrder", true),
            FastMaterialsColumn("Source Priority", 130, "SourcePriority", true),
            FastMaterialsColumn("Archived / exclude from website export", 190, "IsArchived", false,
                MaterialsPrototypeEditorKind.CheckBox),
            FastMaterialsColumn("Website Display Name", 240, "WebsiteDisplayName", true),
            FastMaterialsColumn("Material Key", 260, "MaterialKey", true),
            FastMaterialsColumn("Validation", 180, "ValidationSummary", true)
        ]);

    private static MaterialsPrototypeColumn FastMaterialsColumn(
        string header,
        double width,
        string propertyName,
        bool isReadOnly,
        MaterialsPrototypeEditorKind editorKind = MaterialsPrototypeEditorKind.Text,
        IReadOnlyList<string>? choices = null) =>
        new(
            header,
            width,
            propertyName,
            isReadOnly,
            editorKind,
            choices ?? Array.Empty<string>());

    private void SelectMaterialsPrototypeRow(object source)
    {
        if (source is not NativeMaterialRow row) return;
        foreach (var materialRow in _nativeMaterialRows)
        {
            materialRow.IsCurrentSelection = ReferenceEquals(materialRow, row);
        }
        _lastSelectedNativeMaterial = row;
        _workflowPreferencesService.SetLastSelectedMaterialId(row.MaterialID);
        NativeMaterialSelectionText.Text = $"Selected MaterialID: {row.MaterialID}";
        ShowNativeMaterialDetails(row);
    }

    private List<MaterialsPrototypeRow> BuildMaterialsPrototypeRows(
        IReadOnlyList<MaterialsPrototypeColumn> columns)
    {
        var visibleMaterialIds = GetVisibleNativeMaterialIdsFromCurrentFilters();
        return _nativeMaterialRows
            .Where(row => visibleMaterialIds.Contains(row.MaterialID))
            .Select(row =>
            {
                var cells = columns.Select(column => PrototypeCellText(row, column.PropertyName)).ToArray();
                return new MaterialsPrototypeRow(row, row.MaterialID, cells, cells.ToArray(), () => row.IsRowValid);
            })
            .ToList();
    }

    private static List<MaterialsPrototypeColumn> ApplyFastMaterialsLayout(
        List<MaterialsPrototypeColumn> columns,
        IReadOnlyList<WorkflowColumnLayout> savedLayout)
    {
        if (savedLayout.Count == 0) return columns;
        var legacyDuplicateKeyPresent = columns
            .Where(column => column.LayoutKey?.Contains('#', StringComparison.Ordinal) == true)
            .Select(PrototypeColumnBaseKey)
            .Distinct(StringComparer.Ordinal)
            .Any(baseKey => savedLayout.Any(item => string.Equals(item.Key, baseKey, StringComparison.Ordinal)));
        if (legacyDuplicateKeyPresent)
        {
            // Layouts written before duplicate columns received stable identities
            // cannot place individual spacer columns safely. Fall back once to the
            // canonical order; the next layout write uses the new unique keys.
            return columns;
        }
        var savedByKey = savedLayout
            .Where(item => item.DisplayIndex < columns.Count)
            .ToDictionary(item => item.Key, StringComparer.Ordinal);

        var resized = columns
            .Select((column, originalIndex) =>
            {
                var key = PrototypeColumnKey(column);
                return savedByKey.TryGetValue(key, out var saved)
                    ? (Column: column with { Width = saved.Width }, OriginalIndex: originalIndex, SavedIndex: saved.DisplayIndex)
                    : (Column: column, OriginalIndex: originalIndex, SavedIndex: int.MaxValue);
            })
            .OrderBy(item => item.SavedIndex)
            .ThenBy(item => item.OriginalIndex)
            .Select(item => item.Column)
            .ToList();
        return resized.Count == columns.Count ? resized : columns;
    }

    private static string PrototypeColumnKey(MaterialsPrototypeColumn column) =>
        column.LayoutKey ?? PrototypeColumnBaseKey(column);

    private static string PrototypeColumnBaseKey(MaterialsPrototypeColumn column) =>
        !string.IsNullOrWhiteSpace(column.PropertyName)
            ? $"binding:{column.PropertyName}"
            : $"header:{column.Header}";

    private static List<MaterialsPrototypeColumn> AssignStablePrototypeLayoutKeys(
        List<MaterialsPrototypeColumn> columns)
    {
        var totals = columns
            .GroupBy(PrototypeColumnBaseKey, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
        var occurrences = new Dictionary<string, int>(StringComparer.Ordinal);

        return columns.Select(column =>
        {
            var baseKey = PrototypeColumnBaseKey(column);
            occurrences.TryGetValue(baseKey, out var occurrence);
            occurrence++;
            occurrences[baseKey] = occurrence;
            return column with
            {
                LayoutKey = totals[baseKey] > 1
                    ? $"{baseKey}#{occurrence}"
                    : baseKey
            };
        }).ToList();
    }

    private bool ApplyMaterialsPrototypeChanges(IReadOnlyList<MaterialsPrototypeChange> changes)
    {
        if (changes.Count == 0) return true;
        foreach (var change in changes)
        {
            object value = change.Column.EditorKind == MaterialsPrototypeEditorKind.CheckBox
                ? change.NewValue == "✓"
                : change.NewValue;
            SetPropertyValue(change.Row.Source, change.Column.PropertyName!, value);
        }

        MarkNativeMaterialsDirty();
        RefreshNativeMaterialGridValidation();
        QueueNativeMaterialEditRefresh();
        return true;
    }

    private static string PrototypeCellText(object row, string? propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName)) return string.Empty;
        var value = GetPropertyValue(row, propertyName);
        return value switch
        {
            null => string.Empty,
            bool boolean => boolean ? "✓" : "□",
            _ => value.ToString() ?? string.Empty
        };
    }

    private enum MaterialsPrototypeEditorKind
    {
        Text,
        ComboBox,
        CheckBox
    }

    private enum FastGridCellKind
    {
        Standard,
        TensileSample,
        ImpactSample,
        Computed,
        Spacer
    }

    private sealed record MaterialsPrototypeColumn(
        string Header,
        double Width,
        string? PropertyName,
        bool IsReadOnly,
        MaterialsPrototypeEditorKind EditorKind,
        IReadOnlyList<string> Choices,
        FastGridCellKind CellKind = FastGridCellKind.Standard,
        string? LayoutKey = null);

    private sealed record MaterialsPrototypeRow(
        object Source,
        string MaterialId,
        string[] Cells,
        string[] OriginalCells,
        Func<bool> IsValid);

    private sealed record MaterialsPrototypeChange(
        MaterialsPrototypeRow Row,
        MaterialsPrototypeColumn Column,
        int ColumnIndex,
        string OldValue,
        string NewValue);

    private sealed class MaterialsRenderingPrototypeView : UserControl
    {
        private readonly ScrollViewer _scrollViewer;
        private readonly MaterialsRenderingSurface _surface;
        private readonly Canvas _editorLayer;
        private readonly List<MaterialsPrototypeColumn> _columns;
        private readonly List<MaterialsPrototypeRow> _rows;
        private readonly Func<IReadOnlyList<MaterialsPrototypeChange>, bool> _applyChanges;
        private readonly Action<IReadOnlyList<WorkflowColumnLayout>> _saveLayout;
        private readonly Func<IReadOnlyList<MaterialsPrototypeColumn>, List<MaterialsPrototypeRow>> _reloadRows;
        private readonly Action<object> _selectRow;
        private readonly bool _directCanonicalEditing;
        private readonly bool _reloadAfterApply;
        private readonly TextBlock _status;
        private readonly Button _applyButton;
        private readonly Button _reloadButton;
        private long _scrollStarted;
        private Control? _activeEditor;

        public MaterialsRenderingPrototypeView(
            IReadOnlyList<MaterialsPrototypeColumn> columns,
            IReadOnlyList<MaterialsPrototypeRow> rows,
            Func<IReadOnlyList<MaterialsPrototypeChange>, bool> applyChanges,
            Action<IReadOnlyList<WorkflowColumnLayout>> saveLayout,
            Func<IReadOnlyList<MaterialsPrototypeColumn>, List<MaterialsPrototypeRow>> reloadRows,
            Action<object> selectRow,
            bool directCanonicalEditing,
            bool reloadAfterApply = false)
        {
            _columns = columns as List<MaterialsPrototypeColumn> ?? columns.ToList();
            _rows = rows as List<MaterialsPrototypeRow> ?? rows.ToList();
            _applyChanges = applyChanges;
            _saveLayout = saveLayout;
            _reloadRows = reloadRows;
            _selectRow = selectRow;
            _directCanonicalEditing = directCanonicalEditing;
            _reloadAfterApply = reloadAfterApply;
            var root = new DockPanel { Background = Brushes.White };
            var explanation = new TextBlock
            {
                Text = directCanonicalEditing
                    ? "Fast Materials view. Edits enter the normal canonical validation and SQLite auto-save workflow; layout preferences remain separate from the fallback grid."
                    : "Snapshot-only interaction prototype. Changes remain local until explicit Apply; layout preferences remain separate from the fallback grid.",
                TextWrapping = TextWrapping.Wrap,
                Padding = new Thickness(10, 8, 10, 8),
                Background = new SolidColorBrush(Color.FromRgb(239, 246, 255)),
                Foreground = new SolidColorBrush(Color.FromRgb(30, 64, 175))
            };
            if (!directCanonicalEditing)
            {
                DockPanel.SetDock(explanation, Dock.Top);
                root.Children.Add(explanation);
            }

            var footer = new DockPanel
            {
                Background = new SolidColorBrush(Color.FromRgb(248, 250, 252))
            };
            _applyButton = new Button
            {
                Content = "Apply snapshot changes to Materials...",
                IsEnabled = false,
                Visibility = directCanonicalEditing ? Visibility.Collapsed : Visibility.Visible,
                Margin = new Thickness(8, 4, 8, 4),
                Padding = new Thickness(10, 4, 10, 4)
            };
            _applyButton.Click += ApplyButton_Click;
            DockPanel.SetDock(_applyButton, Dock.Right);
            footer.Children.Add(_applyButton);
            _reloadButton = new Button
            {
                Content = "Reload current Materials filters/data",
                Margin = new Thickness(8, 4, 0, 4),
                Padding = new Thickness(10, 4, 10, 4)
            };
            _reloadButton.Click += ReloadButton_Click;
            DockPanel.SetDock(_reloadButton, Dock.Right);
            footer.Children.Add(_reloadButton);
            _status = new TextBlock
            {
                Text = $"{rows.Count:N0} rows × {columns.Count:N0} columns; no scroll measured yet.",
                Padding = new Thickness(10, 6, 10, 6),
                Foreground = new SolidColorBrush(Color.FromRgb(51, 65, 85))
            };
            footer.Children.Add(_status);
            DockPanel.SetDock(footer, Dock.Bottom);
            root.Children.Add(footer);

            _surface = new MaterialsRenderingSurface(columns, rows);
            _surface.FrameRendered += Surface_FrameRendered;
            _surface.CellActivated += Surface_CellActivated;
            _surface.SelectedRowChanged += Surface_SelectedRowChanged;
            _surface.SnapshotChanged += (_, _) => HandleSnapshotChanged();
            _surface.LayoutChanged += (_, _) => _saveLayout(_surface.CaptureLayout());
            _surface.EnsureCellVisible += Surface_EnsureCellVisible;
            _editorLayer = new Canvas();
            _scrollViewer = new ScrollViewer
            {
                Content = _surface,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Top,
                CanContentScroll = false,
                IsDeferredScrollingEnabled = true
            };
            _scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            _scrollViewer.SizeChanged += (_, _) => _surface.SetViewport(
                _scrollViewer.HorizontalOffset,
                _scrollViewer.VerticalOffset,
                _scrollViewer.ViewportWidth,
                _scrollViewer.ViewportHeight);
            var viewportHost = new Grid();
            viewportHost.Children.Add(_scrollViewer);
            Panel.SetZIndex(_editorLayer, 10);
            viewportHost.Children.Add(_editorLayer);
            root.Children.Add(viewportHost);
            Content = root;

            Loaded += (_, _) =>
            {
                _surface.SetViewport(0, 0, _scrollViewer.ViewportWidth, _scrollViewer.ViewportHeight);
                _surface.InvalidateVisual();
            };
        }

        public bool ConfirmCanClose()
        {
            CloseEditor(commit: true);
            var changes = GetChanges();
            if (changes.Count == 0)
            {
                _saveLayout(_surface.CaptureLayout());
                return true;
            }

            var result = MessageBox.Show(
                Window.GetWindow(this),
                $"{changes.Count:N0} unapplied snapshot change(s) remain.\n\n" +
                "Yes = apply the changes to canonical Materials and close.\n" +
                "No = discard the snapshot changes and close.\n" +
                "Cancel = keep this window open.",
                "Unapplied Materials Prototype Changes",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Warning,
                MessageBoxResult.Cancel);
            if (result == MessageBoxResult.Cancel)
            {
                return false;
            }
            if (result == MessageBoxResult.Yes)
            {
                if (!_applyChanges(changes))
                {
                    return false;
                }
                foreach (var row in _rows)
                {
                    Array.Copy(row.Cells, row.OriginalCells, row.Cells.Length);
                }
            }
            _saveLayout(_surface.CaptureLayout());
            return true;
        }

        public void ResetLayout(IReadOnlyList<MaterialsPrototypeColumn> defaultColumns)
        {
            CloseEditor(commit: true);
            _surface.ResetLayout(defaultColumns);
            _saveLayout(_surface.CaptureLayout());
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_activeEditor is not null)
            {
                CloseEditor(commit: true);
            }
            if (Math.Abs(e.HorizontalChange) > 0.01 || Math.Abs(e.VerticalChange) > 0.01)
            {
                _scrollStarted = Stopwatch.GetTimestamp();
            }

            _surface.SetViewport(
                e.HorizontalOffset,
                e.VerticalOffset,
                e.ViewportWidth,
                e.ViewportHeight);
        }

        private void Surface_FrameRendered(object? sender, MaterialsPrototypeRenderEventArgs e)
        {
            var elapsed = _scrollStarted == 0
                ? TimeSpan.Zero
                : Stopwatch.GetElapsedTime(_scrollStarted);
            _status.Text =
                $"{e.VisibleRows:N0} visible rows × {e.VisibleColumns:N0} visible columns = {e.DrawnCells:N0} drawn cells; " +
                $"offset {e.HorizontalOffset:0}/{Math.Max(0, _scrollViewer.ScrollableWidth):0}; " +
                $"render {e.RenderMilliseconds:0.0} ms" +
                (_scrollStarted == 0 ? "." : $"; scroll-to-render {elapsed.TotalMilliseconds:0.0} ms.");
            _scrollStarted = 0;
        }

        private void Surface_CellActivated(object? sender, MaterialsPrototypeCellEventArgs e)
        {
            CloseEditor(commit: true);
            var column = _columns[e.ColumnIndex];
            if (column.IsReadOnly || string.IsNullOrWhiteSpace(column.PropertyName))
            {
                FocusSurfaceAfterMouseClick();
                return;
            }

            if (column.EditorKind == MaterialsPrototypeEditorKind.CheckBox)
            {
                if (e.CheckBoxHit)
                {
                    var current = _rows[e.RowIndex].Cells[e.ColumnIndex];
                    _rows[e.RowIndex].Cells[e.ColumnIndex] = current == "✓" ? "□" : "✓";
                    _surface.InvalidateVisual();
                    HandleSnapshotChanged();
                }
                FocusSurfaceAfterMouseClick();
                return;
            }

            Control editor;
            if (column.EditorKind == MaterialsPrototypeEditorKind.ComboBox)
            {
                var comboBox = new ComboBox
                {
                    ItemsSource = column.Choices,
                    SelectedItem = _rows[e.RowIndex].Cells[e.ColumnIndex],
                    Padding = new Thickness(4, 0, 4, 0)
                };
                comboBox.DropDownClosed += (_, _) => CloseEditor(commit: true);
                editor = comboBox;
            }
            else
            {
                var textBox = new TextBox
                {
                    Text = _rows[e.RowIndex].Cells[e.ColumnIndex],
                    Padding = new Thickness(4, 0, 4, 0),
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                textBox.SelectAll();
                editor = textBox;
            }

            editor.Tag = e;
            editor.Width = Math.Max(30, e.CellBounds.Width);
            editor.Height = Math.Max(22, e.CellBounds.Height);
            if (editor is TextBox)
            {
                editor.LostKeyboardFocus += (_, _) => CloseEditor(commit: true);
            }
            editor.PreviewKeyDown += Editor_PreviewKeyDown;
            var editorPosition = _surface.TranslatePoint(
                new Point(e.CellBounds.X, e.CellBounds.Y),
                _editorLayer);
            Canvas.SetLeft(editor, editorPosition.X);
            Canvas.SetTop(editor, editorPosition.Y);
            Panel.SetZIndex(editor, 10);
            _editorLayer.Children.Add(editor);
            _activeEditor = editor;
            _ = Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, () =>
            {
                if (!ReferenceEquals(_activeEditor, editor)) return;
                editor.Focus();
                Keyboard.Focus(editor);
                if (editor is TextBox textBox)
                {
                    textBox.SelectAll();
                }
                else if (editor is ComboBox comboBox)
                {
                    comboBox.IsDropDownOpen = true;
                }
            });
        }

        private void Surface_SelectedRowChanged(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < _rows.Count)
            {
                _selectRow(_rows[rowIndex].Source);
            }
        }

        private void FocusSurfaceAfterMouseClick()
        {
            _ = Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, () =>
            {
                _surface.Focus();
                Keyboard.Focus(_surface);
            });
        }

        private void Surface_EnsureCellVisible(Rect cellBounds)
        {
            var horizontalOffset = _scrollViewer.HorizontalOffset;
            var verticalOffset = _scrollViewer.VerticalOffset;
            if (cellBounds.Left < horizontalOffset)
                horizontalOffset = cellBounds.Left;
            else if (cellBounds.Right > horizontalOffset + _scrollViewer.ViewportWidth)
                horizontalOffset = cellBounds.Right - _scrollViewer.ViewportWidth;

            if (cellBounds.Top < verticalOffset + 32)
                verticalOffset = Math.Max(0, cellBounds.Top - 32);
            else if (cellBounds.Bottom > verticalOffset + _scrollViewer.ViewportHeight)
                verticalOffset = cellBounds.Bottom - _scrollViewer.ViewportHeight;

            _scrollViewer.ScrollToHorizontalOffset(Math.Max(0, horizontalOffset));
            _scrollViewer.ScrollToVerticalOffset(Math.Max(0, verticalOffset));
        }

        private void Editor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox && _activeEditor?.Tag is MaterialsPrototypeCellEventArgs cell)
            {
                var movement = e.Key switch
                {
                    Key.Left => (Row: 0, Column: -1),
                    Key.Right => (Row: 0, Column: 1),
                    Key.Up => (Row: -1, Column: 0),
                    Key.Down => (Row: 1, Column: 0),
                    _ => (Row: 0, Column: 0)
                };
                if (movement.Row != 0 || movement.Column != 0)
                {
                    e.Handled = true;
                    CloseEditor(commit: true);
                    _surface.MoveFromEditor(cell.RowIndex, cell.ColumnIndex, movement.Row, movement.Column);
                    return;
                }
            }
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                e.Handled = true;
                var currentCell = _activeEditor?.Tag as MaterialsPrototypeCellEventArgs;
                CloseEditor(commit: true);
                if (e.Key == Key.Tab && currentCell is not null)
                {
                    var direction = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? -1 : 1;
                    _ = Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ContextIdle, () =>
                        _surface.MoveFromEditor(
                            currentCell.RowIndex,
                            currentCell.ColumnIndex,
                            0,
                            direction));
                }
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                CloseEditor(commit: false);
            }
        }

        private void CloseEditor(bool commit)
        {
            if (_activeEditor is null) return;
            var editor = _activeEditor;
            _activeEditor = null;
            if (commit && editor.Tag is MaterialsPrototypeCellEventArgs cell)
            {
                _rows[cell.RowIndex].Cells[cell.ColumnIndex] = editor switch
                {
                    TextBox textBox => textBox.Text,
                    ComboBox comboBox => comboBox.SelectedItem?.ToString() ?? comboBox.Text,
                    _ => _rows[cell.RowIndex].Cells[cell.ColumnIndex]
                };
            }

            _editorLayer.Children.Remove(editor);
            _surface.InvalidateVisual();
            _surface.Focus();
            HandleSnapshotChanged();
        }

        private void HandleSnapshotChanged()
        {
            if (!_directCanonicalEditing)
            {
                UpdateApplyState();
                return;
            }

            var changes = GetChanges();
            if (changes.Count == 0) return;
            if (!_applyChanges(changes))
            {
                foreach (var change in changes)
                {
                    change.Row.Cells[change.ColumnIndex] = change.OldValue;
                }
                _surface.InvalidateVisual();
                _status.Text = "Invalid value was not applied; the cell was restored.";
                return;
            }
            if (_reloadAfterApply)
            {
                RefreshCurrentRowsFromSources();
                UpdateApplyState();
                _status.Text = $"Saved {changes.Count:N0} changed field(s) through the canonical auto-save workflow.";
                return;
            }
            foreach (var row in _rows)
            {
                Array.Copy(row.Cells, row.OriginalCells, row.Cells.Length);
            }
            UpdateApplyState();
            _status.Text = $"Saved {changes.Count:N0} changed field(s) through the canonical Materials auto-save workflow.";
        }

        private void RefreshCurrentRowsFromSources()
        {
            foreach (var row in _rows)
            {
                for (var columnIndex = 0; columnIndex < _columns.Count; columnIndex++)
                {
                    var value = PrototypeCellText(row.Source, _columns[columnIndex].PropertyName);
                    row.Cells[columnIndex] = value;
                    row.OriginalCells[columnIndex] = value;
                }
            }
            _surface.InvalidateVisual();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            CloseEditor(commit: true);
            var changes = GetChanges();
            if (changes.Count == 0) return;
            var materialCount = changes.Select(change => change.Row.MaterialId).Distinct(StringComparer.OrdinalIgnoreCase).Count();
            var confirmation = MessageBox.Show(
                Window.GetWindow(this),
                $"Apply {changes.Count:N0} changed field(s) across {materialCount:N0} MaterialID(s) to canonical Materials?\n\n" +
                "This will enter the normal validation and SQLite auto-save workflow. No application files are changed.",
                "Apply Materials Prototype Changes",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);
            if (confirmation != MessageBoxResult.Yes) return;

            if (!_applyChanges(changes)) return;
            foreach (var row in _rows)
            {
                Array.Copy(row.Cells, row.OriginalCells, row.Cells.Length);
            }
            UpdateApplyState();
            _status.Text = $"Applied {changes.Count:N0} changed field(s) to canonical Materials; normal validation/auto-save queued.";
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            ReloadFromCanonical("current Materials filters/data");
        }

        public bool ReloadFromCanonical(string reason)
        {
            return SynchronizeFromCanonical(reason, preferredSelection: null, resetVerticalOffset: true);
        }

        public bool SynchronizeFromCanonical(
            string reason,
            object? preferredSelection = null,
            bool resetVerticalOffset = false)
        {
            CloseEditor(commit: true);
            var changes = GetChanges();
            if (changes.Count > 0)
            {
                var result = MessageBox.Show(
                    Window.GetWindow(this),
                    $"{changes.Count:N0} unapplied snapshot change(s) remain.\n\n" +
                    "Yes = apply before reload.\n" +
                    "No = discard and reload.\n" +
                    "Cancel = keep the current snapshot.",
                    "Reload Materials Prototype",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning,
                    MessageBoxResult.Cancel);
                if (result == MessageBoxResult.Cancel) return false;
                if (result == MessageBoxResult.Yes && !_applyChanges(changes)) return false;
            }

            var selectedSource = preferredSelection ?? _surface.SelectedSource;
            var reloaded = _reloadRows(_columns);
            var reloadedBySource = reloaded.ToDictionary(row => row.Source);
            var existingSources = _rows.Select(row => row.Source).ToList();
            if (existingSources.Count == reloaded.Count &&
                existingSources.All(reloadedBySource.ContainsKey))
            {
                RefreshCurrentRowsFromSources();
                _surface.SelectSource(selectedSource);
                UpdateApplyState();
                _status.Text = $"Refreshed {_rows.Count:N0} row(s) from {reason}.";
                return true;
            }

            var ordered = existingSources
                .Where(reloadedBySource.ContainsKey)
                .Select(source => reloadedBySource[source])
                .ToList();
            var retainedSources = ordered.Select(row => row.Source).ToHashSet();
            ordered.AddRange(reloaded.Where(row => !retainedSources.Contains(row.Source)));
            _rows.Clear();
            _rows.AddRange(ordered);
            _surface.ReplaceRows(selectedSource);
            var horizontalOffset = _scrollViewer.HorizontalOffset;
            var verticalOffset = resetVerticalOffset ? 0 : _scrollViewer.VerticalOffset;
            _scrollViewer.ScrollToVerticalOffset(verticalOffset);
            _surface.SetViewport(
                horizontalOffset,
                verticalOffset,
                _scrollViewer.ViewportWidth,
                _scrollViewer.ViewportHeight);
            UpdateApplyState();
            _status.Text = $"Reloaded {_rows.Count:N0} row(s) from {reason}.";
            return true;
        }

        private List<MaterialsPrototypeChange> GetChanges()
        {
            var changes = new List<MaterialsPrototypeChange>();
            foreach (var row in _rows)
            {
                for (var columnIndex = 0; columnIndex < _columns.Count; columnIndex++)
                {
                    var column = _columns[columnIndex];
                    if (column.IsReadOnly ||
                        string.IsNullOrWhiteSpace(column.PropertyName) ||
                        string.Equals(row.Cells[columnIndex], row.OriginalCells[columnIndex], StringComparison.Ordinal))
                    {
                        continue;
                    }
                    changes.Add(new MaterialsPrototypeChange(
                        row,
                        column,
                        columnIndex,
                        row.OriginalCells[columnIndex],
                        row.Cells[columnIndex]));
                }
            }
            return changes;
        }

        private void UpdateApplyState()
        {
            var changes = GetChanges();
            _applyButton.IsEnabled = changes.Count > 0;
            _applyButton.Content = changes.Count == 0
                ? "Apply snapshot changes to Materials..."
                : $"Apply {changes.Count:N0} snapshot change(s) to Materials...";
        }
    }

    private sealed class MaterialsRenderingSurface : FrameworkElement
    {
        private const double HeaderHeight = 32;
        private const double RowHeight = 25;
        private const double CellPadding = 5;
        private readonly TensileSampleValueBrushConverter _tensileBrushConverter = new();
        private readonly ImpactSampleValueBrushConverter _impactBrushConverter = new();
        private readonly List<MaterialsPrototypeColumn> _columns;
        private readonly List<MaterialsPrototypeRow> _rows;
        private readonly double[] _columnOffsets;
        private readonly double[] _columnWidths;
        private double _horizontalOffset;
        private double _verticalOffset;
        private double _viewportWidth = 1;
        private double _viewportHeight = 1;
        private double _contentHeight;
        private int _selectedRow = -1;
        private int _selectedColumn = -1;
        private int _resizingColumn = -1;
        private int _draggingColumn = -1;
        private double _resizeStartX;
        private double _resizeStartWidth;
        private double _columnDragStartX;
        private bool _columnDragMoved;
        private int _sortColumn = -1;
        private bool _sortAscending = true;

        public event EventHandler<MaterialsPrototypeRenderEventArgs>? FrameRendered;
        public event EventHandler<MaterialsPrototypeCellEventArgs>? CellActivated;
        public event EventHandler? SnapshotChanged;
        public event EventHandler? LayoutChanged;
        public event Action<Rect>? EnsureCellVisible;
        public event Action<int>? SelectedRowChanged;

        public MaterialsRenderingSurface(
            IReadOnlyList<MaterialsPrototypeColumn> columns,
            IReadOnlyList<MaterialsPrototypeRow> rows)
        {
            _columns = columns as List<MaterialsPrototypeColumn> ?? columns.ToList();
            _rows = rows as List<MaterialsPrototypeRow> ?? rows.ToList();
            _columnOffsets = new double[columns.Count + 1];
            _columnWidths = columns.Select(column => column.Width).ToArray();
            RebuildColumnOffsets();
            Width = Math.Max(1, _columnOffsets[^1]);
            _contentHeight = Math.Max(HeaderHeight + RowHeight, HeaderHeight + rows.Count * RowHeight);
            Height = _contentHeight;
            ClipToBounds = true;
            Focusable = true;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        private void RebuildColumnOffsets()
        {
            _columnOffsets[0] = 0;
            for (var index = 0; index < _columns.Count; index++)
            {
                _columnOffsets[index + 1] = _columnOffsets[index] + _columnWidths[index];
            }
        }

        public void SetViewport(double horizontalOffset, double verticalOffset, double viewportWidth, double viewportHeight)
        {
            if (double.IsFinite(viewportWidth) && viewportWidth > 1)
            {
                _viewportWidth = viewportWidth;
            }
            if (double.IsFinite(viewportHeight) && viewportHeight > 1)
            {
                _viewportHeight = viewportHeight;
            }
            Height = Math.Max(_contentHeight, _viewportHeight);
            _horizontalOffset = Math.Clamp(
                double.IsFinite(horizontalOffset) ? horizontalOffset : 0,
                0,
                Math.Max(0, Width - _viewportWidth));
            _verticalOffset = Math.Clamp(
                double.IsFinite(verticalOffset) ? verticalOffset : 0,
                0,
                Math.Max(0, _contentHeight - _viewportHeight));
            InvalidateVisual();
        }

        public void MoveFromEditor(int rowIndex, int columnIndex, int rowDelta, int columnDelta)
        {
            _selectedRow = Math.Clamp(rowIndex + rowDelta, 0, Math.Max(0, _rows.Count - 1));
            _selectedColumn = Math.Clamp(columnIndex + columnDelta, 0, Math.Max(0, _columns.Count - 1));
            var bounds = CurrentCellBounds();
            EnsureCellVisible?.Invoke(bounds);
            InvalidateVisual();
            _ = Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, () =>
            {
                var column = _columns[_selectedColumn];
                if (!column.IsReadOnly &&
                    !string.IsNullOrWhiteSpace(column.PropertyName) &&
                    column.EditorKind != MaterialsPrototypeEditorKind.CheckBox)
                {
                    ActivateCurrentCell(checkBoxHit: false);
                }
                else
                {
                    Focus();
                    Keyboard.Focus(this);
                }
            });
        }

        public IReadOnlyList<WorkflowColumnLayout> CaptureLayout() =>
            _columns
                .Select((column, index) => new WorkflowColumnLayout(
                    PrototypeColumnKey(column),
                    _columnWidths[index],
                    index))
                .ToList();

        public void ResetLayout(IReadOnlyList<MaterialsPrototypeColumn> defaultColumns)
        {
            if (defaultColumns.Count != _columns.Count) return;

            for (var targetIndex = 0; targetIndex < defaultColumns.Count; targetIndex++)
            {
                var targetKey = PrototypeColumnKey(defaultColumns[targetIndex]);
                var sourceIndex = -1;
                for (var index = targetIndex; index < _columns.Count; index++)
                {
                    if (!string.Equals(PrototypeColumnKey(_columns[index]), targetKey, StringComparison.Ordinal)) continue;
                    sourceIndex = index;
                    break;
                }
                if (sourceIndex < 0) return;
                if (sourceIndex != targetIndex)
                {
                    MoveColumn(sourceIndex, targetIndex);
                }
                _columnWidths[targetIndex] = Math.Clamp(defaultColumns[targetIndex].Width, 50, 500);
            }

            RebuildColumnOffsets();
            Width = Math.Max(1, _columnOffsets[^1]);
            InvalidateMeasure();
            InvalidateVisual();
            LayoutChanged?.Invoke(this, EventArgs.Empty);
        }

        public object? SelectedSource =>
            _selectedRow >= 0 && _selectedRow < _rows.Count
                ? _rows[_selectedRow].Source
                : null;

        public void SelectSource(object? source)
        {
            if (source is null) return;
            var rowIndex = _rows.FindIndex(row => ReferenceEquals(row.Source, source));
            if (rowIndex < 0) return;
            _selectedRow = rowIndex;
            _selectedColumn = Math.Max(0, _selectedColumn);
            EnsureCellVisible?.Invoke(CurrentCellBounds());
            InvalidateVisual();
        }

        public void ReplaceRows(object? selectedSource = null)
        {
            _selectedRow = selectedSource is null
                ? -1
                : _rows.FindIndex(row => ReferenceEquals(row.Source, selectedSource));
            if (_selectedRow < 0) _selectedColumn = -1;
            _contentHeight = Math.Max(HeaderHeight + RowHeight, HeaderHeight + _rows.Count * RowHeight);
            Height = Math.Max(_contentHeight, _viewportHeight);
            InvalidateMeasure();
            InvalidateVisual();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Focus();
            var position = e.GetPosition(this);
            if (position.Y >= _verticalOffset && position.Y < _verticalOffset + HeaderHeight)
            {
                var separator = FindHeaderSeparator(position.X);
                if (separator >= 0)
                {
                    _resizingColumn = separator;
                    _resizeStartX = position.X;
                    _resizeStartWidth = _columnWidths[separator];
                    CaptureMouse();
                    e.Handled = true;
                }
                else
                {
                    _draggingColumn = FindColumnAt(position.X);
                    _columnDragStartX = position.X;
                    _columnDragMoved = false;
                    if (_draggingColumn >= 0)
                    {
                        CaptureMouse();
                        e.Handled = true;
                    }
                }
                return;
            }
            var rowIndex = (int)Math.Floor((position.Y - HeaderHeight) / RowHeight);
            if (rowIndex >= 0 && rowIndex < _rows.Count)
            {
                var selectedRowChanged = _selectedRow != rowIndex;
                _selectedRow = rowIndex;
                _selectedColumn = FindColumnAt(position.X);
                InvalidateVisual();
                if (selectedRowChanged)
                {
                    SelectedRowChanged?.Invoke(_selectedRow);
                }
                var columnIndex = _selectedColumn;
                if (columnIndex >= 0)
                {
                    var cellBounds = new Rect(
                        _columnOffsets[columnIndex],
                        HeaderHeight + rowIndex * RowHeight,
                        _columnWidths[columnIndex],
                        RowHeight);
                    var checkBoxBounds = new Rect(
                        cellBounds.X + Math.Max(3, (cellBounds.Width - 14) / 2),
                        cellBounds.Y + Math.Max(3, (cellBounds.Height - 14) / 2),
                        14,
                        14);
                    CellActivated?.Invoke(this, new MaterialsPrototypeCellEventArgs(
                        rowIndex,
                        columnIndex,
                        cellBounds,
                        checkBoxBounds.Contains(position)));
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (_rows.Count == 0 || _columns.Count == 0) return;
            if (_selectedRow < 0) _selectedRow = 0;
            if (_selectedColumn < 0) _selectedColumn = 0;

            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0 && e.Key == Key.C)
            {
                Clipboard.SetText(_rows[_selectedRow].Cells[_selectedColumn] ?? string.Empty);
                e.Handled = true;
                return;
            }
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0 && e.Key == Key.V)
            {
                PasteCurrentCell();
                e.Handled = true;
                return;
            }

            var previousRow = _selectedRow;
            var moved = true;
            switch (e.Key)
            {
                case Key.Left:
                    _selectedColumn = Math.Max(0, _selectedColumn - 1);
                    break;
                case Key.Right:
                    _selectedColumn = Math.Min(_columns.Count - 1, _selectedColumn + 1);
                    break;
                case Key.Up:
                    _selectedRow = Math.Max(0, _selectedRow - 1);
                    break;
                case Key.Down:
                    _selectedRow = Math.Min(_rows.Count - 1, _selectedRow + 1);
                    break;
                case Key.Tab:
                    _selectedColumn = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)
                        ? Math.Max(0, _selectedColumn - 1)
                        : Math.Min(_columns.Count - 1, _selectedColumn + 1);
                    break;
                case Key.Enter:
                case Key.F2:
                    ActivateCurrentCell(checkBoxHit: false);
                    e.Handled = true;
                    return;
                case Key.Space when _columns[_selectedColumn].EditorKind == MaterialsPrototypeEditorKind.CheckBox:
                    ToggleCurrentCheckBox();
                    e.Handled = true;
                    return;
                default:
                    moved = false;
                    break;
            }

            if (!moved) return;
            if (_selectedRow != previousRow)
            {
                SelectedRowChanged?.Invoke(_selectedRow);
            }
            EnsureCellVisible?.Invoke(CurrentCellBounds());
            InvalidateVisual();
            ActivateSelectedEditableAfterNavigation();
            e.Handled = true;
        }

        private void ActivateSelectedEditableAfterNavigation()
        {
            var column = _columns[_selectedColumn];
            if (column.IsReadOnly ||
                string.IsNullOrWhiteSpace(column.PropertyName) ||
                column.EditorKind == MaterialsPrototypeEditorKind.CheckBox)
            {
                return;
            }

            _ = Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input, () =>
                ActivateCurrentCell(checkBoxHit: false));
        }

        private void PasteCurrentCell()
        {
            var column = _columns[_selectedColumn];
            if (column.IsReadOnly || string.IsNullOrWhiteSpace(column.PropertyName) || !Clipboard.ContainsText()) return;
            var text = Clipboard.GetText();
            if (column.EditorKind == MaterialsPrototypeEditorKind.CheckBox)
            {
                var normalized = text.Trim();
                if (normalized.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                    normalized.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                    normalized == "1" ||
                    normalized == "✓")
                    _rows[_selectedRow].Cells[_selectedColumn] = "✓";
                else if (normalized.Equals("false", StringComparison.OrdinalIgnoreCase) ||
                         normalized.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                         normalized == "0" ||
                         normalized == "□")
                    _rows[_selectedRow].Cells[_selectedColumn] = "□";
                else
                    return;
            }
            else if (column.EditorKind == MaterialsPrototypeEditorKind.ComboBox)
            {
                var choice = column.Choices.FirstOrDefault(value =>
                    string.Equals(value, text.Trim(), StringComparison.OrdinalIgnoreCase));
                if (choice is null) return;
                _rows[_selectedRow].Cells[_selectedColumn] = choice;
            }
            else
            {
                _rows[_selectedRow].Cells[_selectedColumn] = text;
            }
            SnapshotChanged?.Invoke(this, EventArgs.Empty);
            InvalidateVisual();
        }

        private void ToggleCurrentCheckBox()
        {
            var column = _columns[_selectedColumn];
            if (column.IsReadOnly || column.EditorKind != MaterialsPrototypeEditorKind.CheckBox) return;
            _rows[_selectedRow].Cells[_selectedColumn] =
                _rows[_selectedRow].Cells[_selectedColumn] == "✓" ? "□" : "✓";
            SnapshotChanged?.Invoke(this, EventArgs.Empty);
            InvalidateVisual();
        }

        private void ActivateCurrentCell(bool checkBoxHit)
        {
            var bounds = CurrentCellBounds();
            CellActivated?.Invoke(this, new MaterialsPrototypeCellEventArgs(
                _selectedRow,
                _selectedColumn,
                bounds,
                checkBoxHit));
        }

        private Rect CurrentCellBounds() => new(
            _columnOffsets[_selectedColumn],
            HeaderHeight + _selectedRow * RowHeight,
            _columnWidths[_selectedColumn],
            RowHeight);

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var position = e.GetPosition(this);
            if (_resizingColumn >= 0 && e.LeftButton == MouseButtonState.Pressed)
            {
                _columnWidths[_resizingColumn] = Math.Clamp(
                    _resizeStartWidth + position.X - _resizeStartX,
                    50,
                    500);
                RebuildColumnOffsets();
                Width = Math.Max(1, _columnOffsets[^1]);
                Cursor = Cursors.SizeWE;
                InvalidateMeasure();
                InvalidateVisual();
                e.Handled = true;
                return;
            }
            if (_draggingColumn >= 0 && e.LeftButton == MouseButtonState.Pressed)
            {
                if (Math.Abs(position.X - _columnDragStartX) >= 8)
                {
                    _columnDragMoved = true;
                    var targetColumn = FindColumnAt(position.X);
                    if (targetColumn >= 0 && targetColumn != _draggingColumn)
                    {
                        MoveColumn(_draggingColumn, targetColumn);
                        _draggingColumn = targetColumn;
                        _columnDragStartX = position.X;
                    }
                }
                Cursor = Cursors.SizeAll;
                e.Handled = true;
                return;
            }

            Cursor = position.Y >= _verticalOffset &&
                     position.Y < _verticalOffset + HeaderHeight &&
                     FindHeaderSeparator(position.X) >= 0
                ? Cursors.SizeWE
                : Cursors.Arrow;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (_resizingColumn < 0 && _draggingColumn < 0) return;
            var sortColumn = _resizingColumn < 0 && _draggingColumn >= 0 && !_columnDragMoved
                ? _draggingColumn
                : -1;
            _resizingColumn = -1;
            _draggingColumn = -1;
            ReleaseMouseCapture();
            Cursor = Cursors.Arrow;
            if (sortColumn >= 0)
            {
                SortRows(sortColumn);
            }
            else
            {
                LayoutChanged?.Invoke(this, EventArgs.Empty);
            }
            e.Handled = true;
        }

        private void SortRows(int columnIndex)
        {
            var selected = _selectedRow >= 0 && _selectedRow < _rows.Count
                ? _rows[_selectedRow]
                : null;
            if (_sortColumn == columnIndex)
                _sortAscending = !_sortAscending;
            else
            {
                _sortColumn = columnIndex;
                _sortAscending = true;
            }

            _rows.Sort((left, right) =>
            {
                var comparison = CompareCellValues(left.Cells[columnIndex], right.Cells[columnIndex]);
                if (comparison == 0)
                    comparison = StringComparer.OrdinalIgnoreCase.Compare(left.MaterialId, right.MaterialId);
                return _sortAscending ? comparison : -comparison;
            });
            _selectedRow = selected is null ? -1 : _rows.IndexOf(selected);
            InvalidateVisual();
        }

        private static int CompareCellValues(string left, string right)
        {
            if (decimal.TryParse(left, NumberStyles.Number, CultureInfo.CurrentCulture, out var leftNumber) &&
                decimal.TryParse(right, NumberStyles.Number, CultureInfo.CurrentCulture, out var rightNumber))
            {
                return leftNumber.CompareTo(rightNumber);
            }
            return StringComparer.CurrentCultureIgnoreCase.Compare(left ?? string.Empty, right ?? string.Empty);
        }

        private void MoveColumn(int fromIndex, int toIndex)
        {
            var column = _columns[fromIndex];
            _columns.RemoveAt(fromIndex);
            _columns.Insert(toIndex, column);

            var width = _columnWidths[fromIndex];
            MoveArrayItem(_columnWidths, fromIndex, toIndex, width);
            foreach (var row in _rows)
            {
                var cell = row.Cells[fromIndex];
                MoveArrayItem(row.Cells, fromIndex, toIndex, cell);
                var originalCell = row.OriginalCells[fromIndex];
                MoveArrayItem(row.OriginalCells, fromIndex, toIndex, originalCell);
            }
            if (_selectedColumn == fromIndex)
                _selectedColumn = toIndex;
            else if (fromIndex < toIndex && _selectedColumn > fromIndex && _selectedColumn <= toIndex)
                _selectedColumn--;
            else if (fromIndex > toIndex && _selectedColumn >= toIndex && _selectedColumn < fromIndex)
                _selectedColumn++;
            if (_sortColumn == fromIndex)
                _sortColumn = toIndex;
            else if (fromIndex < toIndex && _sortColumn > fromIndex && _sortColumn <= toIndex)
                _sortColumn--;
            else if (fromIndex > toIndex && _sortColumn >= toIndex && _sortColumn < fromIndex)
                _sortColumn++;

            RebuildColumnOffsets();
            Width = Math.Max(1, _columnOffsets[^1]);
            InvalidateMeasure();
            InvalidateVisual();
        }

        private static void MoveArrayItem<T>(T[] values, int fromIndex, int toIndex, T value)
        {
            if (fromIndex < toIndex)
            {
                Array.Copy(values, fromIndex + 1, values, fromIndex, toIndex - fromIndex);
            }
            else
            {
                Array.Copy(values, toIndex, values, toIndex + 1, fromIndex - toIndex);
            }
            values[toIndex] = value;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var started = Stopwatch.GetTimestamp();
            base.OnRender(drawingContext);

            var firstColumn = FindFirstVisibleColumn(_horizontalOffset);
            var lastColumn = FindLastVisibleColumn(_horizontalOffset + _viewportWidth);
            var firstRow = Math.Clamp((int)Math.Floor(Math.Max(0, _verticalOffset - HeaderHeight) / RowHeight), 0, Math.Max(0, _rows.Count - 1));
            var lastRow = Math.Clamp((int)Math.Ceiling(Math.Max(0, _verticalOffset + _viewportHeight - HeaderHeight) / RowHeight), 0, _rows.Count);
            var dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip;
            var typeface = new Typeface("Segoe UI");
            var headerTypeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.SemiBold, FontStretches.Normal);
            var gridPen = new Pen(new SolidColorBrush(Color.FromRgb(209, 213, 219)), 1);
            var headerBrush = new LinearGradientBrush(
                new GradientStopCollection
                {
                    new(Color.FromRgb(255, 255, 255), 0),
                    new(Color.FromRgb(248, 248, 248), 0.55),
                    new(Color.FromRgb(229, 231, 235), 1)
                },
                new Point(0, 0),
                new Point(0, 1));
            var selectedBrush = new SolidColorBrush(Color.FromRgb(219, 234, 254));
            var invalidBrush = new SolidColorBrush(Color.FromRgb(254, 242, 242));
            var textBrush = new SolidColorBrush(Color.FromRgb(15, 23, 42));

            var drawnCells = 0;
            for (var rowIndex = firstRow; rowIndex < lastRow; rowIndex++)
            {
                var y = HeaderHeight + rowIndex * RowHeight;
                var rowBrush = rowIndex == _selectedRow
                     ? selectedBrush
                     : !_rows[rowIndex].IsValid() ? invalidBrush
                     : rowIndex % 2 == 0 ? Brushes.White : new SolidColorBrush(Color.FromRgb(248, 250, 252));
                for (var columnIndex = firstColumn; columnIndex <= lastColumn && columnIndex < _columns.Count; columnIndex++)
                {
                    var x = _columnOffsets[columnIndex];
                    var width = _columnWidths[columnIndex];
                    var cellBrush = rowIndex == _selectedRow || !_rows[rowIndex].IsValid()
                        ? rowBrush
                        : _columns[columnIndex].CellKind switch
                        {
                            FastGridCellKind.TensileSample => (Brush)_tensileBrushConverter.Convert(
                                _rows[rowIndex].Cells[columnIndex],
                                typeof(Brush),
                                null!,
                                CultureInfo.CurrentCulture),
                            FastGridCellKind.ImpactSample => (Brush)_impactBrushConverter.Convert(
                                _rows[rowIndex].Cells[columnIndex],
                                typeof(Brush),
                                null!,
                                CultureInfo.CurrentCulture),
                            FastGridCellKind.Computed => new SolidColorBrush(Color.FromRgb(241, 245, 249)),
                            FastGridCellKind.Spacer => new SolidColorBrush(Color.FromRgb(203, 213, 225)),
                            _ => rowBrush
                        };
                    drawingContext.DrawRectangle(cellBrush, gridPen, new Rect(x, y, width, RowHeight));
                    if (_columns[columnIndex].EditorKind == MaterialsPrototypeEditorKind.CheckBox)
                    {
                        DrawCheckBox(drawingContext, _rows[rowIndex].Cells[columnIndex] == "✓", x, y, width, RowHeight, textBrush);
                    }
                    else
                    {
                        DrawText(drawingContext, _rows[rowIndex].Cells[columnIndex], x, y, width, RowHeight, typeface, textBrush, dpi);
                    }
                    if (rowIndex == _selectedRow && columnIndex == _selectedColumn)
                    {
                        drawingContext.DrawRectangle(
                            null,
                            new Pen(new SolidColorBrush(Color.FromRgb(37, 99, 235)), 2),
                            new Rect(x + 1, y + 1, Math.Max(1, width - 2), RowHeight - 2));
                    }
                    drawnCells++;
                }
            }

            // Draw the header after the rows at the current vertical offset. The
            // ScrollViewer translates the full surface, so this world-space offset
            // keeps the header fixed at the top of the visible viewport.
            for (var columnIndex = firstColumn; columnIndex <= lastColumn && columnIndex < _columns.Count; columnIndex++)
            {
                var x = _columnOffsets[columnIndex];
                var width = _columnWidths[columnIndex];
                drawingContext.DrawRectangle(headerBrush, gridPen, new Rect(x, _verticalOffset, width, HeaderHeight));
                var header = _columns[columnIndex].Header +
                             (columnIndex == _sortColumn ? (_sortAscending ? " ▲" : " ▼") : string.Empty);
                DrawText(drawingContext, header, x, _verticalOffset, width, HeaderHeight, headerTypeface, textBrush, dpi);
            }

            var elapsed = Stopwatch.GetElapsedTime(started);
            FrameRendered?.Invoke(this, new MaterialsPrototypeRenderEventArgs(
                Math.Max(0, lastRow - firstRow),
                Math.Max(0, lastColumn - firstColumn + 1),
                drawnCells,
                _horizontalOffset,
                elapsed.TotalMilliseconds));
        }

        private int FindFirstVisibleColumn(double offset)
        {
            for (var index = 0; index < _columns.Count; index++)
            {
                if (_columnOffsets[index + 1] >= offset) return index;
            }
            return Math.Max(0, _columns.Count - 1);
        }

        private int FindColumnAt(double x)
        {
            for (var index = 0; index < _columns.Count; index++)
            {
                if (x >= _columnOffsets[index] && x < _columnOffsets[index + 1]) return index;
            }
            return -1;
        }

        private int FindHeaderSeparator(double x)
        {
            for (var index = 0; index < _columns.Count; index++)
            {
                if (Math.Abs(x - _columnOffsets[index + 1]) <= 5) return index;
            }
            return -1;
        }

        private int FindLastVisibleColumn(double offset)
        {
            for (var index = FindFirstVisibleColumn(_horizontalOffset); index < _columns.Count; index++)
            {
                if (_columnOffsets[index] > offset) return Math.Max(0, index - 1);
            }
            return Math.Max(0, _columns.Count - 1);
        }

        private static void DrawText(
            DrawingContext drawingContext,
            string value,
            double x,
            double y,
            double width,
            double height,
            Typeface typeface,
            Brush brush,
            double pixelsPerDip)
        {
            var safePixelsPerDip = double.IsFinite(pixelsPerDip) && pixelsPerDip > 0
                ? pixelsPerDip
                : 1d;
            var safeWidth = double.IsFinite(width) && width > 0 ? width : 1d;
            var safeHeight = double.IsFinite(height) && height > 0 ? height : RowHeight;
            var text = new FormattedText(
                value ?? string.Empty,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                12,
                brush,
                safePixelsPerDip)
            {
                MaxTextWidth = Math.Max(1, safeWidth - CellPadding * 2),
                MaxTextHeight = Math.Max(1, safeHeight - 4),
                Trimming = TextTrimming.CharacterEllipsis
            };
            var safeX = double.IsFinite(x) ? x : 0;
            var safeY = double.IsFinite(y) ? y : 0;
            drawingContext.DrawText(text, new Point(safeX + CellPadding, safeY + Math.Max(2, (safeHeight - text.Height) / 2)));
        }

        private static void DrawCheckBox(
            DrawingContext drawingContext,
            bool isChecked,
            double x,
            double y,
            double width,
            double height,
            Brush foreground)
        {
            var bounds = new Rect(
                x + Math.Max(3, (width - 14) / 2),
                y + Math.Max(3, (height - 14) / 2),
                14,
                14);
            drawingContext.DrawRectangle(Brushes.White, new Pen(new SolidColorBrush(Color.FromRgb(100, 116, 139)), 1), bounds);
            if (!isChecked) return;

            var geometry = new StreamGeometry();
            using (var context = geometry.Open())
            {
                context.BeginFigure(new Point(bounds.X + 3, bounds.Y + 7), false, false);
                context.LineTo(new Point(bounds.X + 6, bounds.Y + 10), true, false);
                context.LineTo(new Point(bounds.X + 11, bounds.Y + 4), true, false);
            }
            geometry.Freeze();
            drawingContext.DrawGeometry(null, new Pen(foreground, 1.7), geometry);
        }
    }

    private sealed record MaterialsPrototypeRenderEventArgs(
        int VisibleRows,
        int VisibleColumns,
        int DrawnCells,
        double HorizontalOffset,
        double RenderMilliseconds);

    private sealed record MaterialsPrototypeCellEventArgs(
        int RowIndex,
        int ColumnIndex,
        Rect CellBounds,
        bool CheckBoxHit);
}
