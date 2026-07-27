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
            1. Record the purchase
            Create the Purchase Order and its lines. Refresh the ECB reference only when you need a current reference rate for new,
            unsaved purchase data. Saved purchases, inventory lots, material costs and quotes are historical snapshots and are never
            recalculated by a later exchange-rate refresh.

            2. Receive and identify inventory
            Receive eligible purchase lines into Inventory. Link the lot to the correct material identity. Use Manufacturers and Base
            Materials to govern reusable catalog values; do not create near-duplicate names when an accepted value already exists.

            3. Complete the material record
            Use Materials and Material Detail for identity, printing profile, links, notes and publication eligibility. Save changes
            before relying on the record in measurements, reports or website output.

            4. Record controlled measurements
            Enter Tensile, Impact and Stiffness samples under the correct MaterialID. Experimental Testing is for governed series and
            runs; its active/history controls change comparison scope, not the historical records themselves.

            5. Review engineering results
            Review Material Detail, Rankings, Category Rankings, Awards and Dashboard Insights. These surfaces consume canonical
            calculated results; they do not replace source measurements.

            6. Produce output
            Build reports and preview website output. Confirm the intended public material selection and inspect HTML/PDF visually.
            Run Verification Center and require PASS before release or public publishing.

            7. Publish only with explicit authority
            Production and FTPS remain guarded and default to No. A preview, verification result or generated package is not proof that
            Production was published. Retain the resulting transfer or release evidence.

            8. Recover safely
            Use governed backups, Recovery Center and diagnostics. Never replace the owner database with an automation fixture. SQLite
            restore and Excel disaster recovery are explicit operations with validation and evidence.
            """,
            "workflow", "purchase", "inventory", "material", "measurements", "verification", "publish", "recovery"),
        new(
            "purchasing-inventory",
            "Purchasing and cost",
            "Purchase Orders, Inventory and Usage",
            "Create purchase snapshots, receive lots and record append-only usage.",
            """
            Purchase Orders own supplier transactions and their currency snapshot. Enter the supplier currency and rate that applies
            to the new order. ECB data is an optional reference prefill; review it before saving.

            Receiving creates inventory history from eligible lines. Inventory quantity and value must remain traceable to the saved
            purchase and lot. A newer exchange rate must never rewrite an older purchase, inventory lot, material price or quote.

            Usage records consumption as ledger events. Corrections add governed correcting events instead of silently changing
            history. Confirm the selected material, lot, amount and unit before saving.
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
            Select the intended MaterialID before entering measurements. Use the unit and specimen workflow shown in each measurement
            tab. Tensile Upright and Flat are independent; Impact and Stiffness use their own accepted input contracts.

            Validation messages identify missing, malformed or incomplete inputs. Fix source data rather than typing calculated output
            manually. Recalculate and review results after source changes, then run Verification Center before publishing.
            """,
            "tensile", "impact", "stiffness", "samples", "MPa", "validation", "calculation"),
        new(
            "experimental-testing",
            "Testing and engineering",
            "Experimental Testing",
            "Manage material-linked test series, controlled runs and publication readiness.",
            """
            A series defines the material, experiment and default unit. Runs hold the controlled values and measurement state.
            Active only filters series visibility. Include inactive history in comparison affects result comparison scope only when
            inactive completed history exists; it does not change the run grid or reactivate records.

            Website publication is a deliberate series property. A checkbox edit is committed when the grid edit commits, which may
            occur when focus moves. Review the publication-readiness message before website export.
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

            Filters and comparison scopes can change what is visible without changing stored measurements. Confirm the visible scope
            before exporting or using a result in a decision.
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

            Website Export builds the public package from materials explicitly eligible for public reports. Preview is the normal review
            path. Public allowlists and route checks remain authoritative. Production and FTPS require explicit confirmation and default
            to No; never infer a successful publish solely from local generation.

            Run Verification Center and require PASS before publishing. Retain publish-plan, hash and transfer evidence where produced.
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
