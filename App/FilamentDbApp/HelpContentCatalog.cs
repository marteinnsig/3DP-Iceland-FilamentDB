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
        "Reports / PDF Export" or "Website Export" => "reports-website",
        "AI Assistant" or "YouTube Research" => "settings-tools",
        _ => StartHereId
    };
}
