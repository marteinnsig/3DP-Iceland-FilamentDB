namespace FilamentDbApp;

internal sealed record HelpSection(
    string Id,
    string Category,
    string Title,
    string Summary,
    string Body,
    params string[] Keywords);

internal static class HelpContentCatalog
{
    internal const string StartHereId = "start-here";

    internal static IReadOnlyList<HelpSection> Sections { get; } =
    [
        new(
            StartHereId,
            "Start here",
            "Start-to-finish workflow",
            "The safe order for moving work from a purchase to verified engineering output.",
            """
            1. Create the Purchase Order
            Click New Order. The Draft order is saved immediately and later grid edits auto-save. Enter supplier, order number/date,
            currency, tax treatment, charges and allocation method. ECB is an optional reference for a session-new order only; review
            the displayed rate and provenance before calculating costs or receiving. Later ECB refreshes never rewrite a saved order
            rate or created Inventory, Usage or quote history.

            2. Add and cost every ordered item
            Click Add Ordered Item for each invoice line. Set description, Category, expected quantity, unit price, discount, unit
            weight and allocation inputs. Optionally link an existing Material. Click Calculate Landed Costs after committing edits.
            This validates and saves allocations and synchronizes current pricing/provenance to linked Materials; it is not a read-only
            preview. Resolve the Cost Allocation Validation message before continuing.

            3. Record physical receipt
            When the shipment arrives, click Receive / Reconcile. Review Received quantity and Check status for every line; correct
            missing, damaged or extra items. This action records receiving state but does not create Inventory.

            4. Create Materials and received spools
            Click Create Materials + Received Spools. Only Filament lines with positive Received quantity participate. Linked materials
            are reused; otherwise the app can create a draft Material and one Unopened Inventory spool per received unit. Other
            categories remain recorded on the Purchase Order. The action is repeat-safe and only fills a missing spool deficit.

            5. Govern material identity
            Review every created or linked row in Materials. Auto-created rows contain draft supplier/base-material defaults, not a
            guaranteed canonical identity. Complete the Material, use exact Manufacturer/Base Material binding where appropriate, set
            printing/profile/link data and save before testing. MaterialID is the stable downstream join key.

            6. Review Inventory and optionally record Usage
            Confirm spool MaterialID, quantity, weight, remaining weight, storage and retained purchase evidence in Inventory.
            Inventory owns the Materials quantity projection. Usage is optional private evidence: select the active MaterialID and,
            optionally, a matching spool; record fixed grams, minutes and counts. Corrections append reversal/replacement events.

            7. Enter native measurements
            Materials search and filters control which MaterialID rows appear in Tensile, Impact and Stiffness; there is no separate
            measurement MaterialID selector. Enter only editable raw inputs. Committing a valid edit recalculates read-only outputs,
            auto-saves SQLite and assigns today as measured date only when the first raw input has no existing date.

            8. Use Experimental Testing only when needed
            Experimental Testing is a separate optional series/run workflow. Create a material-linked Series, add controlled Runs and
            enter its five canonical measurement rows. Active/history controls change visibility or comparison scope without deleting
            history. Website publication is an explicit readiness-confirmed choice for active Series.

            9. Review engineering results in the intended scope
            Review Material Detail plus Rankings, Category Rankings, Awards and Dashboard Insights. These consume canonical calculated
            results and never replace source measurements. Materials search/filter scope also narrows comparison dashboards; clear
            filters before interpreting a whole-database comparison.

            10. Select public boundaries and build reports
            In Materials, save the intended Publish public reports choices. Publish public test details is a separate raw-detail
            permission, and Experimental Website selection is separate again. In Reports / PDF Export, preview/export individual
            reports as needed, then use Build Public Report Package for the canonical website handoff. Inspect representative HTML/PDF
            manually; building locally uploads nothing.

            11. Generate and inspect Website Preview
            In Website Export, confirm the active SQLite template and root folder, then click Generate Preview. The main Website DATA
            contains every active, non-archived MaterialID; Publish public reports controls linked report artifacts. Inspect
            index-test.html, navigation, experimental content, links and representative reports. Preview never changes Production.

            12. Verify and stop on failure
            Open Help > Verification Center and click Refresh after final changes. Require Full Data Verification PASS and Website
            READY FOR PUBLISH, then export the Verification report. Recalculate Native Results is a separate mutating repair action,
            not a routine read-only refresh. If any required gate fails, stop, fix the named source and repeat Preview plus Verification.

            13. Publish only with explicit authority
            An optional Publish Website Test is isolated below /preview/ and is not Production evidence. Generate Production requires a
            default-No confirmation and creates local backups plus a verified publish plan. Publish Website Production regenerates
            Production, then requires a second default-No live FTPS confirmation. Retain the manifest, publish plan, Verification
            export and completion log, and independently inspect the live site. Deep failure and recovery guidance is in Safety and
            support.
            """,
            "workflow", "landed costs", "receive reconcile", "inventory", "material", "measurements", "verification",
            "ready for publish", "production", "FTPS"),
        new(
            "materials.overview",
            "Materials and catalogs",
            "Materials reference",
            "Canonical MaterialID records, fields, filters, lifecycle, validation and publication boundaries.",
            """
            Purpose and ownership
            Materials is the canonical SQLite source for MaterialID identity. Measurements, Inventory, Material Detail, analysis,
            reports and website output join through MaterialID. Archived rows retain history but leave active output scope.

            Commands and persistence
            Add creates a backup, generates a MaterialID and starts a default draft row. Duplicate copies product/pricing/profile facts
            but clears review URL and resets test, video and archive state. Archive and Unarchive are reversible and auto-save after a
            backup. Delete is permanent, automation-blocked, default-No and removes associated measurement references after backup.
            Reset Columns changes only local layout. Manual Backup creates evidence; it is not a Save button. Normal grid edits auto-save.

            Search and filters
            Search plus Manufacturer, Material type, Category, Reinforcement and Test status control the visible workflow. Clear Filters
            restores full active scope. Filters also affect native measurement visibility and global analysis/report Visible scope; they
            never delete records.

            Fields and states
            Edit governed identity/catalog values, public choices, purchase/spool facts, MSRP/landed evidence, media, notes and archive
            state. MaterialID, derived category/test/video flags, normalized USD/per-kg values, sort/source priority, Website Display
            Name, Material Key and Validation are read-only/computed. Required identity includes MaterialID, Manufacturer, Product Line,
            Base Material and Website Display Name.

            Publication boundaries
            Publish public reports controls linked public report artifacts. Publish public test details separately permits raw test
            detail in eligible reports. Main Website DATA uses every active non-archived MaterialID. Save the intended choices before
            building reports or Preview.
            """,
            "materialid", "manual backup", "add material", "duplicate", "archive", "unarchive", "delete material",
            "reset columns", "search", "filters", "auto-save", "validation", "public reports", "public test details"),
        new(
            "manufacturers.overview",
            "Materials and catalogs",
            "Manufacturers reference",
            "Governed manufacturer records, exact binding, rename propagation and relationship-safe lifecycle.",
            """
            Commands and filters
            Add and Duplicate create unique draft records and save immediately. Search narrows the grid; Show archived includes inactive
            records. Archive / Restore changes activity without deleting history. Delete requires confirmation and backup and is blocked
            while canonical Materials reference the ManufacturerID.

            Fields and auto-save
            Name, Display Name, Country, Founded, Website, Logo URL, Description, Engineering Focus, Material Categories, Strengths,
            Weaknesses, Sustainability, Typical Applications, Headquarters, Notes, Sort and Active are editable. Property changes
            auto-save. A committed canonical rename propagates display-name snapshots to linked Materials; draft-name edits do not
            prematurely propagate.

            Bind Exact Material Names
            This command previews only unique exact legacy-name matches and requires explicit confirmation before creating
            ManufacturerID relationships. Near matches and ambiguous names remain unbound for owner review.
            """,
            "manufacturer", "add", "duplicate", "bind exact", "archive", "restore", "delete", "show archived",
            "rename", "auto-save", "relationship"),
        new(
            "purchase-orders.overview",
            "Purchasing and cost",
            "Purchase Orders reference",
            "Order headers, items, exchange-rate provenance, landed costs, receiving and Inventory handoff.",
            """
            Order commands and save mode
            New Order immediately persists a Draft. Delete Order requires confirmation and retains already-created Inventory spools.
            Add Ordered Item immediately adds a default Filament line; Delete Item removes the selected line but not independent spools.
            Header and line grid edits auto-save after commit.

            Order header fields
            PO ID is read-only. Edit Supplier, Order number/date, Workflow stage, Received date, Currency, Exchange rate, Tax treatment,
            supplier totals/shipping/allocation/VAT/invoice, import VAT, customs, clearance, other fees and Notes. Rate source/date,
            Cost status and Document are read-only. Stages are Draft, Ordered, Awaiting import charges, Awaiting delivery, Receiving,
            Verified, Inventory created and Complete.

            Ordered-item fields
            Line ID is read-only. Edit description, Category, SKU, Expected/Received quantity, Check state, optional Material link,
            storage, unit price, discount, unit weight, allocation toggle/manual shipping and Notes. Net/allocated/landed values and
            Allocation status are calculated. Categories are Filament, Printer, Equipment, Spare Parts, Consumables and Other.

            ECB and landed costs
            Refresh ECB Reference is optional and offline-safe. Only orders created in the current session may receive reference prefill;
            saved rates/provenance never refresh. Stale cache is labeled and governed Settings remain fallback. Calculate Landed Costs
            commits edits, validates and saves allocations and synchronizes current linked-Material pricing evidence. Resolve the Cost
            Allocation Validation message before receiving.

            Receiving and downstream creation
            Receive / Reconcile records counts, Check state, stage and received date; it does not create Inventory. Create Material from
            Selected Item requires a described Filament line and creates/links a draft requiring later identity review. Create Materials
            + Received Spools processes only positively received Filament lines, creates/reuses Materials and adds one Unopened spool per
            unit; it is repeat-safe. Other categories remain PO-only. Attach Document is optional and copies the file into governed
            storage.
            """,
            "new order", "delete order", "add ordered item", "delete item", "currency", "ECB", "landed costs",
            "receive reconcile", "create materials received spools", "attachment", "auto-save", "workflow stage"),
        new(
            "inventory.overview",
            "Purchasing and cost",
            "Inventory reference",
            "Editable spool records, filters, calculated summaries, validation and Usage handoff.",
            """
            Record model and commands
            One row is one spool or homogeneous group. Add Spool uses Material defaults only for the first spool. Duplicate explicitly
            copies the selected inventory record. Delete requires confirmation and removes the spool, not its Material. Refresh
            Inventory commits edits, recalculates summaries and synchronizes the Materials quantity projection.

            Filters and summaries
            Search covers material, spool ID, supplier, storage, batch and order. Show Empty defaults on; Only Opened, Low Stock under
            20 percent and Show Archived Materials narrow scope. Clear Filters restores the normal view. Summary cards show total,
            unopened/opened/empty, remaining, estimated value, average cost/kg and review count.

            Editable and calculated fields
            Spool ID is read-only. Edit Material, Status, Qty, Spool g, Remaining g/spool, Storage, Batch, Price/spool, Currency,
            Supplier, Purchase date, Order and Notes. Status is Unopened, Opened or Empty and controls row color. The calculated grid
            shows remaining percent, estimated value, cost/kg and Validation/Review.

            Save, validation and handoff
            Add, Duplicate and committed cell edits persist. Validation reviews quantity, weights, remaining range, storage and cost
            evidence; OK means no issues. Purchase receiving can create spool rows. Usage can select a same-MaterialID spool and
            atomically reduce remaining weight.
            """,
            "inventory", "add spool", "duplicate", "delete", "refresh inventory", "show empty", "only opened",
            "low stock", "archived", "remaining", "estimated value", "validation", "usage"),
        new(
            "usage.overview",
            "Purchasing and cost",
            "Usage reference",
            "Accepted usage events, Inventory-linked consumption, totals and append-only corrections.",
            """
            Record Usage
            Select an active MaterialID and optionally a matching Inventory spool. Enter event Type, Occurred UTC, fixed Filament g,
            Provenance, Print minutes, Hands-on minutes, Produced/Accepted/Rejected counts, Source and Note. Inputs do not auto-save;
            Record Usage validates and commits one explicit event.

            Inventory and validation
            A linked spool must belong to the same MaterialID and have sufficient remaining weight. Recording occurs in the same SQLite
            transaction as the spool decrement. MaterialID, occurrence, amounts, counts and provenance are validated.

            Ledger and totals
            Effective events, ledger rows, net filament, print/hands-on time, part counts and evidence coverage are read-only summaries.
            The ledger retains timestamp, MaterialID, Type, Entry kind, spool, quantities, durations, counts, source, note and Event ID.

            Corrections
            Correct Selected prepares a replacement but never edits the accepted row. Commit appends an exact reversal and replacement
            transaction; MaterialID cannot change and a reversal cannot be reversed again. Cancel Correction abandons the draft only.
            """,
            "usage", "record usage", "correct selected", "cancel correction", "filament grams", "provenance",
            "inventory spool", "ledger", "accepted event", "reversal", "replacement"),
        new(
            "printers.overview",
            "Costing and quoting",
            "Printers reference",
            "Governed machine-cost fields, validation, lifecycle and prospective quote-rate handoff.",
            """
            Commands and save behavior
            Add Printer, Duplicate and Archive / Restore persist immediately. Cell edits save after commit; Save performs explicit
            validation/persistence. Delete is default-No and can be blocked by saved quote references, restoring the row on failure.

            Fields and validation
            Printer ID is read-only. Edit Name, Manufacturer, Model, Currency, Purchase cost, Upfront cost, Annual maintenance,
            estimated Life years, Uptime percent, Power watts, Buffer override, Active and Notes. Name and governed rate inputs must be
            valid; Uptime is 0 through 100. The status text reports the resulting hourly-rate readiness.

            Quote handoff
            Active Printer plus prospective Settings inputs produces hourly cost evidence for a new quote draft. Later Printer/Settings
            changes do not automatically update an existing saved quote. Obsolete or test quotes can be explicitly deleted from history.
            """,
            "printer", "add printer", "duplicate", "archive restore", "delete", "save", "currency", "uptime",
            "power", "buffer", "hourly rate", "quote"),
        new(
            "print-job-quotes.overview",
            "Costing and quoting",
            "Print Job Quotes reference",
            "Prospective calculation inputs, saved quote history, customer PDF and permanent deletion.",
            """
            Customer and material inputs
            Enter Customer, Description, Prepared by and Quote currency. Choose a canonical Material or enable explicit manual cost/kg
            with its currency. Enter grams per part and quantity. The evidence text explains the selected pricing provenance.

            Printer, time and price inputs
            Choose an active Printer and enter Print hours, print/post-processing labor, consulting, design/change time, additional ISK
            charges and target margin. The calculation summary updates prospectively while editing; draft inputs are not history.

            Saved quote history
            Save Quote validates the complete draft and writes calculation, Material, Printer, rates, Settings and evidence snapshots.
            Later Settings, Material, Inventory, Printer or exchange-rate changes do not automatically recalculate a saved row. History
            is read-only in this view and shows quote number, UTC creation, customer, Material/Printer snapshots, final price and currency.

            PDF and deletion
            Export Selected PDF requires a saved history selection and renders the customer-safe saved snapshot. Delete Selected is a
            permanent default-No removal of the quote and its snapshot; it is not archive.
            """,
            "quote", "customer", "prepared by", "currency", "manual cost", "material", "printer", "labor",
            "margin", "save quote", "history", "PDF", "saved snapshot", "delete"),
        new(
            "base-materials.overview",
            "Materials and catalogs",
            "Base Materials reference",
            "Canonical material families, controlled test-print profiles, exact binding and relationship-safe deletion.",
            """
            Fields
            Edit Base Material, Category, Sort Order; minimum/recommended/maximum nozzle, bed, speed and cooling values; cooling
            guidance; drying temperature/hours; enclosure; printer/G-code and slicer references; Profile ID and Profile kind.

            Commands and persistence
            Add Base Material and Duplicate persist immediately. Grid value edits auto-save; names must be non-empty and unique.
            Canonical rename propagates snapshots to linked Materials. Reset Columns requires confirmation and changes layout only.

            Binding and deletion
            Bind Exact Material Names previews unique exact unlinked names and requires explicit confirmation before adding
            BaseMaterialID relationships. Delete is automation-blocked and refused while any Material references the ID; an unreferenced
            deletion is immediate, so verify the selected row first.

            Downstream meaning
            Profiles are controlled 3DPIceland test-print baselines consumed by Material Detail and testing workflow, not manufacturer
            recommendations.
            """,
            "base material", "add", "duplicate", "delete", "reset columns", "bind exact", "nozzle", "bed",
            "speed", "cooling", "drying", "profile", "relationship"),
        new(
            "settings.overview",
            "Configuration",
            "Settings Manager reference",
            "Governed measurement, calculation, currency, purchasing and deployment settings with prospective ownership.",
            """
            Grid and sections
            Section, Parameter, Unit, Used By and Notes are read-only; Value is editable. Sections include measurement/calculation
            constants, multi-currency purchasing reference values and Deployment/FTPS settings. Base Material Catalog has separate
            ownership even though it participates in the save workflow.

            Commands
            Save Settings writes General and Deployment settings and preserves the separately governed Base Material Catalog. Reload
            Saved Settings is default-No and discards unsaved General/Deployment edits without changing Base Materials. Restore
            Built-in Defaults is default-No and replaces/saves General rows only; Deployment and Base Materials remain. Reset Columns
            changes layout only.

            Prospective boundary
            Settings feed ResultsService calculations, offline purchasing currency fallback, Printer rates, quote drafts and guarded
            deployment. Changes apply prospectively and may refresh current calculated views/drafts. They never rewrite saved Purchase
            Order rates, received Inventory provenance, Usage history or saved quote snapshots. ECB remains an optional reference for
            session-new Purchase Orders, not the Settings owner.
            """,
            "settings", "value", "measurement", "calculation", "currency", "purchasing", "deployment", "FTPS",
            "save settings", "reload", "restore defaults", "reset columns", "prospective"),
        new(
            "measurements",
            "Testing and engineering",
            "Tensile, Impact and Stiffness Measurements",
            "Enter source samples under the correct material and let canonical services calculate results.",
            """
            Complete the canonical Material first. Materials search/filter determines which MaterialID rows appear in all three native
            measurement grids; the test tabs have no separate MaterialID selector. Identity and computed fields are read-only.

            Tensile accepts up to ten Upright N and ten Flat N samples, each from 0 through less than 505. Impact accepts up to ten
            Upright and Flat percentage samples from 0 through 100. Stiffness accepts Revolutions from 0 through 10 and Degrees from
            0 through 359. Notes and measured date are editable.

            A valid committed raw edit recalculates, updates tested state and auto-saves SQLite. The first raw value assigns today only
            if measured date is empty. Fix Validation errors in source inputs; do not type MPa, kJ/m², deflection, modulus, standard
            deviation, CV, count or confidence manually because those outputs are calculated read-only.
            """,
            "tensile", "impact", "stiffness", "samples", "MPa", "validation", "calculation"),
        new(
            "experimental-testing",
            "Testing and engineering",
            "Experimental Testing",
            "Manage material-linked test series, controlled runs and publication readiness.",
            """
            Experimental Testing is separate from native measurement entry. A series defines the material, experiment and default unit;
            Add Series prefers the selected active Material and otherwise uses the first active Material. Runs hold controlled values
            and five canonical editors: Tensile Upright/Flat, Impact Upright/Flat and Stiffness. Valid edits auto-calculate and auto-save.

            Only one Run can be baseline per Series. Runs default Planned and Active.
            Active only filters series visibility. Include inactive history in comparison affects result comparison scope only when
            inactive completed history exists; it does not change the run grid or reactivate records.

            Website publication is a deliberate active-Series property. Clicking it evaluates readiness, defaults incomplete
            publication confirmation to No and persists the accepted choice immediately. Review the readiness message before export.
            """,
            "experimental", "series", "run", "active only", "inactive history", "website", "baseline"),
        new(
            "analysis",
            "Analysis and decisions",
            "Results, Rankings, Awards and Insights",
            "Interpret canonical outputs without changing their source data.",
            """
            Material Detail and Experimental Results provide the closest review of one material or test series. Rankings Dashboard and
            Category Rankings compare accepted calculated metrics across their displayed scope. Awards & Winners applies the governed
            award rules. Dashboard Insights summarizes broader patterns.

            Materials search and filters define the comparison scope inherited by Rankings, Category Rankings, Awards and Insights.
            Rows without a score for the chosen ranking metric are omitted. Filters never change stored measurements; clear them before
            interpreting a whole-database result. Experimental results remain separately scoped to the selected Series and Runs.
            """,
            "results", "rankings", "category", "awards", "dashboard", "comparison", "filters"),
        new(
            "measurements.tensile", "Testing and engineering", "Tensile Measurements reference",
            "Upright/Flat force samples, calculated MPa, validation, date ownership and auto-save.",
            """
            Materials search/filter determines visible MaterialID rows; there is no separate selector. Enter up to ten Upright N and ten
            Flat N samples from 0 through less than 505. Notes and measured date are editable. Identity, MPa, standard deviation, CV,
            count, confidence and Validation are read-only.

            A valid committed sample invokes ResultsService with the governed tensile cross-section Setting, updates tested state and
            auto-saves SQLite. The first sample assigns today only when measured date is empty. Invalid input is rejected. Reset Columns
            changes layout only. Correct raw samples here rather than calculated fields.
            """, "tensile", "upright", "flat", "MPa", "CV", "confidence", "auto-save"),
        new(
            "measurements.impact", "Testing and engineering", "Impact Measurements reference",
            "Upright/Flat percentages, calculated energy, governed Settings, validation and auto-save.",
            """
            Materials search/filter owns visible MaterialID rows. Enter up to ten Upright and ten Flat needle percentages from 0 through
            100. Notes and measured date are editable. Identity, kJ/m², standard deviation, CV, count, confidence and Validation are
            read-only.

            A valid commit invokes ResultsService with governed impact Settings and auto-saves SQLite. The first input assigns today only
            if the date is blank. Invalid values remain errors. Reset Columns changes layout only; correct source percentages or Settings
            at their owner instead of calculated energy.
            """, "impact", "percentage", "kJ/m²", "Settings", "CV", "validation", "auto-save"),
        new(
            "measurements.stiffness", "Testing and engineering", "Stiffness Measurements reference",
            "Revolutions/degrees inputs, calculated deflection/modulus, validation and auto-save.",
            """
            Materials search/filter owns visible MaterialID rows. Enter Revolutions from 0 through 10 and Degrees from 0 through 359,
            plus optional Notes/date. Identity, Deflection mm, Modulus MPa and Validation are read-only.

            A valid commit invokes ResultsService with governed stiffness Settings and auto-saves SQLite. The first source input assigns
            today only when the date is empty. Invalid values are rejected. Reset Columns changes layout only. Revolutions and Degrees
            form one observation; never edit Deflection or Modulus directly.
            """, "stiffness", "revolutions", "degrees", "deflection", "modulus", "auto-save"),
        new(
            "experimental.series", "Experimental testing", "Experimental Series reference",
            "Material-linked definitions, filters, publication readiness and governed lifecycle.",
            """
            Add Series prefers the selected active Material; Duplicate creates a new identity and resets publication state. Delete
            confirms and removes the Series graph. Find, Active only and Clear Filters affect visibility only; Active only defaults on.

            Series ID is read-only. Material, Experiment, Default Unit, Baseline Material, Website, Active and Notes save after commit.
            Website is separate from Material report permissions. Enabling it checks readiness, defaults incomplete confirmation to No
            and persists only an accepted choice; hiding needs no confirmation.

            Result views
            Experimental Dashboard summarizes completeness and best results. Experimental Table lists comparable Runs and deltas.
            Experimental Charts visualizes metrics and baseline-normalized results.
            """, "series", "add", "duplicate", "delete", "active only", "website", "readiness"),
        new(
            "experimental.runs", "Experimental testing", "Experimental Runs reference",
            "Controlled values, lifecycle, baseline uniqueness, active history and persistence.",
            """
            Add Run creates a Planned active row. Duplicate creates a clean Planned, non-baseline Run without a measured date. Delete
            confirms and removes the Run graph. Value, Unit, Status, date, Baseline, Active and Notes save after commit.

            Only one Run per Series can be Baseline. Inactive Runs retain history. Include inactive history changes Results comparison
            scope only when eligible completed history exists; it never changes the grid, reactivates or publishes a Run.
            """, "runs", "planned", "baseline", "active", "inactive history"),
        new(
            "experimental.measurements", "Experimental testing", "Experimental measurement editors reference",
            "Run-scoped Tensile, Impact and Stiffness source rows with canonical calculation.",
            """
            Editors belong to the selected Run, not native Material rows. Enter only bounded Tensile force, Impact percentage,
            Stiffness Revolutions/Degrees, dates and Notes.

            MPa, kJ/m², Deflection, Modulus, deviation, CV, counts, confidence and Validation are ResultsService outputs. Valid commits
            auto-save the graph and refresh Results. Invalid or missing coverage remains incomplete regardless of Run status.
            """, "experimental measurement", "tensile", "impact", "stiffness", "auto-save"),
        new(
            "experimental.measurements.tensile", "Experimental testing", "Experimental Tensile editor reference",
            "Run-scoped Upright/Flat force samples and calculated tensile outputs.",
            """
            This editor belongs to the selected Experimental Run. Enter up to ten Upright and ten Flat force samples in N, plus the
            measured date and Notes. Material/Run identity, Average MPa, standard deviation, CV, count, confidence and Validation are
            calculated or read-only.

            Valid committed source samples auto-save the Series graph and refresh Results through the same governed ResultsService
            formulas used by native measurements. Invalid or incomplete values remain visible as validation/readiness gaps. Editing
            this Run never changes native Material tensile rows.
            """,
            "experimental tensile", "upright", "flat", "force", "MPa", "auto-save"),
        new(
            "experimental.measurements.impact", "Experimental testing", "Experimental Impact editor reference",
            "Run-scoped Upright/Flat percentage samples and calculated impact outputs.",
            """
            This editor belongs to the selected Experimental Run. Enter up to ten Upright and ten Flat needle-percentage samples, plus
            the measured date and Notes. Material/Run identity, Average kJ/mÂ², standard deviation, CV, count, confidence and Validation
            are calculated or read-only.

            Valid commits auto-save the Series graph and refresh Results through governed impact Settings and ResultsService. Values
            outside the accepted percentage range remain errors and incomplete coverage cannot be repaired by changing Run status.
            Experimental inputs do not overwrite native Material impact measurements.
            """,
            "experimental impact", "percentage", "kJ/mÂ²", "validation", "auto-save"),
        new(
            "experimental.measurements.stiffness", "Experimental testing", "Experimental Stiffness editor reference",
            "Run-scoped revolutions/degrees inputs and calculated stiffness outputs.",
            """
            This editor belongs to the selected Experimental Run. Enter Revolutions and Degrees as the paired source observation, plus
            measured date and Notes. Material/Run identity, Deflection mm, Modulus MPa and Validation are calculated or read-only.

            A valid commit auto-saves the Series graph and refreshes Results through governed stiffness Settings and ResultsService.
            Correct Revolutions or Degrees rather than calculated output. Experimental values remain separate from the selected
            Material's native Stiffness Measurements rows.
            """,
            "experimental stiffness", "revolutions", "degrees", "deflection", "modulus", "auto-save"),
        new(
            "experimental.results", "Experimental testing", "Experimental Results reference",
            "Choose Dashboard, Table or Charts for the selected-Series comparison.",
            """
            Results is scoped to the selected Experimental Series and the current active/history comparison choice. Dashboard summarizes
            readiness, baseline, best metrics and recommendation. Table exposes comparable Run rows, rank, deltas and variation. Charts
            plot metrics and baseline-normalized values.

            All three views are read-only projections of saved Run inputs and canonical calculations. If the Series, eligible Runs or
            unique Baseline is missing, Results reports the limitation instead of creating values. Correct ownership in Series, Runs or
            the measurement editors, then return to the appropriate Results view.
            """,
            "experimental results", "dashboard", "table", "charts", "selected Series", "read-only"),
        new(
            "experimental.results.dashboard", "Experimental testing", "Experimental Dashboard reference",
            "Selected-Series completeness, baseline, best results, recommendation and history scope.",
            """
            Scope is the selected Series plus active/history comparison choice. Cards show Run/Completed counts, missing results,
            Baseline, quality, highest CV, best metrics, best overall and recommended setting.

            These are comparative summaries, not measurements. Missing/high-variation evidence stays visible; without eligible Runs,
            the dashboard reports unavailable rather than inventing values.
            """, "dashboard", "recommended", "quality", "highest CV", "baseline"),
        new(
            "experimental.results.table", "Experimental testing", "Experimental Table reference",
            "Read-only ranked Run metrics, delta-to-baseline, variation and baseline highlighting.",
            """
            The table ranks eligible selected-Series Runs and shows identity, controlled value/unit, lifecycle, date, calculated metrics,
            overall score, delta to baseline, CV and Baseline. Rows are read-only.

            Baseline highlighting identifies the comparison reference. Without a valid baseline, delta is unavailable; correct baseline
            ownership in the Run grid.
            """, "results table", "rank", "delta", "CV", "baseline", "read-only"),
        new(
            "experimental.results.charts", "Experimental testing", "Experimental Charts reference",
            "Metric charts and baseline-normalized comparison for the selected Series.",
            """
            Charts plot eligible selected-Series Runs for Tensile, Impact, Stiffness and overall score. Baseline normalization requires
            one valid Baseline Run. Charts refresh from ResultsService outputs and are read-only; they never save or alter lifecycle.
            """, "charts", "tensile", "impact", "stiffness", "baseline normalized"),
        new(
            "material-detail.general", "Material detail", "Material Detail — General reference",
            "Selected MaterialID identity and dynamically grouped read-only fields.",
            """
            Material Detail follows the selected Materials row and repeats its MaterialID. General groups available identity, catalog,
            pricing, Inventory, media and governance fields. Edit values at Materials, Manufacturers, Base Materials, Purchasing or
            Inventory; this projection is read-only.
            """, "material detail", "general", "MaterialID", "grouped", "read-only"),
        new(
            "material-detail.printing-profile", "Material detail", "Material Detail — Printing Profile reference",
            "Controlled Base Material test-print/G-code baseline and Not recorded meaning.",
            """
            The profile resolves through the Base Material relationship and shows the controlled 3DPIceland test-print/G-code baseline.
            It is not a manufacturer recommendation and publishes nothing. Not recorded means the catalog has no value; edit Base
            Materials rather than this read-only view.
            """, "printing profile", "base material", "G-code", "not recorded"),
        new(
            "material-detail.mechanical", "Material detail", "Material Detail — Mechanical reference",
            "Canonical test status, calculated properties, reliability and expanded evidence.",
            """
            Mechanical shows identity/status plus canonical Tensile, Impact and Stiffness outputs. Orientation metrics, consistency,
            counts and reliability come from native measurements and ResultsService. Expand Canonical mechanical data for source rows.
            Correct raw inputs on measurement tabs; this view never edits them.
            """, "mechanical", "tensile", "impact", "stiffness", "reliability"),
        new(
            "material-detail.charts", "Material detail", "Material Detail — Charts reference",
            "Five normalized engineering axes and the limits of the overall profile.",
            """
            Charts shows normalized 0–100 Tensile, Impact, Stiffness, Consistency and Layer Adhesion. Overall averages available radar
            axes. This is comparative 3DPIceland chart input, not a certified scientific rating; missing results remain unavailable.
            """, "charts", "0-100", "consistency", "layer adhesion", "scientific rating"),
        new(
            "material-detail.analytics", "Material detail", "Material Detail — Analytics reference",
            "Visible-scope grouping, multi-select radar overlay and selection controls.",
            """
            Chart Mode groups visible Materials into radar rows. Select one or Ctrl-click multiple rows; Clear radar selection removes
            local selection only. Materials search/filters define scope. Analytics uses the five normalized axes and changes no data.
            """, "analytics", "chart mode", "radar", "multi-select", "clear"),
        new(
            "material-detail.compare", "Material detail", "Material Detail — Compare reference",
            "A–D Material selectors, selected-Material handoff, winners and deltas.",
            """
            Choose up to four canonical Materials in A–D. Use Selected copies the current identity into a slot without changing source
            data. Winners/deltas appear only for comparable canonical results; missing results stay missing and the view is read-only.
            """, "compare", "A-D", "use selected", "winner", "delta"),
        new(
            "material-detail.video-planner", "Material detail", "Material Detail — Video Planner reference",
            "Local creator-planning filters, idea lifecycle, dashboard and prompt handoff.",
            """
            Video Planner derives candidates from visible Materials. Filters, Refresh and Clear affect planning only. Idea actions own
            separate local records. Copy prompt writes owner-review text to the clipboard; it calls no external service and publishes
            nothing.
            """, "video planner", "filters", "refresh", "ideas", "copy prompt"),
        new(
            "material-detail.recommendations", "Material detail", "Material Detail — Recommendations reference",
            "Evidence, alternatives, cautions, prompt and Video Planner handoff.",
            """
            Recommendations project verified results in visible scope. Filters/Refresh choose guidance; details expose evidence,
            alternatives and cautions. Copy prompt is local; Send to Video Planner transfers planning context only. Neither action edits
            measurements, certifies suitability or publishes.
            """, "recommendations", "evidence", "alternatives", "cautions", "video planner"),
        new(
            "material-detail.notes", "Material detail", "Material Detail — Notes reference",
            "Current placeholder boundary for a future dedicated Detail notes workspace.",
            """
            A dedicated Detail Notes workspace is not implemented. The application is not read-only: governed edits remain at owning
            tabs, including Materials Notes. Do not treat this placeholder as evidence that notes are absent from SQLite.
            """, "notes", "placeholder", "materials notes"),
        new(
            "analysis.rankings", "Analysis and decisions", "Rankings Dashboard reference",
            "Visible-scope metric ranking, filters, Top 25 default, refresh and CSV.",
            """
            Choose Overall, Tensile, Impact, Stiffness, Consistency or Layer Adhesion plus optional Base Material, Manufacturer and
            Reinforcement filters. Rows defaults to Top 25; Top 10/50/100 and All ranked are available. Missing scores are omitted.

            Reset restores defaults, Refresh rebuilds read-only rows and Export CSV writes displayed scope. Clear Materials filters
            before whole-database interpretation.
            """, "rankings", "top 25", "filters", "refresh", "CSV"),
        new(
            "analysis.category-rankings", "Analysis and decisions", "Category Rankings reference",
            "Winner-focused grouped rankings with 10 rows per group by default.",
            """
            Choose a performance Category and Overall/Base Material/Manufacturer grouping, then optional scope filters. Rows per group
            defaults to 10; 5, 50, 100 and All are available. Reset, Refresh and Export CSV operate on this read-only projection.
            """, "category rankings", "winner", "rows per group", "10", "CSV"),
        new(
            "analysis.awards", "Analysis and decisions", "Awards & Winners reference",
            "Governed award sets, visible-scope filters, winner reasoning and CSV.",
            """
            Choose All, Performance, Material family or Reinforcement awards plus optional scope filters. Rows show Winner, score,
            Runner Up, Use Case, Why and Status. Reset/Refresh rebuild read-only awards and Export CSV writes them; this view does not
            publish.
            """, "awards", "winner", "runner up", "why", "CSV"),
        new(
            "analysis.dashboard-insights", "Analysis and decisions", "Dashboard Insights reference",
            "Current counts, highest verified metrics and read-only narrative insights.",
            """
            Insights summarizes tested Materials, Manufacturers, Material/Reinforcement Types and highest Overall, Tensile, Impact and
            Stiffness results. Narrative derives from canonical calculated results, edits nothing and has no separate Save.
            """, "dashboard insights", "counts", "highest overall", "read-only"),
        new(
            "reports-website",
            "Output and publishing",
            "Reports, Website Export and public publishing",
            "Generate from verified data, inspect previews and keep Production guarded.",
            """
            Reports / PDF Export builds governed report models from canonical summaries. Choose the report, material scope and output
            folder, then preview and visually inspect the HTML/PDF before external use.

            Individual report preview/export obeys the selected template and Selected/Visible scope. Public batch actions instead use
            their named family and opted-in MaterialIDs. Build Public Report Package is the canonical website handoff and automatically
            rebuilds missing/stale public artifacts. Building reports locally uploads nothing.

            Generate Preview builds index-test.html and automatically ensures the public report package. Main Website DATA uses every
            active, non-archived MaterialID; Publish public reports controls linked report artifacts, Publish public test details
            separately controls raw report detail, and Experimental Website selection is separate.

            After visual review, Refresh Verification and require PASS plus READY FOR PUBLISH. Publish Website Test is isolated from
            Production. Generate Production and live FTPS are separately guarded, default-No actions; Publish Website Production invokes
            generation first and therefore requires two confirmations. Retain manifests, publish plan, Verification and transfer logs.
            """,
            "report", "PDF", "website", "preview", "public", "Production", "FTPS", "allowlist"),
        new(
            "settings-tools",
            "Configuration",
            "Settings Manager, AI Assistant and YouTube Research",
            "Manage governed defaults and local creator-support workflows.",
            """
            Settings Manager owns governed application defaults and calculation inputs. Changes apply prospectively; they must not
            rewrite saved purchase, inventory or quote snapshots.

            AI Assistant is a local scoped analysis surface unless a separately governed external integration is enabled in a future
            release. Review its visible MaterialID scope before using generated briefs or collections.

            YouTube Research and video-planning tools support creator workflow. They do not alter engineering measurements or replace
            Verification.
            """,
            "settings", "currency", "AI Assistant", "collection", "YouTube", "video"),
        new(
            "reports.overview", "Output and publishing", "Reports / PDF Export reference",
            "Governed templates, scope, previews, exports, public builds and retained evidence.",
            """
            Choose one of the governed report templates, Selected or Visible Material scope and an output folder. Refresh Preview builds
            a read-only canonical model; preview/export actions write local artifacts only. Public builds use explicit publication
            choices rather than the current preview selection.

            Current report, Engineering Package and public-build actions have different output contracts. Inspect HTML/PDF visually and
            retain status/log evidence. Reports consume canonical results; they never repair or recalculate raw measurements.
            """, "reports", "PDF", "template", "scope", "preview", "export", "public builds"),
        new(
            "reports.current-report", "Reports", "Current report reference",
            "Preview and export one of the twelve governed report templates.",
            """
            Report template selects one of twelve governed report templates. The six engineering templates are Material Summary, Material Engineering,
            Comparison, Manufacturer, Test Session and Printing Recommendation. The six purchasing templates are Inventory, Purchase,
            Supplier, Low Stock, Inventory Verification and Purchasing Intelligence. A separator in the list only distinguishes the
            families; it does not change scope or output behavior.

            Selected Material Only uses the current MaterialID where that template supports material scope. All Visible Materials uses
            the current Materials search/filter projection, so inspect the selected-material/scope line and clear unintended filters.
            Refresh Preview rebuilds the read-only preview log from canonical SQLite-backed models. Export Current Report validates the
            same selection and writes the canonical HTML, PDF, text, metadata and manifest set to the chosen local output folder.

            Preview and export do not publish, upload, edit measurements or change saved source records. A generated PDF still needs
            visual inspection for pagination, clipping and customer-facing content before it is handed off externally.
            """,
            "current report", "twelve templates", "Material Summary", "Inventory Report", "preview", "export"),
        new(
            "reports.engineering-package", "Reports", "Engineering Report Package reference",
            "Build the complete accepted engineering-report family as one local package.",
            """
            Export Engineering Package is separate from Export Current Report. It builds all six accepted engineering templates:
            Material Summary, Material Engineering, Comparison, Manufacturer, Test Session and Printing Recommendation. The selected
            purchasing template does not become part of this package.

            The action validates the canonical report set and creates an indexed local package containing HTML, PDF, text, metadata,
            manifests and required assets. The index and manifests describe the generated package; they are not proof that every page
            has been visually accepted.

            Review representative HTML and PDF files, confirm intended material scope, and retain the manifest and status evidence.
            The package action performs no FTPS transfer and does not alter source measurements, materials or saved report inputs.
            """,
            "engineering package", "six accepted engineering reports", "manifest", "metadata", "local package"),
        new(
            "reports.public-builds", "Reports", "Public report builds reference",
            "Build explicitly eligible public report families and the canonical public package.",
            """
            Each public button owns its own scope; Report template and Report scope do not control these batch actions. Build Public
            Material Reports uses active MaterialIDs opted into Public reports. Public Comparisons, Manufacturers, Test Sessions,
            Recommendations and Material Summary apply their named report-family rules and explicit MaterialID allowlists. Public Test
            Sessions exposes raw inputs or notes only when the separate Public test details approval permits them.

            Build Public Report Package ensures missing or stale eligible reports, verifies the six public report families and creates
            the canonical portfolio index, manifest and catalog. Main Website DATA is different: Website Export includes every active,
            non-archived MaterialID while the public-report flags govern linked report artifacts.

            These actions write local public artifacts only. They do not upload or make a Material public by themselves. Inspect the
            generated pages and exclusions before Website Preview or any separately guarded FTPS action.
            """,
            "public builds", "public reports", "public test details", "allowlist", "MaterialID", "portfolio catalog"),
        new("reports.scope-and-output", "Reports", "Report scope and output reference", "Selected/Visible scope and governed output-folder ownership.",
            """
            Selected Material Only resolves the current Materials selection to its stable MaterialID. If nothing valid is selected,
            a material-scoped report cannot safely proceed. All Visible Materials uses the current Materials search and filter
            projection, not automatically every row in SQLite; archived or filtered-out rows remain outside that projection.

            The chosen output folder is a local artifact destination shared by preview/export actions. Choose Folder changes that
            destination and Open Folder only opens it in Windows Explorer. Confirm the intended scope and output
            folder before generating, and clear filters when the real intent is the whole eligible database.

            Changing scope or folder does not modify Materials. Generated files can replace same-named artifacts in the selected
            destination, so keep accepted external copies in a governed release/evidence location.
            """,
            "selected", "visible", "output folder"),
        new(
            "reports.preview-evidence", "Reports", "Report preview and evidence reference",
            "Interpret the preview, status line, logs and generated artifact evidence.",
            """
            Refresh Preview rebuilds the chosen report model and shows a readable summary in Report Preview. The blue summary line
            identifies the latest result. Treat missing-data, scope or validation messages as blockers; do not export a different
            report and assume the requested one passed.

            After export, retain the report manifest, metadata, output paths and relevant Verification result. Open representative HTML
            and PDF artifacts and check headings, values, wrapping, page breaks, charts and intended public/customer-safe fields.
            Deterministic Verification can prove structure and data contracts, but visual acceptance remains manual.

            A successful local build does not prove Website generation, FTPS transfer or live-site correctness. Those stages have their
            own logs, confirmations and evidence in Website Export.
            """,
            "preview", "evidence", "logs", "visual acceptance", "successful build"),
        new(
            "website.overview", "Output and publishing", "Website Export reference",
            "SQLite templates, Preview, guarded Production, FTPS and restore evidence.",
            """
            Choose the website root and active SQLite template. Generate Preview writes index-test.html and ensures public reports without
            touching Production. Generate Production validates, confirms default-No, backs up existing output and creates a publish plan.

            FTPS Test and Production are separate guarded transfers. Production publishing regenerates first and requires a second live
            confirmation. Credentials remain governed outside exported files. Retain logs/manifests and use Restore only for the last
            governed Production backup.
            """, "website", "template", "preview", "production", "FTPS", "restore"),
        new(
            "website.folder", "Website Export", "Website folder reference",
            "Choose and retain the local website root used by generation and recovery.",
            """
            Website root folder must be the local root containing the canonical index.html location. Choose Folder stores the selected
            root for later sessions; Open Folder only opens that location and does not generate or publish anything. Check the full path
            before every Production action, especially after moving or restoring a website working copy.

            Preview, Production, ExportLogs and Backups use governed locations beneath this root. The manufacturers redirect is also
            maintained by export. An external index.html may be inspected as read-only input, but generated index files are never
            promoted into SQLite as templates.

            A restored folder path is convenience state, not readiness evidence. If the root is missing, unexpected or read-only, stop
            and select the correct owner-controlled location before generating.
            """,
            "folder", "root", "Open Folder", "ExportLogs", "Backups"),
        new(
            "website.templates", "Website Export", "Website templates reference",
            "Import, activate, review and export SQLite-owned website template versions.",
            """
            Template database status identifies the active SQLite template, stored-version count and digest. Every main website export
            uses that active database version. The Template versions list lets you inspect stored versions without silently changing
            which one is active.

            Import HTML validates a selected source file, stores it as a new version and activates it. Use only an intentional master
            template, never index.html or index-test.html generated by the application. Activate Selected explicitly restores the
            chosen stored version as canonical. Export Active Template writes a standalone review/backup copy and does not activate a
            different version.

            Template operations change template ownership, not Materials or measurements. After importing or activating, generate a
            Preview and visually inspect navigation, embedded data, report links and responsive layout before Production.
            """,
            "template", "Import HTML", "Activate Selected", "Export Active Template", "digest", "SQLite"),
        new(
            "website.preview", "Website Export", "Website Preview reference",
            "Generate an isolated local website preview and inspect all dependent output.",
            """
            Generate Preview validates the active SQLite template and current canonical data, automatically rebuilds missing or stale
            eligible public reports, and writes index-test.html. It also writes export-manifest.txt and a timestamped ExportLogs entry.
            Main Website DATA includes every active, non-archived MaterialID; public-report checkboxes control linked report artifacts.

            Preview does not replace index.html, use Production paths or perform FTPS. Open index-test.html and inspect navigation,
            material counts, representative cards, report links and browser layout. Missing public artifacts or validation failures must
            be resolved before Production.

            Preview success is local evidence only. Retain its manifest/log when diagnosing differences, but do not describe the live
            website as updated until a separately authorized Production transfer and independent live check succeed.
            """,
            "preview", "index-test.html", "export-manifest.txt", "ExportLogs", "public reports"),
        new(
            "website.production-generate", "Website Export", "Generate Production reference",
            "Create guarded local Production with preflight validation, backup and publish plan.",
            """
            Generate Production first performs readiness validation and presents a default-No confirmation describing the target and
            consequences. If approved, it ensures required public artifacts, backs up the existing index.html under Backups and then
            writes the new local index.html. The selected external index is never modified as template input.

            A successful generation records export evidence and creates the publish plan used to compare local output with a completed
            remote deployment state. Review the local index.html and plan before any transfer. If validation, backup or generation
            fails, stop; do not bypass the missing stage or infer that the previous file is safe.

            Generate Production does not perform FTPS. It changes local website artifacts only and leaves SQLite business data
            unchanged.
            """,
            "production", "default-No", "index.html", "publish plan", "backup", "does not perform FTPS"),
        new(
            "website.ftps-test", "Website Export", "Website FTPS Test reference",
            "Validate credentials and publish only to the isolated server test route.",
            """
            FTPS uses the governed endpoint displayed in the tab and a password entered in the Password box. Test Connection checks the
            explicit TLS/passive connection and credentials without uploading website content. After a successful encrypted
            connection, the password may be retained in Windows Credential Manager; it is never embedded in exported files.

            Publish Website Test builds the current Preview and transfers it only to the isolated server test route. It does not replace
            Production. Inspect the test URL and retain the transfer log, including the exact remote target and completion result.

            Connection success proves only authentication/connectivity. Test-publish success is not Production approval, does not
            authorize a live transfer and cannot substitute for checking the live Production site.
            """,
            "FTPS test", "Test Connection", "Publish Website Test", "Credential Manager", "isolated"),
        new(
            "website.ftps-production", "Website Export", "Website FTPS Production reference",
            "Perform the separately guarded live transfer and verify the deployed site.",
            """
            Publish Website Production is a live action. It regenerates and verifies guarded Production first, compares local files
            with completed remote deployment evidence, and shows a second default-No confirmation specifically for FTPS. Cancel at
            either confirmation if the root, endpoint, file set or release intent is wrong.

            When approved, the publisher takes the governed recovery snapshot and transfers only changed files over explicit FTPS,
            ordering the root index last. Completion requires retained transfer evidence; a partial or failed operation must remain
            visibly incomplete and must not be described as published.

            After success, independently open the live HTTPS site, bypass stale cache if necessary, and check version/data,
            representative downloads and public-report links. Local generation or a green connection test alone is not live acceptance.
            """,
            "FTPS production", "Publish Website Production", "live", "second confirmation", "root index last"),
        new(
            "website.restore", "Website Export", "Website restore reference",
            "Restore the latest completed Production deployment backup through the guarded recovery path.",
            """
            Restore Last Production Backup is for website deployment recovery, not database recovery. It requires confirmation, creates
            a recovery snapshot of the current remote state, then restores the latest completed Production deployment backup with the
            root index transferred last.

            Only a completed governed backup is eligible. Inspect the named backup and target before approval. If no valid completed
            backup exists, stop and investigate rather than choosing an arbitrary local file.

            Restore does not restore SQLite, change Materials or silently reuse a preview as Production. Retain restore/transfer logs and
            inspect the live site after completion; a successful file transfer still needs functional verification.
            """,
            "restore", "Restore Last Production Backup", "completed Production deployment", "recovery snapshot", "SQLite"),
        new(
            "website.logs-evidence", "Website Export", "Website logs and evidence reference",
            "Interpret export logs, manifests, publish plans, backups and transfer results.",
            """
            Export Log is the immediate readable account of Preview, Production, FTPS and restore actions. Preview evidence normally
            includes index-test.html, export-manifest.txt and a timestamped ExportLogs entry. Production adds the backed-up index path,
            generated index.html and publish-plan/deployment comparison evidence.

            FTPS and restore evidence must identify the endpoint/route, intended file set, changed/skipped results, completion state and
            any recovery snapshot. Passwords must never appear in logs, manifests or exported website files. Retain Verification output
            with release evidence when publication depends on it.

            Missing, contradictory, partial or failed evidence means stop and investigate. Never infer upload success from local file
            existence, infer visual correctness from a manifest, or infer current live state from an older completed transfer.
            """,
            "logs", "manifest", "publish plan", "transfer results", "passwords", "evidence"),
        new(
            "ai.overview", "Assistant", "AI Assistant reference",
            "Local deterministic analysis, planning sessions, collections and read-only output.",
            """
            AI Assistant works from the current visible MaterialID scope and local deterministic services. Review the exact scope preview
            before generating briefs. Sessions and collections are owner-managed local planning records, separate from engineering data.

            Generated output is advisory/read-only. Preview actions write nothing; explicit collection/session actions own their local
            persistence. No external AI service is implied, and Assistant output never replaces measurements or Verification.
            """, "AI Assistant", "visible scope", "sessions", "collections", "output"),
        new(
            "ai.visible-scope", "AI Assistant", "AI visible scope reference",
            "Review the exact visible MaterialID input before any analysis.",
            """
            AI Assistant uses the current Materials search/filter projection. Refresh Visible Scope rebuilds the scope summary and
            MaterialID preview, including row count, unique stable IDs and representative identifiers. Filtering Materials therefore
            changes later briefs even though no engineering row is edited.

            Review the count and IDs before generating. Clear Materials filters for whole-database intent, or deliberately keep them to
            analyze a bounded family/manufacturer subset. Empty, duplicate or legacy identity warnings should be resolved or explicitly
            understood before trusting coverage conclusions.

            Refresh Visible Scope is read-only. It does not save a collection, change lifecycle status or contact an external AI
            service.
            """,
            "visible scope", "Refresh Visible Scope", "MaterialID", "filters", "read-only"),
        new(
            "ai.planning-briefs", "AI Assistant", "AI planning briefs reference",
            "Generate deterministic local briefs from templates or named analysis actions.",
            """
            Template chooses Video Ideas, Comparison Planning, Thumbnail Hooks, Full Assistant Brief or Playlist Suggestions. Planning
            note is included as owner-authored context; it is not interpreted by an external model. Generate From Template uses the
            selected template, while the named buttons generate Video Ideas, Comparisons, Recommended Next Video, Recommended
            Comparisons, Hidden Gems or the complete Full Assistant Brief.

            Every action consumes the reviewed visible MaterialID scope and canonical local summaries. Results are deterministic
            advisory text in the read-only output box. Check cited materials, missing evidence and scope before reusing a recommendation.

            Generation does not edit Materials, measurements, reports or website data and does not save a session automatically. Use
            Save Session only when the current title, template, note and output should become a retained local planning record.
            """,
            "briefs", "Generate From Template", "Recommended Next Video", "Hidden Gems", "planning note"),
        new(
            "ai.sessions", "AI Assistant", "AI sessions reference",
            "Save, reload, refresh and explicitly delete local planning sessions.",
            """
            Session title names the planning record. Save Session retains the current planning context and generated output locally;
            confirm the intended title and current scope before saving. Research sessions selects an existing record. Load Session
            restores its saved planning state, while Refresh Sessions reloads the list from storage.

            Delete Session is an explicit destructive planning-data action and requires the intended saved session to be selected.
            Deletion does not delete Materials or measurements, but the session context cannot be inferred later from engineering data.

            Sessions are owner-managed planning history, not canonical evidence and not an external conversation. Loading an older
            session does not recalculate it against today’s visible rows; generate again when a fresh projection is required.
            """,
            "sessions", "Save Session", "Load Session", "Delete Session", "Refresh Sessions"),
        new(
            "ai.collections", "AI Assistant", "AI collections reference",
            "Preview, create, update, reuse and delete exact MaterialID collections.",
            """
            Intelligence scope chooses Current visible rows or Selected collection. Preview Visible Collection shows the exact unique
            MaterialIDs that would be used and performs no write. Collection title names a new collection; Collections selects an
            existing stable collection. The action-state line explains whether Create / Update Collection will create or replace the
            selected planning snapshot.

            Create / Update Collection is the explicit persistence boundary. Load Collection Brief analyzes the selected saved
            collection. Delete Collection removes that planning collection after owner intent; it does not delete the referenced
            Materials. A collection stores stable MaterialID membership rather than duplicate material records.

            Collection Dashboard and Video Pipeline Dashboard summarize local planning coverage. Review missing/legacy identity before
            treating their counts as complete, and re-preview visible rows before updating membership.
            """,
            "collections", "Preview Visible Collection", "Create / Update Collection", "Load Collection Brief", "Dashboard"),
        new(
            "ai.coverage-status", "AI Assistant", "AI coverage status reference",
            "Manage local workflow status and resolve legacy coverage identity safely.",
            """
            Material status offers Untested, Tested, Video Planned, Filmed, Edited and Published. Apply Status writes the chosen local
            workflow state for the selected collection/material scope. Mark Collection Published applies the published workflow state
            deliberately. These statuses describe creator planning; they do not change Material lifecycle, test readiness or website
            publication.

            Clear Selected Collection Status removes saved workflow statuses for the selected collection without deleting the
            collection. Bind Exact Legacy Coverage first previews legacy collection-title/material-label rows and only binds candidates
            when exact unique CollectionID and MaterialID matches exist; ambiguous or unmatched rows remain visible for review.

            Coverage identity status reports stable bindings, legacy rows and candidates. It is diagnostic evidence for planning
            ownership, not proof of measurements, video publication or Verification readiness.
            """,
            "coverage", "Apply Status", "Mark Collection Published", "Clear Selected Collection Status", "Bind Exact Legacy Coverage"),
        new(
            "ai.output", "AI Assistant", "AI output reference",
            "Interpret the read-only local output and hand it off only after owner review.",
            """
            AI Assistant Output displays generated briefs, collection previews, dashboards, coverage diagnostics and action results in
            a read-only text surface. A new action replaces the visible output, so save a session or copy intentionally when the current
            planning result must be retained.

            Output is rule-based local analysis, not a response from an external AI service. Recommendations depend on the visible or
            selected collection scope and available canonical summaries; missing measurements must remain visible as missing rather
            than being invented.

            Any copy/manual handoff leaves the governed application context. Review material identity, claims, dates and intended
            audience first. Reading or copying output does not publish, edit SQLite engineering data or certify the recommendation.
            """,
            "output", "read-only", "local rule-based", "copy", "missing measurements"),
        new(
            "youtube.overview", "Creator tools", "YouTube Research reference",
            "Generate local creator research, review evidence and copy owner-selected planning text.",
            """
            Generate YouTube Research derives local title, thumbnail, comparison, gap, calendar, playlist and candidate planning from
            canonical visible data. It changes no engineering measurement or public website.

            Seven copy actions place owner-review text on the clipboard. Review scope and claims before external use; clipboard success
            is not publication or evidence that a video was produced.
            """, "YouTube", "generate", "titles", "thumbnail", "calendar", "playlist"),
        new(
            "youtube.generate", "YouTube Research", "Generate YouTube Research reference",
            "Refresh every local title, thumbnail, comparison, calendar, gap and playlist projection.",
            """
            Generate YouTube Research rebuilds all creator-research sections from the current Materials filter projection and canonical
            local analysis signals. Inputs include no-video priority, score profile, reinforcement/variant hooks, comparison gaps and
            data outliers. The status line reports the resulting candidate state.

            Generation refreshes Top Thumbnail, Comparison Discovery, the 12-week Content Calendar, Channel Gap Analysis, Playlist
            Discovery and the ranked title-candidate table together. Review the visible Materials scope before generating; clearing or
            changing filters changes the candidate population.

            The operation is read-only for engineering and website data. It does not contact YouTube, publish content, save an external
            plan or prove that suggested claims have enough evidence for production.
            """,
            "Generate YouTube Research", "filters", "no-video priority", "data outliers", "read-only"),
        new(
            "youtube.copy-actions", "YouTube Research", "YouTube copy actions reference",
            "Understand the seven explicit clipboard handoffs and their review boundary.",
            """
            Copy Top 10 Title Prompt copies ranked title-planning text. Copy Best Thumbnail copies the leading thumbnail recommendation;
            Copy Top 10 Thumbnail Briefs copies the wider thumbnail set. Copy Top Comparisons, Copy 12 Week Plan, Copy Top Gaps and Copy
            Top Playlists copy their named generated sections. Each action uses the most recently generated local results.

            These seven buttons write formatted text to the Windows clipboard only. They do not create files, send prompts, call an AI
            service, publish to YouTube or change SQLite. If research has not been generated or has become stale after filters changed,
            regenerate before copying.

            Clipboard content is an external handoff once pasted elsewhere. Inspect Material names, evidence-sensitive claims, scope,
            dates and formatting before use; clipboard success is not publication or acceptance evidence.
            """,
            "seven", "Copy Top 10 Title Prompt", "Copy Best Thumbnail", "Copy 12 Week Plan", "clipboard"),
        new(
            "youtube.thumbnail", "YouTube Research", "YouTube thumbnail reference",
            "Interpret the top thumbnail recommendation and ranked thumbnail briefs.",
            """
            Top Thumbnail Recommendation displays main text, grade, pattern, visual layout, face/reaction direction and the reason for
            the selection. The ranked candidate table adds thumbnail score, hook angle and per-material context. These fields are
            planning prompts for a human-designed asset.

            Compare the suggested hook with the underlying material and available results. A high thumbnail grade ranks the local
            presentation pattern; it does not certify a scientific claim, predict platform performance or establish a public result.

            Copy Best Thumbnail and Copy Top 10 Thumbnail Briefs place text on the clipboard. They do not generate an image asset,
            modify branding, upload a thumbnail or create a YouTube draft.
            """,
            "thumbnail", "grade", "visual layout", "face reaction", "image asset"),
        new(
            "youtube.comparisons", "YouTube Research", "YouTube comparisons reference",
            "Review ranked comparison opportunities without changing engineering rankings.",
            """
            Comparison Discovery shows the leading video angle, score, pattern, thumbnail text, Material A, Material B and reason, then
            lists additional candidates. The ranking looks for comparable visible materials and useful gaps; it is a creator-planning
            projection rather than the canonical Rankings Dashboard.

            Confirm that both materials belong in the intended comparison and that missing or unequal test coverage is stated honestly.
            Suggested pairings must not be presented as measured winners unless the canonical results support that conclusion.

            Copy Top Comparisons writes the generated planning text to the clipboard only. Comparison research never adds measurements,
            changes scores, opts materials into public reports or publishes a comparison page.
            """,
            "comparisons", "Material A", "Material B", "canonical Rankings Dashboard", "Copy Top Comparisons"),
        new(
            "youtube.gaps", "YouTube Research", "YouTube content gaps reference",
            "Interpret channel coverage gaps separately from engineering-data gaps.",
            """
            Channel Gap Analysis ranks uncovered creator opportunities by gap score and type. It shows material, manufacturer, family,
            proposed video, thumbnail text, playlist completion and the reason the opportunity is considered a gap.

            A channel gap means absent or weak creator coverage in the local planning model. It does not automatically mean missing
            measurements, failed Verification or an incomplete Material record. Read the source status and canonical results before
            making an engineering claim.

            Copy Top Gaps copies owner-review planning text to the clipboard. Gap analysis does not create tasks, videos, Materials or
            external channel state.
            """,
            "gaps", "gap score", "playlist completion", "engineering-data gaps", "Copy Top Gaps"),
        new(
            "youtube.calendar", "YouTube Research", "YouTube calendar reference",
            "Review the generated 12-week publishing sequence and its diversity rules.",
            """
            Content Calendar Planner assigns candidate videos to weeks with publish priority, manufacturer, material family, thumbnail
            and a reason for that week. The planner avoids long runs from the same manufacturer or family and uses the currently
            generated candidate population.

            Treat the sequence as advisory. Confirm production capacity, embargoes, evidence readiness and owner priorities before
            adopting dates. Regenerate after material filters or relevant planning signals change.

            Copy 12 Week Plan writes text to the clipboard only. These suggestions do not create external calendar events, reminders,
            YouTube schedules or Material lifecycle changes.
            """,
            "calendar", "12-week", "diversity", "Copy 12 Week Plan", "do not create external calendar events"),
        new(
            "youtube.playlists", "YouTube Research", "YouTube playlists reference",
            "Review grouped playlist opportunities, coverage and proposed next videos.",
            """
            Playlist Discovery ranks candidate groups by score and type. For each group it shows coverage, videos already represented,
            missing/next candidates and the reason the grouping may be useful. The top panel summarizes the strongest current
            opportunity.

            Coverage is local planning coverage, not a query of the live YouTube channel. Verify actual channel state and confirm that
            grouped materials belong together before adopting the recommendation.

            Copy Top Playlists copies formatted suggestions to the clipboard. It does not create, rename or modify YouTube playlists,
            and it does not change engineering or Material data.
            """,
            "playlists", "coverage", "missing next", "Copy Top Playlists", "live YouTube channel"),
        new(
            "youtube.candidates", "YouTube Research", "YouTube candidates reference",
            "Read the ranked per-material title, hook and comparison candidate table.",
            """
            The candidate table shows thumbnail score and grade, pattern, Material, manufacturer, type, reinforcement, advanced title,
            hook angle, main thumbnail text, visual layout, face/reaction direction and comparison idea. Sort/read these as planning
            attributes tied to the current generated scope.

            Candidate order combines local signals such as missing-video priority, score profile, variants and outliers. It is not an
            engineering ranking and does not guarantee audience performance. Confirm every evidence-sensitive title against canonical
            measurements and public-safe wording.

            The grid is read-only. Generate refreshes it; copy actions hand off selected sections, but no candidate is automatically
            saved, produced or published.
            """,
            "candidates", "advanced title", "reinforcement", "hook angle", "read-only"),
        new(
            "help.whitepaper", "Help and support", "Engineering Whitepaper reference",
            "Export the governed engineering-methodology PDF to an owner-selected path.",
            """
            Export Engineering Methodology Whitepaper builds the packaged methodology document for the running release and asks for a
            local destination. Review the selected filename and folder before saving, then open the PDF and inspect page layout,
            branding, formulas and version context.

            The whitepaper explains governed methodology; it is not a database backup, Verification report or live website action.
            Export writes only the selected local PDF and does not alter SQLite, measurements or publishing state.
            """,
            "whitepaper", "methodology", "PDF", "local destination"),
        new(
            "help.changelog", "Help and support", "Changelog reference",
            "Read the packaged chronological release history for the installed application.",
            """
            Changelog opens the release-history document shipped with the running build. Use it to identify delivered features,
            corrections and compatibility notes for earlier versions. It is read-only and may describe historical behavior that has
            since been replaced.

            The Changelog is not the authoritative plan for unstarted work; current roadmap governance remains in project documentation.
            Opening it changes no settings, files or SQLite data.
            """,
            "changelog", "release history", "historical behavior", "roadmap"),
        new(
            "help.about", "Help and support", "About reference",
            "Confirm the running version, storage model, license and third-party notices.",
            """
            About identifies the installed 3DPIceland release and its native SQLite storage model. Use the displayed version when
            collecting diagnostics, comparing an update feed or reporting a runtime issue.

            License and third-party notices describe distribution and dependency obligations. They are informational and do not grant
            publishing readiness or database compatibility by themselves. About changes no settings, files or data.
            """,
            "about", "version", "SQLite", "license", "third-party notices"),
        new(
            "menu.tools-validation", "Menus and support", "Tools validation reference",
            "Validate Materials, rebuild computed display fields and inspect the rendering prototype.",
            """
            Validate Materials checks the current Material records and reports missing or invalid governed identity without silently
            repairing rows. Rebuild Computed Fields is different: after confirmation it recalculates persisted computed Material display
            fields from their canonical inputs. Create a backup and review the validation result before using that mutating command.

            Materials Rendering Prototype opens the supported diagnostic/prototype view for rendering inspection; it is not another
            Materials editor and does not replace the accepted owner-drawn grid. Update, release-publishing and storage commands in the
            Tools menu have separate safety ownership and are documented in v50.3.
            """,
            "Tools", "Validate Materials", "Rebuild Computed Fields", "rendering prototype", "mutating"),
        new(
            "menu.help", "Menus and support", "Help menu reference",
            "Choose whole-system, contextual, evidence and packaged support destinations.",
            """
            Documentation opens the Start-to-finish whole-system overview. Help for Current View, also available with F1, opens the same
            central Help window at the selected top-level or supported nested tab. Search remains available from either entry point.

            Export Engineering Whitepaper writes the governed methodology PDF to an owner-selected path. Changelog and About are
            read-only packaged information. Verification Center and System Diagnostics are evidence/support surfaces with their own
            refresh, export and mutating recalculation boundaries, documented with recovery and publishing safety in v50.3.
            """,
            "Help menu", "Documentation", "Help for Current View", "F1", "Verification Center", "System Diagnostics"),
        new(
            "verification-recovery",
            "Safety and support",
            "Verification, diagnostics, backup and recovery",
            "Prove data readiness, collect support evidence and recover through guarded paths.",
            """
            Verification Center evaluates the current database and application contracts. PASS is required release evidence, but visual
            HTML/PDF/UI acceptance may still be manual.

            System Diagnostics records environment and ownership facts useful for support. Export both reports when investigating a
            runtime issue.

            Create a manual SQLite backup before risky owner actions. Recovery Center verifies candidate backups and preserves recovery
            evidence. Excel is a governed disaster-recovery path, not the live source of truth. Automation must use a disposable profile
            and must never mutate the canonical owner database.
            """,
            "verification center", "PASS", "diagnostics", "backup", "restore", "recovery", "SQLite", "Excel")
    ];

    internal static string SectionIdForTab(string? tabHeader) => tabHeader switch
    {
        "Materials" => "materials.overview",
        "Manufacturers" => "manufacturers.overview",
        "Purchase Orders" => "purchase-orders.overview",
        "Inventory" => "inventory.overview",
        "Usage" => "usage.overview",
        "Printers" => "printers.overview",
        "Print Job Quotes" => "print-job-quotes.overview",
        "Base Materials" => "base-materials.overview",
        "Settings Manager" => "settings.overview",
        "Material Detail" => "material-detail.general",
        "Tensile Measurements" => "measurements.tensile",
        "Impact Measurements" => "measurements.impact",
        "Stiffness Measurements" => "measurements.stiffness",
        "Experimental Testing" => "experimental.series",
        "Rankings Dashboard" => "analysis.rankings",
        "Category Rankings" => "analysis.category-rankings",
        "Awards & Winners" => "analysis.awards",
        "Dashboard Insights" => "analysis.dashboard-insights",
        "Reports / PDF Export" => "reports.overview",
        "Website Export" => "website.overview",
        "AI Assistant" => "ai.overview",
        "YouTube Research" => "youtube.overview",
        _ => StartHereId
    };

    internal static string SectionIdForExperimentalTab(string? editorHeader, string? resultsHeader = null) =>
        editorHeader switch
        {
            "Tensile" => "experimental.measurements.tensile",
            "Impact" => "experimental.measurements.impact",
            "Stiffness" => "experimental.measurements.stiffness",
            "Results" => resultsHeader switch
            {
                "Dashboard" => "experimental.results.dashboard",
                "Table" => "experimental.results.table",
                "Charts" => "experimental.results.charts",
                _ => "experimental.results"
            },
            _ => "experimental.series"
        };

    internal static string SectionIdForMaterialDetailTab(string? nestedHeader) => nestedHeader switch
    {
        "General" => "material-detail.general",
        "Printing Profile" => "material-detail.printing-profile",
        "Mechanical" => "material-detail.mechanical",
        "Charts" => "material-detail.charts",
        "Analytics" => "material-detail.analytics",
        "Compare" => "material-detail.compare",
        "Video Planner" => "material-detail.video-planner",
        "Recommendations" => "material-detail.recommendations",
        "Notes" => "material-detail.notes",
        _ => "material-detail.general"
    };
}
