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
            Confirm spool MaterialID, quantity, weight, remaining weight, storage and immutable purchase snapshot in Inventory.
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
            "purchasing-inventory",
            "Purchasing and cost",
            "Purchase Orders, Inventory and Usage",
            "Create purchase snapshots, receive lots and record append-only usage.",
            """
            Purchase Orders own supplier transactions and their currency snapshot. New Order and later grid edits auto-save. ECB data
            is an optional reference prefill only for a session-new order; review rate/provenance before Calculate Landed Costs or
            receiving. Calculate Landed Costs validates and persists allocations and updates current linked-Material pricing evidence.

            Receive / Reconcile records counts and check status. Create Materials + Received Spools is the separate Inventory mutation:
            only Filament lines create/reuse draft Materials and one spool per received unit. Other categories remain PO-only. A newer
            exchange rate never rewrites the saved PO rate or created Inventory, Usage or quote history.

            Usage records consumption as ledger events. Corrections add governed correcting events instead of silently changing
            history. Confirm MaterialID, optional matching spool, fixed grams/minutes/count fields and provenance before recording.
            """,
            "purchase order", "supplier", "ECB", "exchange rate", "inventory", "receive", "usage", "correction"),
        new(
            "materials-catalogs",
            "Materials and catalogs",
            "Materials, Manufacturers and Base Materials",
            "Maintain stable material identity and governed reusable catalog values.",
            """
            Materials is the primary material list. MaterialID is the stable join key used by measurements and downstream outputs.
            Archive when history must be retained; delete only when the UI explicitly permits it and no supported relationship blocks it.

            Manufacturers and Base Materials govern reusable values. Prefer exact binding to an existing catalog entry over creating
            spelling variants. Unmapped legacy values remain visible until deliberately resolved.

            Material Detail contains the selected material's general information, printing profile, mechanical results, charts,
            analytics, comparisons, video planning, recommendations and notes.
            """,
            "MaterialID", "manufacturer", "base material", "archive", "material detail", "printing profile"),
        new(
            "printers-quotes",
            "Costing and quoting",
            "Printers and Print Job Quotes",
            "Maintain governed machine rates and immutable customer quote snapshots.",
            """
            Printers stores the governed time and energy inputs used when a new quote is calculated. Review currency, hourly cost,
            power and related settings before quoting.

            Print Job Quotes calculates an unsaved draft from the selected material, printer and job inputs. Saving creates an immutable
            pricing snapshot. Later changes to Settings, printer rates, material prices or exchange-rate references must not recalculate
            an already saved quote. Customer PDF output is produced from the saved snapshot.
            """,
            "printer", "hourly rate", "energy", "quote", "snapshot", "customer PDF"),
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
        "Purchase Orders" or "Inventory" or "Usage" => "purchasing-inventory",
        "Materials" or "Manufacturers" or "Base Materials" or "Material Detail" => "materials-catalogs",
        "Printers" or "Print Job Quotes" => "printers-quotes",
        "Tensile Measurements" or "Impact Measurements" or "Stiffness Measurements" => "measurements",
        "Experimental Testing" => "experimental-testing",
        "Rankings Dashboard" or "Category Rankings" or "Awards & Winners" or "Dashboard Insights" => "analysis",
        "Reports / PDF Export" or "Website Export" => "reports-website",
        "Settings Manager" or "AI Assistant" or "YouTube Research" => "settings-tools",
        _ => StartHereId
    };
}
