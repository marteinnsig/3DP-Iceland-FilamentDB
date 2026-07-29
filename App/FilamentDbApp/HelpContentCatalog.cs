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
            currency, tax treatment, charges and allocation method. Choosing Currency refreshes rate and provenance when the grid
            cell is committed with Enter or focus change. ECB is an optional reference for a session-new order only; New Order refreshes
            a missing or more-than-24-hour-old cache. Review the displayed observation/fetch provenance before calculating costs or
            receiving. Later ECB refreshes never rewrite a saved order rate or created Inventory, Usage or quote history.

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
            Search plus no-modifier Manufacturer, Base Material, Variant / Finish, Reinforcement, Color and Product Line multi-select
            filters control the visible workflow. Values inside one filter use OR; different filters, Category and Test status use AND.
            Each multi-select shows its selected count and values and has Clear. Clear All Filters also clears Search, Category and Test
            status. Exact selections persist per local profile across restart; unavailable saved values remain visible until cleared.
            Filters affect native measurement visibility and global analysis/report Visible scope but never change canonical records.

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
            "materials.controls-fields",
            "Materials and catalogs",
            "Materials controls and fields",
            "Every Materials action, filter and owner-drawn grid field, including editability, units and save timing.",
            """
            Actions
            Add Material creates a Manual Backup first, assigns a new read-only Material ID and inserts a default draft. Duplicate
            Material copies the selected product, profile and price facts but deliberately clears review URL and resets test, video and
            archive state. Archive Material removes the selected row from active/output scope without deleting history; Unarchive
            restores that scope. Delete Material is permanent and uses the same named default-No warning as Base Materials. Enter on No,
            Escape or closing the warning preserves the Material and current selection; only explicit Yes creates a backup and removes
            the Material plus associated native measurement references. Manual Backup creates evidence but does not save pending editor
            text. Reset Columns uses a named default-No warning; No, Escape or closing it preserves the current layout. Explicit Yes
            restores the current application default column order/width only and persists across restart. An existing saved user
            layout otherwise remains authoritative across application upgrades. Normal committed cell edits auto-save; there is no
            separate Save requirement.

            Search and filter controls
            Search searches displayed identity/content. Manufacturer, Base Material, Variant / Finish, Reinforcement, Color and Product
            Line are no-modifier multi-select filters: click each value or use keyboard activation without Ctrl/Shift. Values within one
            filter use OR, while different filters, Search, Category and Test status use AND. Each filter shows selected values, a count
            and Clear; selected zero-count or unavailable restart values remain visible so they can be reviewed and cleared. Clear All
            Filters restores the normal scope. Exact Search and multi-select state persist per local profile across restart.

            Filtering is read-only. Archived rows remain discoverable in Materials Manager, including linked/unlinked review, but leave
            canonical active consumer scope. An empty result clears the current Materials selection/details and changes no database,
            relationship, measurement, report publication or website publication state. Native Measurements, AI Assistant, collections
            and All Visible Materials reports reuse the exact active visible MaterialID projection.

            Identity, publication and catalog cells
            Material ID is generated/read-only. Manufacturer and Base Material are governed selectors; Product Line, Marketing Name,
            Variant / Finish, Reinforcement and Color are editable text. Category is derived/read-only. Public reports and Public test
            details are independent checkboxes: the first controls linked public report artifacts, the second permits eligible raw test
            detail. Manufacturer Website and YouTube Review URL accept reviewed URLs. Video, Tested Status and In Tensile/Impact/
            Stiffness are read-only state. Notes is editable multiline content. Website Display Name, Material Key and Validation are
            read-only derived output.

            Default column order
            The default begins with identity and test status, then Notes, website identity/media, spool and price facts, inventory,
            purchase evidence, publication choices and internal metadata. Existing saved user layouts keep their own width/order until
            Reset Columns is explicitly accepted. All 52 Materials columns remain visible and unfrozen; layout never changes canonical
            data, report/PDF fields or website-export fields.

            Inventory and purchase-evidence cells
            Spool Weight g / spool and Remaining Weight g / spool are grams; Remaining cannot exceed spool weight. Manufacturer SKU,
            Inventory ID, Purchase ID, Purchased From, Supplier URL, Order Number, Batch Number and Storage Location are editable
            references. Purchase Date and Price Checked are dates. Inventory Status is Unopened, Opened or Empty. Inventory Qty is
            calculated/read-only. Purchase Price, Shipping, VAT, MSRP Amount and Landed Cost are non-negative monetary inputs. Purchase
            Currency is directly beside Purchase Price; MSRP and Landed Cost have their own adjacent currencies. Currency choices are
            governed; normalized MSRP USD, Landed USD and both USD/kg columns are calculated read-only. Thumbnail Filename is a
            local/public asset reference. Sort Order and Source Priority are read-only.

            Archive and validation cells
            Archived / exclude from website export is the grid equivalent of Archive/Unarchive and auto-saves after confirmation-safe
            lifecycle handling. Validation explains missing/invalid identity, numeric or relationship data. Fix the named source field;
            do not type into Validation or another calculated cell.

            Bulk Update dialog
            Bulk Update appears only from a supported editable custom-grid cell. Remaining visible rows below starts after the source
            row; Current filtered rows includes the complete filtered result. The summary states rows in scope, already-equal values,
            empty values and existing different values that would be overwritten. Cancel changes nothing. Update is disabled when
            nothing would change and otherwise applies the displayed value to the chosen scope, then uses normal validation/save rules.
            """,
            "materials controls", "material fields", "bulk update", "remaining visible rows", "current filtered rows",
            "spool weight", "remaining weight", "purchase price", "landed cost", "validation"),
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
            "manufacturers.controls-fields",
            "Materials and catalogs",
            "Manufacturers controls and fields",
            "Every Manufacturers command, filter and editable grid column with lifecycle and relationship effects.",
            """
            Commands and scope
            Find searches manufacturer rows. Show archived adds inactive rows; clearing it does not delete them. Add Manufacturer
            creates and immediately saves a uniquely named draft. Duplicate Manufacturer copies the selected descriptive record under
            a new identity. Archive / Restore toggles Active and preserves linked history. Delete Manufacturer is permanent,
            confirmation-guarded and blocked while canonical Materials reference its ManufacturerID.

            Bind Exact Material Names
            This action scans unbound legacy Material manufacturer text. Only one-to-one exact name matches are proposed. The preview
            must be reviewed and confirmed; ambiguous, near or unmatched names remain unchanged. Binding adds the stable relationship
            and does not rewrite unrelated Material identity.

            Identity and web fields
            Name is the canonical unique relationship name. Display Name is public/presentation wording and may differ. Country,
            Headquarters and Founded are descriptive; Founded expects a sensible year or blank. Website and Logo URL accept reviewed
            web addresses. Name commits can propagate the canonical display-name snapshot to linked Materials after validation.

            Engineering and descriptive fields
            Description, Engineering Focus, Material Categories, Strengths, Weaknesses, Sustainability, Typical Applications and
            Notes are editable text used for governed interpretation/output. Sort is numeric display order. Active controls normal
            scope and is the grid equivalent of Archive / Restore. Every committed property edit auto-saves; invalid unique-name or
            relationship changes are rejected and the prior canonical state remains.
            """,
            "manufacturer fields", "display name", "country", "founded", "website", "logo url", "engineering focus",
            "strengths", "weaknesses", "sustainability", "typical applications", "active"),
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
            Refresh ECB Reference is optional and offline-safe. New Order also refreshes a missing or more-than-24-hour-old cache,
            at most once daily; failed network access retains valid cache/manual fallback. Settings `Default Landed Cost Currency` is
            copied only into a newly created Draft; changing Settings never changes an existing order. Select an uncalculated Draft
            to review or override its landed-cost currency. Apply requires default-No confirmation and snapshots the target currency,
            conversion rate,
            source and date. `Manual Governed Settings` means owner-entered offline rates, not downloaded ECB data. The displayed
            direction is `1 invoice currency = rate landed-cost currency`; ECB cross-rates use both
            currencies from one catalog, otherwise both positive manual `ISK per 1` Settings legs are required. Same currency is the
            only valid automatic 1:1 case. A legacy, calculated or non-Draft order is locked, including after restart. Calculate
            Landed Costs keeps unit price, discounts, charges and allocated components in invoice currency, then multiplies only
            Landed line/unit/kg results by the snapshotted rate. Validation runs without clearing saved results on failure. One
            successful calculation stores UTC/version metadata and permanently locks that order against recalculation. It then
            synchronizes only the selected order's current linked-Material pricing evidence. Resolve the Cost Allocation Validation
            message before receiving.

            Receiving and downstream creation
            Receive / Reconcile records counts, Check state, stage and received date; it does not create Inventory. Create Material from
            Selected Item requires a described Filament line and creates/links a draft requiring later identity review. Create Materials
            + Received Spools requires the accepted landed-cost snapshot, processes only positively received Filament lines,
            creates/reuses Materials and adds one Unopened spool per unit; it is repeat-safe. New spools retain invoice purchase
            price/currency separately from landed amount/currency and copy the order's rate provenance. Existing Inventory is never
            refreshed. Other categories remain PO-only. Attach Document is optional and copies the file into governed storage.
            """,
            "new order", "delete order", "add ordered item", "delete item", "currency", "ECB", "landed costs",
            "receive reconcile", "create materials received spools", "attachment", "auto-save", "workflow stage"),
        new(
            "purchase-orders.controls-fields",
            "Purchasing and cost",
            "Purchase Orders controls and fields",
            "Every order/line command and column, including currencies, allocation, receiving and historical boundaries.",
            """
            Order actions
            New Order immediately creates a Draft header. Delete Order is permanent/default-No for the order and lines but never
            deletes Inventory already created from it. Refresh ECB Reference uses official cached/network reference data. New Order
            opportunistically refreshes a missing or more-than-24-hour-old cache before snapshotting the new Draft; it never replaces
            a saved rate or recalculates historical prices. Attach
            Document copies a chosen invoice/evidence file into governed storage. Calculate Landed Costs commits pending edits,
            validates totals/allocations, converts only Landed line/unit/kg results into the selected landed currency, saves a
            one-time calculation snapshot and updates current linked-Material pricing evidence. Invoice inputs and allocated
            shipping/tax/customs/fees remain in invoice currency. A failed validation preserves prior saved results. A calculated or
            legacy order cannot be recalculated. Receive / Reconcile
            records received quantities/check state and does not create Inventory. Create Materials + Received Spools is repeat-safe
            and processes only positively received Filament lines. Create Material from Selected Item creates and links one draft
            Material for the selected described Filament line.

            Order header columns
            PO ID is generated/read-only. Supplier and Order # are editable references; Order date and Received date are dates.
            Workflow stage choices are Draft, Ordered, Awaiting import charges, Awaiting delivery, Receiving, Verified, Inventory
            created and Complete. Currency is the invoice currency. `1 currency unit = ISK` is the reviewed conversion input used by
            current calculations; it must be positive. Rate source and Rate date are read-only provenance and never silently refresh.
            `Default Landed Cost Currency` in Settings prefills only a new Draft. The landed-cost review below the order grid shows
            `1 invoice currency = rate landed-cost currency`. Its selector and Apply action remain enabled only while both lifecycle
            and cost status are Draft and no calculation version/time exists. Apply is default-No and stores reviewed currency,
            positive cross-rate and provenance before calculation. Same currency alone uses 1:1. Landed result columns show the
            selected landed currency while net/allocation columns remain invoice currency. A legacy, calculated or non-Draft
            snapshot stays locked across restart.
            A successful calculation also stores calculation UTC and calculation version as read-only provenance. These fields prove
            which calculation contract produced the saved result; they are not refreshed by later Settings or ECB changes.
            Tax treatment is the governed tax mode. Items total, Shipping, Supplier VAT, Invoice total, Import VAT, Customs, Clearance
            and Other fees are non-negative header amounts in the order currency unless the visible label states otherwise. Notes is
            editable. Cost status and Document are read-only readiness/evidence summaries.

            Ordered-item actions and identity
            Add Ordered Item immediately inserts a default Filament line. Delete Item permanently removes only the selected line.
            Line ID is generated/read-only. Ordered item / invoice description and SKU are editable. Category choices are Filament,
            Printer, Equipment, Spare Parts, Consumables and Other. Link existing material is optional and must match the intended
            canonical MaterialID. Storage location and Notes are editable handoff values.

            Quantity, price and allocation columns
            Expected and Received are non-negative quantities; Received drives receiving eligibility. Check records the line review
            state. Unit price and Discount are monetary inputs; Unit weight g is grams per unit. Allocate enables governed shared-cost
            allocation. Shipping allocation selects the allocation method; Manual shipping is used only by the applicable manual mode.
            Net line, Shipping allocated, Tax allocated, Customs allocated, Fees allocated, Landed line, Landed / unit and Landed / kg
            are calculated/read-only. Allocation status explains incomplete/invalid allocation and must be resolved before downstream
            creation.

            Save and history rules
            Header and line edits auto-save after a valid commit. Calculation or receiving commands first commit pending editors.
            A newly fetched rate may prefill only eligible new unsaved/session-new order data. Saved purchases, received Inventory,
            Material cost history, Usage and saved Quotes are never automatically repriced or recalculated by later ECB/Settings data.
            """,
            "purchase order fields", "order header", "ordered item", "invoice total", "tax treatment", "shipping allocation",
            "manual shipping", "landed per unit", "landed per kg", "rate source", "rate date"),
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
            atomically reduce remaining weight. Editing a spool that is already referenced by Usage updates that same stable spool
            identity. Deleting a referenced spool is blocked, rolls back the transaction and reloads the last saved Inventory state.
            """,
            "inventory", "add spool", "duplicate", "delete", "refresh inventory", "show empty", "only opened",
            "low stock", "archived", "remaining", "estimated value", "validation", "usage"),
        new(
            "inventory.controls-fields",
            "Purchasing and cost",
            "Inventory controls and fields",
            "Every Inventory filter, command, editable spool field and calculated summary column.",
            """
            Commands
            Add Spool creates one new record and uses Material defaults only when that Material has no existing spool. Duplicate copies
            the selected spool into a new identity for explicit review. Delete Spool is permanent/default-No for that Inventory record
            but does not delete its Material or Usage history. A spool referenced by canonical Usage history cannot be deleted; the
            database rolls back, Inventory reloads and the app remains open. The warning identifies its Spool ID and MaterialID. Usage
            is an immutable ledger rather than a deletable log: open Usage and select that MaterialID to see its rows because the ledger
            shows only the currently selected Material. Refresh Inventory commits valid pending edits, recalculates summaries and
            synchronizes Materials quantity projection. Clear Filters restores the normal scope and changes no data.

            Search and filter controls
            Find searches Material, Spool ID, supplier, storage, batch and order. Show Empty defaults on and includes Empty rows. Only
            Opened restricts Status to Opened. Low Stock selects rows below 20 percent remaining. Show Archived Materials includes
            spools whose Material is archived. Filters combine and affect display only.

            Editable spool columns
            Spool ID is generated/read-only. Material selects the canonical MaterialID. Status choices are Unopened, Opened and Empty.
            Qty is a positive whole-number spool/group count. Spool g and Remaining g / spool are grams; remaining must be from zero
            through spool weight. Storage, Batch, Supplier, Order and Notes are editable references. Price / spool is non-negative and
            Currency is its governed currency code. Purchase date is optional date evidence. Valid committed cells auto-save.

            Calculated summary grid
            Material, Spool ID and Status identify the source record. Qty, Remaining and Remaining percent summarize stock. Estimated
            Value and Cost/kg are calculated from retained spool cost evidence. Validation / Review names missing or contradictory
            quantity, weight, storage or price facts. These summary cells are read-only even when XAML column-level metadata inherits
            from the read-only grid.

            Downstream rules
            Purchase receiving may create Unopened spool rows. Usage may consume only a same-MaterialID spool with sufficient remaining
            grams and decrements it atomically with the accepted Usage event. Later currency/catalog changes never rewrite retained
            purchase or Inventory provenance. A spool created from a calculated v53 Purchase Order retains separate invoice purchase
            amount/currency and landed amount/currency plus the snapshotted conversion rate, rate source, observation/fetch evidence,
            calculation UTC and calculation version. Those provenance values are read-only historical evidence even though the normal
            editable spool grid shows only the owner-editable purchase price/currency fields. Legacy rows retain their explicit legacy
            provenance and are never silently upgraded or recalculated by opening, refreshing or editing Inventory.
            """,
            "inventory fields", "show empty", "only opened", "low stock", "spool id", "spool grams",
            "remaining grams", "price per spool", "estimated value", "cost per kg"),
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
            "usage.controls-fields",
            "Purchasing and cost",
            "Usage controls and fields",
            "Every Usage input, selector, action, total and read-only ledger column.",
            """
            Event selectors and inputs
            Material selects one active canonical MaterialID. Inventory spool is optional and lists only matching Material spools.
            Type selects the governed event type. Provenance identifies how the evidence was obtained. Occurred UTC accepts the event
            timestamp in UTC. Filament g is a non-negative fixed gram amount. Print minutes and Hands-on minutes are non-negative
            durations. Produced, Accepted and Rejected are non-negative whole counts and Accepted plus Rejected cannot contradict
            Produced. Source identifies the originating job/system; Note records concise supporting context.

            Record and validation
            Inputs remain drafts until Record Usage is clicked. Record Usage validates required Material, timestamp, values, counts,
            provenance and optional spool ownership/capacity. It commits one accepted event; when a spool is selected, the same SQLite
            transaction decrements Remaining g / spool. Failure changes neither ledger nor Inventory.

            Correction actions
            Correct Selected loads an accepted ledger event into correction mode. The original remains read-only. Committing creates
            an exact reversal plus replacement; MaterialID cannot change and an already reversed event cannot be reversed again.
            Cancel Correction abandons only the correction draft and changes no accepted data.

            Totals and ledger columns
            Effective event count, net filament grams, print/hands-on time, produced/accepted/rejected totals and evidence coverage are
            read-only. Ledger columns Occurred UTC, MaterialID, Type, Entry, Inventory spool, Filament g, Print sec, Hands-on sec,
            Produced, Accepted, Rejected, Provenance, Source, Note and Event ID are all read-only history. Seconds in the ledger are the
            stored normalized form of minute inputs. Entry distinguishes accepted, reversal and replacement rows.
            """,
            "usage fields", "occurred utc", "filament grams", "print minutes", "hands-on minutes", "produced",
            "accepted", "rejected", "source", "event id", "entry"),
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
            "printers.controls-fields",
            "Costing and quoting",
            "Printers controls and fields",
            "Every Printer command and governed rate field with validation and quote handoff.",
            """
            Commands
            Add Printer immediately creates a draft identity. Duplicate Printer copies the selected machine/rate assumptions under a
            new ID for review. Archive / Restore toggles Active without deleting history. Delete Printer is permanent/default-No and
            is blocked while saved quote snapshots reference it. Save Printers explicitly validates and persists all current rows;
            valid committed cell edits also auto-save.

            Identity and lifecycle columns
            Printer ID is generated/read-only. Name is required and identifies selector output. Manufacturer and Model are editable
            descriptive fields. Active controls whether the Printer is offered to new quote drafts. Notes records governed assumptions.

            Cost and capacity columns
            Currency is the currency for Purchase cost, Upfront cost and Annual maintenance. These amounts are non-negative. Purchase
            cost is acquisition evidence; Upfront cost is the governed capital basis used by the rate model. Life (years) must be
            positive. Uptime percent is from 0 through 100 and represents usable capacity. Power W is non-negative electrical load.
            Buffer override is optional/non-negative and replaces the governed default buffer only for this Printer.

            Validation and downstream behavior
            Status text reports missing/invalid fields and hourly-rate readiness. Fix source inputs before selecting the Printer for a
            quote. New quote drafts use the current active Printer and Settings assumptions prospectively. Existing saved quotes retain
            their Printer/rate snapshots and are never automatically updated.
            """,
            "printer fields", "purchase cost", "upfront cost", "annual maintenance", "life years",
            "uptime percent", "power watts", "buffer override", "active printer"),
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
            "print-job-quotes.controls-fields",
            "Costing and quoting",
            "Print Job Quotes controls and fields",
            "Every quote draft input, selector, calculation output and saved-history action.",
            """
            Customer and currency controls
            Customer identifies the recipient; Description identifies the job; Prepared by identifies the estimator. Quote currency
            selects the final presentation currency. These are draft values until Save Quote succeeds.

            Material-cost controls
            Material selects canonical retained cost evidence. Use explicit manual cost per kg switches from that evidence to Manual
            cost/kg plus its Manual currency; disabled manual fields are ignored. Grams per part must be positive and Quantity must be
            a positive whole number. Material evidence text is read-only and states provenance, currency and conversion used.

            Printer, time and commercial inputs
            Printer selects an active governed rate source. Print hours, Print labor minutes, Consulting minutes and Design/change
            minutes are non-negative durations. Additional cost is a non-negative ISK amount. Target margin is the governed percentage
            input and must remain within the displayed validation range. Calculation output is a prospective read-only draft summary;
            changing an input refreshes the draft but does not alter history.

            Saved-history controls
            Save Quote validates all required draft inputs and persists the complete calculation/material/printer/rate/Settings
            snapshots. The history grid is wholly read-only: Quote, Created UTC, Customer, Material snapshot, Printer snapshot, Final
            price and Currency describe the saved record. Export Selected PDF requires one selected saved row and renders from that
            snapshot. Delete Selected permanently removes the selected saved quote after default-No confirmation; there is no archive.

            Historical boundary
            Later Material, Inventory, currency reference, Settings or Printer changes never automatically recalculate saved quotes.
            To offer changed assumptions, calculate and save a new quote or explicitly delete an obsolete/test quote.
            """,
            "quote fields", "quote currency", "manual cost per kg", "grams per part", "quantity", "print hours",
            "additional cost", "target margin", "created utc", "final price", "export selected pdf"),
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
            BaseMaterialID relationships. Delete is automation-blocked and refused before confirmation while any Material references
            the ID. An unreferenced row opens a named Yes/No warning with No focused by default. Choose No, press Escape or close the
            warning to preserve the catalog, current selection and dependent calculations. Only explicit Yes deletes that one ID.

            Downstream meaning
            Profiles are controlled 3DPIceland test-print baselines consumed by Material Detail and testing workflow, not manufacturer
            recommendations.
            """,
            "base material", "add", "duplicate", "delete", "reset columns", "bind exact", "nozzle", "bed",
            "speed", "cooling", "drying", "profile", "relationship"),
        new(
            "base-materials.controls-fields",
            "Materials and catalogs",
            "Base Materials controls and fields",
            "Every catalog command and controlled profile column with units, choices and relationship effects.",
            """
            Commands
            Add Base Material creates and immediately saves a unique draft family. Duplicate copies the selected profile under a new
            identity. Bind Exact Material Names previews only unique exact unlinked names and requires confirmation. Delete is permanent
            and automation-blocked. Referenced IDs are refused before any destructive prompt. An unreferenced row requires a named
            Yes/No warning with No as the default; No, Escape or closing the warning preserves SQLite, selection and calculations.
            Reset Columns confirms before restoring local order/width only. Valid grid edits auto-save.

            Identity fields
            Base Material is the required unique canonical family name. Category is the governed grouping. Sort Order is numeric display
            order. A committed canonical rename propagates the display-name snapshot to linked Materials without changing their IDs.

            Temperature, speed and cooling fields
            Nozzle min/recommended/max, Bed min/recommended/max and Drying temperature are degrees Celsius. Print Speed
            min/recommended/max is mm/s. Cooling min/recommended/max is percent from 0 through 100. Each minimum must not exceed its
            recommended value, and recommended must not exceed maximum. Drying hours is non-negative. Cooling guidance choices are Off,
            Low, Moderate, High and Required.

            Enclosure and profile references
            Enclosure choices are Not required, Recommended, Required and Heated chamber recommended. Printer / G-code reference and
            Slicer profile reference identify controlled baselines. Profile ID is the external/internal profile identifier. Profile
            kind is Slicer provided, Manufacturer provided or User provided. These are 3DPIceland controlled test-print baselines;
            they are not automatically manufacturer recommendations.
            """,
            "base material fields", "nozzle minimum", "nozzle recommended", "bed temperature", "print speed",
            "cooling percent", "cooling guidance", "drying temperature", "drying hours", "enclosure", "profile kind"),
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

            Document branding
            The Document Branding group owns an optional Brand / Organization Name and PNG for generated documents only. Save Brand Name
            trims repeated whitespace and immediately stores 1–80 visible characters in this SQLite database. Select PNG validates and
            immediately stores a governed normalized copy; Save Settings is not required and the selected source file is never changed
            or remembered by path. Preview is read-only. Newly generated reports, report website documents and customer quote exports
            use the current name and logo. Existing files are not rewritten automatically. Application chrome, icons and canonical
            public-site branding are never changed. Generated documents retain a smaller 3DPIceland Engineering Platform provenance.
            """,
            "settings", "value", "measurement", "calculation", "currency", "purchasing", "deployment", "FTPS",
            "save settings", "reload", "restore defaults", "reset columns", "prospective", "document branding", "PNG"),
        new(
            "settings.controls-fields",
            "Configuration",
            "Settings Manager controls and fields",
            "Every Settings column and command, including prospective effects, defaults and secret boundaries.",
            """
            Grid columns
            Section groups ownership. Parameter is the stable governed setting name. Value is the only editable column and accepts the
            type/range described by Unit and Notes. Unit states measurement/currency/time meaning. Used By lists downstream consumers.
            Notes explains validation/default behavior. Section, Parameter, Unit, Used By and Notes are read-only.

            Save and reload commands
            Save Settings validates and writes General plus Deployment values; separately governed Base Material rows keep their own
            save path. Reload Saved Settings is default-No because it discards unsaved General/Deployment editor values and reloads
            SQLite; it does not change Base Materials. Restore Built-in Defaults is default-No, replaces and immediately saves General
            rows only, and leaves Deployment and Base Materials unchanged. Reset Columns confirms and changes local Settings/Base
            Material layout only.

            Measurement and calculation values
            Measurement constants govern native/experimental calculations and validation. Calculation/Printer values govern
            prospective machine, labor, energy and buffer results. Use the displayed Unit and Notes for exact allowed range; invalid
            values block Save and retain the last accepted SQLite value.

            Currency and purchasing values
            Governed currency values are the offline/manual fallback and remain owner-editable. ECB is optional reference data for
            eligible new Purchase Orders only. `Default Landed Cost Currency` accepts a governed currency code and is copied only when
            a new Draft is created; invalid/missing values safely fall back to ISK for that new Draft. Changing or saving Settings never
            rewrites an existing Purchase Order, received Inventory, Usage history or saved Quote.

            Deployment and FTPS values
            Deployment rows govern local output/publishing configuration. Password secrets are never stored in the Settings grid or
            SQLite; FTPS credentials use Windows Credential Manager. Save Settings does not publish, upload, generate Production or
            apply application updates. Those remain separately confirmed actions.

            Optional AI provider foundation
            Provider selects Local deterministic or the optional OpenAI foundation. Pinned model is a non-secret preference outside
            SQLite. Save / Replace Credential writes the masked API key only to Windows Credential Manager; Delete Credential is
            default-No. Test Provider Foundation checks configuration and credential presence locally. It sends no network request or
            material payload. The separate AI Assistant pilot requires exact payload preview and one-time consent before a live request.

            Document Branding controls
            Go to the actual Settings Manager tab and use Document Branding — generated documents only; do not search for these buttons
            inside Help. Brand / Organization Name accepts 1–80 visible characters. Save Brand Name trims leading/trailing whitespace,
            collapses repeated whitespace, saves immediately and survives restart, backup and restore. The default and invalid-state
            fallback are 3DPIceland Labs.

            Select PNG accepts one real PNG up to 5 MiB, with width and height from 16 through 4,096 pixels and no more than
            16,000,000 decoded pixels. It fully decodes, strips metadata by normalization, stores the governed bytes and SHA-256 in the
            current SQLite database and materializes a derived fixed-name cache. The source file and source path are not changed or
            retained. Selection saves immediately and survives restart, backup and restore independently of Save Settings.

            Preview opens a larger read-only logo window showing the current Default, Custom or Fallback state; Close, Escape and the
            window close button return to Settings without changing data. Restore Default defaults to No and, after Yes, clears both
            the custom name and custom logo without changing other Settings. Escape and the dialog close button act like No.
            Missing or corrupt custom state reports Fallback and uses the built-in logo. HTML reports and customer quotes embed the
            normalized selected PNG. Report website/PDF packages keep the stable
            `assets/3dp-iceland-labs-logo-pdf.jpg` route by creating a white-background JPEG derivative from the same immutable
            selection; native PDF evidence uses that exact derivative rather than injecting PNG bytes into a JPEG object. Existing
            generated files and saved quote calculation snapshots are not rewritten. The selected name replaces prominent 3DPIceland
            document headings while a smaller Generated with 3DPIceland Engineering Platform line preserves platform provenance.
            These controls never alter the application icon, splash, window chrome or canonical public website logo/favicon.
            """,
            "settings fields", "section", "parameter", "value", "unit", "used by", "reload saved settings",
            "restore built-in defaults", "measurement constants", "currency fallback", "deployment", "credential manager",
            "brand organization name", "save brand name", "select PNG", "preview document logo",
            "restore default document branding", "fallback", "platform provenance"),
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
            "measurements.controls-fields",
            "Testing and engineering",
            "Native Measurements controls and fields",
            "Every Tensile, Impact and Stiffness custom-grid input, calculated column and layout action.",
            """
            Shared scope and save behavior
            All three native grids show the MaterialIDs currently visible through Materials search and filters. Identity columns
            Material ID, Manufacturer, Product Line, Marketing Name, Base Material, Category, Variant / Finish, Reinforcement and Color
            are read-only here. A valid committed source edit recalculates through ResultsService, updates test status and auto-saves
            SQLite. The first source measurement assigns today only when Measured date is blank. Test Notes and Measured date are
            editable. Reset Columns confirms before restoring only that grid's local width/order; it changes no measurements.

            Tensile columns
            Upright 1-10 and Flat 1-10 are force samples in N. Each accepts blank or a number from 0 through less than 505. Test Notes
            and Measured date are editable. MPa Upright/Flat, standard deviation, CV percent, sample counts, confidence and Validation
            are calculated/read-only. Upright represents the layer-load orientation and Flat represents the separately governed flat
            orientation; never type calculated MPa into a source-force cell.

            Impact columns
            Upright percent 1-10 and Flat percent 1-10 are needle/pendulum percentage inputs from 0 through 100. Test Notes and Measured
            date are editable. kJ/m² Upright/Flat, standard deviation, CV percent, sample counts, confidence and Validation are
            calculated/read-only using governed impact Settings. Correct source percentages or Settings at their owner, not the energy
            result. The governed public-demo builder restores only its hash-pinned legacy decimal encodings before runtime, so every
            fictional demo row follows this same 0-100 validation contract; owner/canonical measurements are never rewritten.

            Stiffness columns
            Revolutions accepts 0 through 10 and Degrees accepts 0 through 359; together they form one source observation. Test Notes
            and Measured date are editable. Deflection mm, Modulus MPa and Validation are calculated/read-only. A blank/incomplete pair
            remains incomplete rather than inventing a result.

            Validation and historical boundary
            Invalid text/ranges are rejected and the prior accepted value remains. Validation names the source issue. Editing native
            measurements refreshes current canonical results and downstream read-only views; it does not rewrite Experimental Runs,
            saved reports, purchases, Inventory, Usage or saved Quotes.
            """,
            "native measurement fields", "upright 1", "flat 10", "force samples", "impact percentage",
            "revolutions", "degrees", "measured date", "reset columns", "calculated read-only"),
        new(
            "experimental.controls-fields",
            "Experimental testing",
            "Experimental Testing controls and fields",
            "Every Series, Run, measurement-editor and Results-table control with lifecycle, units and comparison scope.",
            """
            Series actions and filters
            Find searches Series identity/content. Active only defaults on and filters the Series grid without changing Active state.
            Clear Filters clears Find and restores normal visibility. Add Series creates a new identity and prefers the selected active
            Material. Duplicate Series creates a new graph identity and resets Website publication. Delete Series is permanent,
            confirmation-guarded and removes its child Runs/measurements.

            Series columns
            Series ID and Updated UTC are read-only. Material selects canonical MaterialID. Experiment and Default Unit define the
            controlled variable. Baseline Material is optional comparison context. Website is an explicit publication choice: enabling
            it checks readiness and defaults incomplete confirmation to No; disabling hides without deleting history. Active controls
            normal scope. Series Notes is editable. Valid commits auto-save.

            Run actions and columns
            Add Run creates an Active, Planned, non-baseline row. Duplicate Run creates a clean Planned, non-baseline identity and
            clears Measured date. Delete Run is permanent/default-No for that Run graph. Run ID is read-only. Value and Unit describe
            the controlled setting. Status is the governed lifecycle; Measured date records completion evidence. Baseline is unique:
            selecting one Run clears another baseline in the same Series. Active preserves/hides history. Run Notes is editable.

            Include inactive history
            Include inactive history in comparison changes only eligible Results scope. It neither changes the Run grid nor reactivates
            or publishes anything. Inactive incomplete Runs may still be ineligible. Results always remain selected-Series scoped.

            Run measurement editors
            Tensile N 1-10 and Impact percent 1-10 are editable source samples for Upright and Flat rows. Orientation is read-only.
            Stiffness Revolutions and Degrees are the editable paired source. Notes is editable. Average MPa/kJ/m², Result MPa,
            deviation, CV, Count, Confidence and validation are calculated/read-only. Valid commits auto-save the Series graph and
            refresh Results without changing native Material measurement rows.

            Results views
            Dashboard cards, Table and Charts are fully read-only. Table columns Run, Status, controlled value/unit, Tensile/Impact/
            Stiffness metrics, Overall, delta to Baseline, CV, Rank and Baseline describe calculated comparison output. XAML
            column-level metadata does not make this read-only grid editable. Missing baseline or coverage remains visibly unavailable.
            """,
            "experimental controls", "series fields", "run fields", "include inactive history", "baseline unique",
            "experimental n 1", "experimental percent", "results table", "delta to baseline", "read-only results"),
        new(
            "material-detail.controls-fields",
            "Material detail",
            "Material Detail interactive controls and fields",
            "Analytics, Compare, Video Planner and Recommendation controls, grids, local persistence and read-only output.",
            """
            Read-only Material Detail views
            General, Printing Profile, Mechanical and Charts follow the selected MaterialID and are read-only projections. Edit their
            source at Materials, Base Materials or native Measurements. Notes remains unavailable as its existing topic states.

            Analytics
            Chart Mode chooses the visible-scope grouping. Select one row or Ctrl-click multiple Analytics Results rows to control the
            radar overlay. Clear radar selection clears local selection only. Group, Count, Tensile, Impact, Stiffness, Consistency,
            Layer Adhesion, Overall Profile and Radar are read-only calculated display columns.

            Compare
            Material A-D selectors choose up to four canonical Materials. Use Selected copies the current Materials selection into the
            intended comparison slot; it changes no source record. Metric/Score rows and the A-D value columns, winners and deltas are
            read-only. Missing incomparable results remain blank/unavailable.

            Video Planner filters and generated lists
            Manufacturer, Base Material and Category selectors combine with No YouTube video and Mechanical data only. Refresh rebuilds
            candidates from the current canonical scope; Clear Ideas removes the current local editable idea set only after its normal
            confirmation. Candidate/comparison lists are read-only: Material(s), Source, Reason, scores, standout data, suggested
            title/hook, talking points, winner and comparison text explain why an idea was proposed.

            Editable Video Planner idea fields
            In the editable recommendation idea list, Suggested title, Hook / angle, Talking points, Series, Episode order, Target week,
            Publish date, Status, Effort and Notes are local creator-planning values. Material and Reason remain read-only identity/
            evidence. Valid committed edits persist through the local Video Planner owner; they never alter engineering measurements.
            Copy Prompt copies the displayed local prompt to the Windows clipboard and makes no network request.

            Recommendations
            Use case, Category and Base Material selectors define the recommendation projection. Refresh recalculates read-only
            Performance, Application and Alternative lists from canonical results. Their Material, place, score, use case, why,
            trade-off, gain, MSRP/value and suggested creator-text columns are read-only even where XAML column metadata inherits.
            The ChatGPT Prompt box is read-only local text. Send to Video Planner creates/updates only the local planning handoff after
            explicit action; it never saves a recommendation as engineering truth.
            """,
            "material detail controls", "analytics chart mode", "clear radar selection", "compare material a",
            "video planner fields", "target week", "publish date", "copy prompt", "recommendation filters"),
        new(
            "analysis.controls-fields",
            "Analysis and decisions",
            "Rankings, Category Rankings, Awards and Insights controls and fields",
            "Every global analysis filter, row-limit, refresh/export action and read-only output column.",
            """
            Shared scope and actions
            Materials search/filters provide the upstream visible MaterialID scope. Each analysis surface adds its own selectors.
            Refresh rebuilds the read-only projection from canonical results; it saves no measurements. Reset Filters restores that
            surface's defaults. Export CSV writes the currently displayed governed rows to a chosen local file and changes no data.

            Rankings Dashboard
            Metric selects the rank basis. Manufacturer, Base Material and Reinforcement narrow scope. Rows defaults to Top 25 and also
            offers Top 10, 50, 100 and All. The read-only grid shows rank number, Material, Manufacturer, Type, Reinforcement, Tensile,
            Impact, Stiffness, Consistency, Layer Adhesion, Overall, Rank Score, Best Axis and Status. Rows missing the selected metric
            are omitted rather than assigned invented scores.

            Category Rankings
            Group chooses category grouping and Metric chooses score basis. Manufacturer, Base Material and Reinforcement narrow scope.
            Rows per group defaults to 10 and offers 5, 50, 100 and All. The read-only grid shows group/category, within-group rank,
            Material, Manufacturer, Type, Reinforcement, Overall, Category Score, Best Axis and Status.

            Awards and Winners
            Award Set selects the governed award family. Manufacturer, Base Material and Reinforcement narrow scope. The read-only grid
            shows Award, Type, Use Case, Winner, Runner Up, Manufacturer, Reinforcement, Score, Why and Status. Refresh applies existing
            award rules; users cannot type or appoint a winner in this grid.

            Dashboard Insights
            Insights has no editable field. It summarizes current database/visible-scope counts, highest metrics and narrative findings.
            Refresh ownership follows the surrounding canonical projection. Insight text is interpretation, not a measurement or saved
            approval, and never changes source rows.
            """,
            "rankings controls", "top 25", "rank score", "category rows per group", "award set",
            "runner up", "export csv", "dashboard insights", "read-only analysis"),
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

            AI Assistant keeps its local deterministic workflow and adds an optional guarded OpenAI pilot. Review the visible MaterialID
            scope and exact outbound payload before any external request.

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

            Governed public-demo acceptance keeps every Public reports and Public test details choice off. In the actual Reports / PDF
            Export tab, choose Material Summary Report, All Visible Materials, Refresh Preview and Export Current Report. This writes
            fictional demo identities only to the disposable output folder. Customer-facing Material Summary HTML/PDF omits
            MaterialID while its internal metadata and manifest retain canonical traceability. An empty public-build result is expected
            because demo acceptance never opts records into publication or authorizes Production/FTPS.
            """, "reports", "PDF", "template", "scope", "preview", "export", "public builds"),
        new(
            "reports.controls-fields",
            "Output and publishing",
            "Reports controls and fields",
            "Every report selector, folder field, preview/export action, public-build command and read-only evidence output.",
            """
            Report selectors and folder
            Report template selects one of the twelve governed engineering/purchasing templates. Report scope selects the current
            MaterialID or the current Materials Visible projection where supported. Output folder is an editable local path. Choose
            Folder changes that path; Open Folder opens it in Explorer and writes nothing. Verify scope and the full path before
            generation because same-named local artifacts may be replaced.

            Current report actions
            Refresh Preview builds the selected read-only canonical report model and writes no source data. Export Current Report
            validates the same template/scope and writes its governed HTML/PDF/text/metadata/manifest artifacts locally. Export
            Engineering Package builds the six accepted engineering report families as one local indexed package; it is distinct from
            the selected purchasing report. None of these actions perform FTPS or publish a website.

            Public build actions
            Build Public Material, Comparison, Manufacturer, Test Session, Printing Recommendation and Material Summary Reports each
            use their own explicit MaterialID/publication rules; Report template/scope do not control them. Public Test Sessions may
            expose eligible raw detail only when Public test details allows it. Build Public Report Package verifies/ensures all six
            eligible families and creates the local canonical portfolio handoff. Building locally uploads nothing.

            Preview and status output
            Report Preview/Log and the blue summary/status line are read-only. They identify model validation, scope, output paths and
            generated evidence. A successful log proves structure/generation only; visually inspect representative HTML/PDF for
            wrapping, pagination, charts and customer/public fields before external use. Material Summary HTML/PDF omits MaterialID
            from customer-facing content while governed metadata retains internal traceability.

            Failure and historical boundaries
            Missing selection/data, validation or output-folder errors block the requested action and do not repair measurements.
            Reports consume canonical results and saved snapshots where their model requires them. They never recalculate raw
            measurements, rewrite purchases/Inventory/Usage/Quotes or authorize Website Production/FTPS.
            """,
            "report controls", "report template", "report scope", "output folder", "refresh preview",
            "export current report", "engineering package", "public report package", "preview log"),
        new(
            "website.controls-fields",
            "Output and publishing",
            "Website Export controls and fields",
            "Every folder/template/credential control and the separate Preview, Production, FTPS and restore actions.",
            """
            Folder and template controls
            Website root folder is the editable local root containing the generated index location. Choose Folder stores that path;
            Open Folder only opens it. Template versions selects a SQLite-owned stored version. Import HTML validates/stores/activates a
            new master template; never import generated index.html or index-test.html as the template. Activate Selected explicitly
            makes the chosen stored version canonical. Export Active Template writes a review/backup copy only.

            Generate Preview
            Generate Preview validates the active template/data, ensures eligible public reports and writes index-test.html,
            export-manifest.txt and a timestamped log. It does not replace index.html, create Production, transfer files or change
            business data. Inspect the local preview manually.

            Generate Production
            Generate Production is a local guarded write. It validates, presents a default-No confirmation, backs up existing
            index.html and creates the new local index plus publish plan. It does not perform FTPS. Stop on any validation, backup or
            generation failure.

            FTPS credential and connection
            Password is a transient masked input. Test Connection validates explicit-TLS/passive connectivity and may retain the
            accepted secret in Windows Credential Manager; it never stores the password in SQLite, logs or website artifacts.
            Connection success does not publish.

            Publish Test and Publish Production
            Publish Website Test regenerates/transfers only the isolated Preview route and is not Production evidence. Publish Website
            Production first regenerates governed Production, then requires a second default-No live FTPS confirmation. It transfers
            the governed changed set and root index last. Independently inspect the live HTTPS site after success.

            Restore and evidence
            Restore Last Production Backup is live website recovery, not SQLite recovery. It requires confirmation, a completed
            governed backup and retained transfer/recovery evidence. Export Log is read-only and must identify paths, stage, success/
            failure and transfer results without secrets. Preview, Production, FTPS and Restore are four separate contracts.
            """,
            "website controls", "template versions", "generate preview", "generate production", "password",
            "test connection", "publish website test", "publish website production", "restore production backup"),
        new(
            "ai.controls-fields",
            "Assistant",
            "AI Assistant controls and fields",
            "Every local prompt, session, collection, coverage and dashboard control with explicit persistence boundaries.",
            """
            Scope and prompt controls
            Refresh Scope rebuilds the current visible MaterialID preview and changes no data. Prompt Template selects a governed local
            template. Prompt Editor is editable local text. Generate from Template, Full Brief, Ideas, Comparisons, Hidden Gems,
            Recommended Comparisons and Recommended Next Video create advisory read-only output only; they do not call an external AI
            service or mutate engineering records. Assistant Output is read-only.

            Session controls
            Session Title names an explicit local planning session. Saved Sessions selects a stored session. Save Session writes the
            current local title/prompt/output; Load Session replaces the current local editor/output after selection; Refresh Sessions
            reloads the list. Delete Session is permanent/default-No for that local planning record only.

            Collection controls
            Collection Title and Collection Scope define a local MaterialID collection draft. Preview Collection shows the exact
            intended MaterialIDs and writes nothing. Save Collection explicitly persists it. Collections selector chooses a stored
            collection; Load Brief generates advisory output from it. Delete Collection is permanent/default-No for that local record.
            Status selector plus Apply Status changes only collection workflow state. Mark Published records the local planning status;
            it does not publish any external content.

            Coverage identity and dashboards
            Bind Exact Coverage Identity previews/commits only unique exact legacy identity matches. Clear Coverage removes the selected
            local coverage state after confirmation. Generate Collection Dashboard and Video Pipeline Dashboard build read-only local
            planning summaries. Their counts/status never change Material lifecycle, tests, reports or website publication flags.

            Data and network boundary
            Existing Generate actions remain local/deterministic. Preview OpenAI Payload builds and displays the exact request body
            without network use. Generate with OpenAI requires OpenAI provider/credential readiness, then opens a second full-payload
            review with an unchecked one-time consent. Cancel stops an active request. Copy Operational Evidence becomes available only
            after a live attempt and copies UTC timing, outcome, model/schema/hash, counts, tokens and request IDs. It never includes the
            credential, Authorization header, raw payload, raw response, planning note, material names or MaterialIDs. Only MaterialID,
            manufacturer, product line, marketing name, base material, category, variant/finish, reinforcement, selected template and
            the visible planning note are sent. Purchasing, Inventory, Usage, Quotes, costs, URLs, paths, credentials, internal notes and
            raw measurements are excluded.

            The request uses Responses API, store=false, no tools/files/web search and strict structured output. Unknown evidence
            MaterialIDs are rejected. Output stays advisory and is not saved unless Save Session is clicked explicitly. Provider
            retention terms may still apply even with store=false. No Assistant action may silently edit Materials, measurements,
            purchasing, Inventory, Usage, saved Quotes, reports or Website Production. Save Session rejects an exact outbound payload
            preview instead of persisting the raw request locally; generate and review an advisory result before saving a session.
            """,
            "assistant controls", "prompt editor", "generate full brief", "save session", "delete session",
            "collection title", "apply status", "mark published", "coverage identity", "local deterministic",
            "Preview OpenAI Payload", "Generate with OpenAI", "Cancel OpenAI Request", "Copy Operational Evidence",
            "store false"),
        new(
            "youtube.controls-fields",
            "Configuration and creator tools",
            "YouTube Research controls and fields",
            "Refresh-generated read-only research tables and the seven explicit clipboard actions.",
            """
            Refresh and source boundary
            Refresh Research rebuilds local creator suggestions from current canonical Materials, results and retained video metadata.
            It writes no engineering data and performs no YouTube/network search. All five tables are read-only even where XAML
            column-level metadata inherits from their grid definition.

            Title and thumbnail research
            Title candidates show Material, Manufacturer, Family, Type, Score, Coverage, Advanced title, Video angle and Thumbnail/hook
            angle. Thumbnail briefs show Material, Grade, Main text, Face/reaction, Visual layout and Why. These are generated planning
            suggestions, not saved engineering conclusions.

            Comparison, gap, calendar and playlist tables
            Comparison Discovery shows Material A/B, Comparison idea, Thumbnail text, Score and Why. Channel Gaps shows Gap, Pattern,
            Videos, Missing/next, Score and Why. Content Calendar shows Week, Video idea, Type, Manufacturer, Reinforcement, Why this
            week and Thumbnail. Playlist Discovery shows Playlist, Type, Video, Priority and Why. Every column is read-only.

            Clipboard actions
            Copy Research Prompt, Best Thumbnail, Thumbnail Briefs, Top Comparisons, Top Channel Gaps, Calendar Plan and Top Playlists
            copy the named current local text to the Windows clipboard. They do not open a browser, create calendar/YouTube records,
            upload, publish or save canonical data. Empty/unavailable output reports the limitation instead of inventing content.
            """,
            "youtube controls", "refresh research", "advanced title", "thumbnail brief", "comparison discovery",
            "channel gaps", "content calendar", "playlist discovery", "clipboard"),
        new(
            "menu-runtime.controls-fields",
            "Safety and support",
            "Application menu and runtime-window controls and fields",
            "Menu command destinations plus Recovery, Verification, Diagnostics, updater and document-viewer runtime controls.",
            """
            Menu containers and ordinary commands
            File, Materials, Navigate, Tools and Help are menu containers, not invoked actions. Navigate contains six workflow
            groups and 22 commands that select and focus existing top-level tabs. Navigate never saves, recalculates, restores, exports,
            publishes or changes filters by itself. Opening a destination can run the same read/refresh or lazy-load behavior as clicking
            its visible tab.

            Materials Add/Duplicate/Archive/Unarchive/Delete/Clear Search/Clear Filters invoke the same governed tab workflows and
            confirmations described in Materials Help. Navigate > Publishing > Website Export only selects that tab and never generates
            or publishes. Validate Materials is read-only validation; Rebuild Computed Fields is mutating.

            Runtime profile identity
            The main header always names the active ownership contract. OWNER / PRODUCTION uses the configured owner database and
            owner preferences and retains separately guarded Production/FTPS/update capabilities. VERIFICATION / DISPOSABLE uses
            manifest-contained database, preferences and output roots; owner database, Production/FTPS and general updates are blocked.
            CLEAN / READINESS is a seedless disposable first-run contract. It creates a new empty SQLite database only inside its
            manifest root, reports data-dependent checks as Not applicable and keeps release, identity and security failures mandatory.
            Verification always shows runtime identity separately from the data profile. Mandatory checks can never become Not applicable;
            only explicitly classified CanonicalDataDependent checks may do so when no canonical Materials exist.
            Disposable profiles never read owner credentials or owner update-transaction history. Their database, preferences, output,
            evidence and cleanup ownership remain manifest/runner-contained. Cleanup is runner-owned and defaults to a reviewable
            dry-run plan. Apply requires that exact plan SHA-256 and removes only older valid unpinned PASS profiles. The latest required
            PASS per scenario plus every FAIL, aborted, malformed or pinned acceptance dependency is retained. Reparse points, path
            drift and the automation root itself are always blocked.
            Profile identity never grants an action by itself and never weakens crash, recovery, security or support evidence.

            File, update and publishing commands
            Backup and Recovery Center opens the runtime recovery window. Choose Storage Folder is a guarded storage mutation; Open
            Storage Folder is read-only navigation. Update Readiness and Check for Updates are read-only discovery until a separately
            confirmed download/apply. Publish Application Release and Publish Application Update build/upload application artifacts
            under their own guarded authority and are distinct from Website publishing. Standard automation blocks them.

            Help/document commands
            Documentation opens the whole-system overview; Help for Current View/F1 resolves the selected tab. Whitepaper and Changelog
            open generated/document viewers; About shows release/storage/license identity. Verification Center and System Diagnostics
            open separate runtime windows.

            Recovery Center runtime controls
            Catalog status, selected-backup details and backup grid are read-only. Refresh and Verify Selected inspect only. Create
            SQLite/Excel backups write evidence. Restore Selected, Restore SQLite File and Restore Excel Backup are mutating,
            confirmation-guarded recovery actions. Open Storage Folder is navigation. Never infer restore authority from successful
            verification.

            Verification and Diagnostics runtime controls
            Report/output boxes and status labels are read-only. Refresh is read-only. Run Integrity Check reads SQLite integrity.
            Export Report writes evidence. Recalculate Native Results and Recalculate All Materials are mutating repair actions and are
            not routine verification. Automation Evidence export exists only in an authorized disposable profile.

            Updater and document viewer
            Guarded updater confirmations separate Download, Apply and Cancel; default-No cancellation changes no application files.
            The document viewer Search/Find Next and Version/Jump fields navigate read-only packaged text. The hidden HTML print-host
            WebView is supported internal report infrastructure but is not a user-facing Help control.

            Automation-only shell controls
            Eight hidden CRUD/recovery buttons exist only in an authorized disposable automation profile. They are unsupported as
            owner-facing UI and must never appear in normal operation. Their presence in source does not authorize owner-database,
            Production, FTPS, update or recovery mutations.
            """,
            "application menu controls", "recovery center controls", "verification controls", "diagnostics controls",
            "recalculate mutating", "updater confirmations", "document viewer", "automation-only"),
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
            material counts, representative cards, report links and browser layout. Open Printing Price Calculator in the website
            navigation and confirm its live totals, Reset Defaults and quote-print preview. It is an independent browser calculator:
            it never reads Materials, SQLite, MaterialID, reports or Website DATA. The relative `price/` route redirects to the
            calculator tab after coordinated Production activation. Confirm the Labs wordmark is readable in desktop and narrow
            navigation and that the browser shows the canonical favicon. Missing branding assets or validation failures must be
            resolved before Production.

            Estimated Uptime is a percentage from 0 to 100: enter 50 for 50%, not 0.5. Capital cost per hour spreads printer purchase,
            upfront cost and lifetime maintenance across available lifetime uptime hours. Electrical cost per hour is then added and
            Printer Cost Buffer Factor applies to that combined hourly cost. A warning appears when job hours exceed available
            lifetime uptime hours.

            Preview success is local evidence only. Retain its manifest/log when diagnosing differences, but do not describe the live
            website as updated until a separately authorized Production transfer and independent live check succeed.
            """,
            "preview", "index-test.html", "export-manifest.txt", "ExportLogs", "public reports", "price calculator", "price/",
            "wordmark", "favicon"),
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
            "Local deterministic analysis plus an optional exact-preview, consent-gated OpenAI advisory pilot.",
            """
            AI Assistant works from the current visible MaterialID scope and local deterministic services. Review the exact scope preview
            before generating briefs. Sessions and collections are owner-managed local planning records, separate from engineering data.

            Generated output is advisory/read-only. Existing Generate actions stay local. Preview OpenAI Payload performs no network
            request and shows the exact JSON body, allowlisted IDs and SHA-256. Generate with OpenAI requires an additional unchecked
            one-time consent, store=false, no tools/files/web search and strict structured output. It never saves output automatically.
            Secret-safe operational evidence remains in memory until explicitly copied. Exact payload previews cannot be saved as
            sessions. Assistant output never replaces measurements or Verification.
            """, "AI Assistant", "visible scope", "sessions", "collections", "output"),
        new(
            "ai.visible-scope", "AI Assistant", "AI visible scope reference",
            "Review the exact visible MaterialID input before any analysis.",
            """
            AI Assistant uses the current Materials search/filter projection. Refresh Visible Scope rebuilds the scope summary and
            MaterialID preview, including row count, unique stable IDs, representative identifiers and a deterministic scope hash.
            Multi-select values use OR within one filter and AND across filters. Filtering Materials changes later briefs even though no
            engineering row is edited.

            Review the count and IDs before generating. Clear Materials filters for whole-database intent, or deliberately keep them to
            analyze a bounded family/manufacturer subset. Empty, duplicate or legacy identity warnings should be resolved or explicitly
            understood before trusting coverage conclusions.

            Refresh Visible Scope is read-only. It does not save a collection, change lifecycle status or contact an external service.
            The same canonical scope feeds the OpenAI pilot, which shows source, allowlisted and omitted counts for its governed
            forty-MaterialID limit. Local briefs similarly disclose their processed and omitted counts. Narrow Materials scope when the
            complete visible set must be processed, and review the exact payload before any separately consented live request.
            """,
            "visible scope", "Refresh Visible Scope", "MaterialID", "filters", "read-only"),
        new(
            "ai.planning-briefs", "AI Assistant", "AI planning briefs reference",
            "Generate deterministic local briefs from templates or named analysis actions.",
            """
            Template chooses Video Ideas, Comparison Planning, Thumbnail Hooks, Full Assistant Brief or Playlist Suggestions. Planning
            note is included as owner-authored context in local briefs. Generate From Template uses the selected template, while the
            named buttons generate Video Ideas, Comparisons, Recommended Next Video, Recommended Comparisons, Hidden Gems or the
            complete Full Assistant Brief. These existing actions remain local.

            The optional OpenAI pilot includes the selected template and planning note in its exact preview. Treat the note as outbound
            data: remove private information before approving the request.

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
            Materials. A collection stores stable MaterialID membership and an exact set hash rather than duplicate material records.
            Load Collection Brief reports active, archived and missing membership without rewriting the saved snapshot.

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

            Output may be a local rule-based result, an exact OpenAI request preview or a validated OpenAI advisory. The heading and
            request IDs identify external advisory output. Recommendations depend on the visible scope; unknown evidence MaterialIDs
            invalidate an OpenAI response instead of being displayed as trusted evidence.

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
            "Validate Materials and rebuild computed display fields through distinct read-only and mutating commands.",
            """
            Validate Materials checks the current Material records and reports missing or invalid governed identity without silently
            repairing rows. Rebuild Computed Fields is different: after confirmation it recalculates persisted computed Material display
            fields from their canonical inputs. Create a backup and review the validation result before using that mutating command.

            The former snapshot-only Materials Rendering Prototype command is retired. Supported owner-drawn rendering remains embedded
            in Materials, measurement, Settings and Base Materials tabs. Update, release-publishing and storage commands in the Tools
            menu have separate safety ownership and are documented in v50.3.
            """,
            "Tools", "Validate Materials", "Rebuild Computed Fields", "retired prototype", "mutating"),
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
            "menu.file-recovery", "Safety and support", "File and Recovery menu reference",
            "Open governed backup/recovery, choose storage deliberately or exit safely.",
            """
            Backup and Recovery Center opens the local backup catalog and guarded recovery actions. It does not restore anything merely
            by opening. Choose Storage Folder is a separate mutating action that moves the canonical SQLite database and changes future
            storage ownership. Exit closes the application after normal pending-save handling.

            Create and verify a backup before moving storage or restoring data. Never choose the canonical owner database as an
            automation target. Recovery and storage changes can affect the entire application, so confirm the exact source, destination
            and retained evidence rather than treating File-menu actions as ordinary navigation.
            """,
            "File menu", "Recovery Center", "Choose Storage Folder", "Exit", "canonical SQLite"),
        new(
            "menu.storage", "Safety and support", "Storage ownership reference",
            "Distinguish changing the canonical storage folder from opening it.",
            """
            Choose Storage Folder moves the current canonical database into an owner-selected folder and reopens storage from that
            location. It is not a folder shortcut. Back up first, select a stable local owner-controlled location and verify the
            resulting database path before continuing work.

            Open Storage Folder only opens the folder containing the active database. It creates the directory if missing but does not
            move or restore SQLite. Diagnostics reports the active database path and folder; use those values when confirming which
            profile is actually open.
            """,
            "storage", "move database", "Open Storage Folder", "database path", "backup"),
        new(
            "menu.updates", "Safety and support", "Application update menu reference",
            "Inspect signed packages, discover remote updates and apply only through the guarded updater.",
            """
            Update Readiness inspects an owner-selected signed ZIP for readable package, manifest, exact inventory, hashes, trusted
            signature, version policy and SQLite schema compatibility. Inspection is read-only, but a ready result offers a separate
            default-No guarded Apply confirmation.

            Check for Updates queries the governed HTTPS feed. Download requires confirmation and verifies bytes, hash, signature,
            version and schema before Apply is offered. Apply creates a verified SQLite backup and last-known-good application snapshot,
            closes the app and uses the external updater. SQLite is never silently restored.
            """,
            "updates", "readiness", "signed package", "Check for Updates", "default-No", "SQLite backup"),
        new(
            "menu.release-publishing", "Safety and support", "Application release publishing menu reference",
            "Publish governed installers or signed updates separately from Website publishing.",
            """
            Publish Application Release consumes a verified application-deployment-plan.json and requires default-No confirmation before
            live FTPS. Versioned installer/portable artifacts activate first; stable download routes activate last. A separate remote
            application-release backup is retained.

            Publish Application Update consumes a locally verified latest.json and publishes the signed package before activating
            /updates/latest.json last. Both actions require the governed stored FTPS credential. They do not change Website deployment
            manifests or SQLite and are distinct from Publish Website Production.
            """,
            "release publishing", "application deployment plan", "latest.json", "FTPS", "stable routes last"),
        new(
            "recovery.overview", "Safety and support", "Recovery Center reference",
            "Inspect local backup evidence and choose the correct guarded recovery path.",
            """
            Recovery Center loads a read-only SQLite backup catalog with file, type, modified time, schema, integrity, row counts and
            compatibility. Select a row to read its exact path and compatibility detail. Opening or refreshing the center changes no
            business data.

            Restore Selected, Restore SQLite Backup and Restore Excel Backup are destructive recovery boundaries. Verify the source,
            review warnings and accept the default-No confirmation only with explicit restore intent. Successful database restore
            creates pre/post evidence and restarts the application.
            """,
            "Recovery Center", "catalog", "compatibility", "restore", "restart"),
        new(
            "recovery.catalog-and-verification", "Safety and support", "Recovery catalog and verification reference",
            "Interpret backup compatibility and run isolated migration verification.",
            """
            Refresh inventories local SQLite backups. Ready means integrity/schema and canonical Material structure are accepted.
            Ready — empty profile is healthy and explicitly restorable but contains zero Materials. Migration required needs the
            isolated dry-run; legacy/incomplete, newer/incompatible and corrupt/unreadable remain blocked.

            Verify Selected runs compatibility and supported migration checks without replacing the active database. A PASS makes that
            exact candidate eligible; it does not prove the backup contains the business data you intend. Confirm Material and
            measurement counts plus the full path before Restore Selected.
            """,
            "backup catalog", "Verify Selected", "empty profile", "migration required", "blocked"),
        new(
            "recovery.sqlite-backup-and-restore", "Safety and support", "SQLite backup and restore reference",
            "Create exact local backups and restore only verified compatible SQLite candidates.",
            """
            Create SQLite Backup writes and verifies a manual backup of the active canonical database. Preserve it in the governed
            backup location before risky owner actions. Restore Selected uses the catalog candidate; Restore SQLite Backup lets the
            owner select an external SQLite file and then applies the same compatibility guards.

            Restore creates a verified pre-restore recovery backup, replaces the active database, validates the result, records a
            post-restore evidence backup and restarts. A healthy empty-profile restore explicitly replaces current business data with
            zero Materials. Cancellation or failed verification leaves restore blocked.
            """,
            "SQLite backup", "Restore Selected", "pre-restore", "post-restore", "empty profile"),
        new(
            "recovery.public-demo-install-and-remove", "Safety and support", "Public demo install and removal",
            "Evaluate the public demo through guarded SQLite restore and return safely to the exact prior profile.",
            """
            Download the governed public demo ZIP from https://www.iskort.is/3dp/downloads/3DPIceland-Public-Demo.zip and extract its
            versioned .sqlite member. Open File > Backup and Recovery Center, choose Restore SQLite Backup and select that extracted
            file. Review the default-No confirmation carefully. A successful restore verifies the database, creates a pre-restore
            recovery backup, replaces the active database and restarts with 36 fictional demo Materials and real disclosed measurements.

            To remove the demo, reopen Backup and Recovery Center and choose Refresh. Select the Pre-SQLite restore recovery row whose
            timestamp, full path and row counts match the restore that installed the demo. Choose Verify Selected, then Restore Selected,
            review the default-No confirmation and choose Yes only for that exact backup. It restores the prior owner data, or the
            healthy zero-Material profile when the demo was installed into a new empty profile, and restarts.

            The current governed demo is v56.0.6.1. Its explicit public AppMeta marker lets Verification classify only the 12
            intentionally excluded owner Website-publication checks as Not applicable. Security, paths, recovery, installer and every
            other check remain mandatory.

            Never overwrite or delete the active filamentdb.sqlite manually. Choose Storage Folder moves the current canonical database;
            it does not switch between demo and owner datasets.
            """,
            "public demo", "Restore SQLite Backup", "Pre-SQLite restore recovery", "remove demo", "36 Materials"),
        new(
            "recovery.excel-disaster-recovery", "Safety and support", "Excel disaster-recovery reference",
            "Use governed Excel packages only as explicit disaster recovery, never as the live source of truth.",
            """
            Create Excel Backup exports the governed recovery workbook/package from canonical SQLite. It is retained recovery evidence,
            not the database used by normal workflows. Inspect the output and keep it with its release/schema context.

            Restore Excel Backup validates the governed package, requires explicit confirmation, creates recovery evidence, restores
            canonical application tables and restarts. Do not use an ordinary spreadsheet or legacy working copy as a restore source.
            SQLite remains the live source of truth before and after a successful governed restore.
            """,
            "Excel backup", "disaster recovery", "governed package", "SQLite source of truth", "restart"),
        new(
            "verification.overview", "Safety and support", "Verification Center reference",
            "Interpret PASS/FAIL, recalculate deliberately and retain release evidence.",
            """
            Refresh rebuilds the report read-only. Recalculate Native Results is mutating: it recalculates and saves native Tensile,
            Impact and Stiffness outputs from their source inputs and governed Settings. Use it only when recalculation is intended, not
            merely to refresh the screen.

            Export Report writes owner-selected TXT evidence. Automation Evidence exists only in a disposable authorized profile.
            Full Data Verification PASS proves applicable deterministic contracts; READY FOR PUBLISH adds publication readiness.
            Neither replaces manual HTML/PDF/UI visual acceptance or proves a live transfer.
            """,
            "Verification Center", "PASS", "FAIL", "Recalculate Native Results", "READY FOR PUBLISH", "evidence"),
        new(
            "diagnostics.overview", "Safety and support", "System Diagnostics reference",
            "Collect environment, database, update and integrity evidence without confusing refresh with recalculation.",
            """
            Refresh rebuilds the read-only diagnostics report. Run Integrity Check executes SQLite integrity_check and reports the
            result without repairing data. Export Report writes the displayed support evidence to an owner-selected file.

            Recalculate All Materials is mutating: it recalculates and saves native measurement results. Diagnostics also reports the
            running version, executable/database paths, ECB cache ownership and update transaction evidence. Its landed-cost section
            reports only aggregate current, legacy, other-versioned and uncalculated or unversioned counts for Purchase Orders and
            Inventory. It does not expose order, supplier, Material or credential values. Migration and governed Excel recovery are
            separate verified workflows; opening or refreshing Diagnostics never runs either one. Export diagnostics before changing
            state when investigating a reproducible failure.
            """,
            "System Diagnostics", "integrity check", "Recalculate All Materials", "export", "database path"),
        new(
            "updates.guarded-apply-and-recovery", "Safety and support", "Guarded update apply and recovery reference",
            "Understand snapshots, external apply, health acknowledgement, rollback and interrupted recovery.",
            """
            Guarded Apply re-verifies extraction, saves pending Material changes, creates a verified SQLite backup and stages governed
            application files. The external helper waits for shutdown, snapshots last-known-good files, installs, restarts and requires
            startup health acknowledgement. Failed launch or health verification rolls application files back.

            Interrupted transaction detection is default-No. Recovery validates durable request/live-directory identity before running
            the external helper. Transaction state, snapshots and backups remain evidence. SQLite backup is evidence only: update apply,
            rollback and recovery never automatically restore SQLite.
            """,
            "guarded update", "health acknowledgement", "rollback", "interrupted", "never automatically restore SQLite"),
        new(
            "publishing.application-release-and-update", "Safety and support", "Application publishing safety reference",
            "Keep application artifacts, signed update feed and live activation order governed.",
            """
            Application Release publishing verifies the deployment plan and exact stable/versioned paths before a default-No live FTPS
            confirmation. Versioned artifacts transfer first, stable installer/portable routes last, and replaced remote files are
            retained in an application-release backup.

            Application Update publishing verifies latest.json and its signed package, uploads immutable versioned content first and
            activates the stable latest feed last. Retain local plan/feed, hashes, signatures and transfer evidence. These workflows are
            separate from Website Production and cannot be inferred from a successful Website transfer.
            """,
            "application publishing", "versioned first", "stable last", "signed update", "transfer evidence"),
        new(
            "publishing.website-safety", "Safety and support", "Website publishing safety reference",
            "Follow Preview, local Production, FTPS Test and guarded live Production as separate stages.",
            """
            Generate Preview is local and isolated. Generate Production validates, confirms and backs up local index output but performs
            no FTPS. Publish Website Test uses the isolated remote test route. Publish Website Production regenerates Production and
            requires its own second default-No live confirmation.

            Require Verification PASS and READY FOR PUBLISH where applicable, inspect representative HTML/PDF, retain manifests,
            publish plans and transfer logs, then verify the live HTTPS site independently. Restore Production backup is also live and
            guarded; it is not SQLite recovery.
            """,
            "Website publishing", "Preview", "Production", "FTPS Test", "READY FOR PUBLISH", "live"),
        new(
            "troubleshooting.verification-fail", "Troubleshooting", "Troubleshooting Verification FAIL",
            "Identify the exact failed contract before recalculating or publishing.",
            """
            Refresh Verification and locate the first FAIL with its detail; also check profile name and applicable/pass/fail counts.
            Export Verification and System Diagnostics before changing data. A FAIL may be identity, relationship, schema, calculation,
            publication or release-contract evidence rather than a stale screen.

            Correct the source at its owning workflow. Use Recalculate only when source inputs or governed Settings require it, because
            recalculation writes results. Re-run Full Verification and require PASS; do not bypass a failure or describe the build as
            READY FOR PUBLISH while an applicable check fails.
            """,
            "Verification FAIL", "first failed contract", "recalculate", "export evidence", "PASS"),
        new(
            "troubleshooting.backup-restore-blocked", "Troubleshooting", "Troubleshooting blocked backup or restore",
            "Use compatibility evidence to distinguish empty, migratable, incompatible and corrupt candidates.",
            """
            Select the candidate and read its full path, schema, integrity, counts, compatibility status and detail. Run Verify Selected
            for a supported older schema. Ready — empty profile is intentionally empty; newer/incompatible, legacy/incomplete and
            corrupt/unreadable candidates remain blocked.

            Do not rename/copy a file to bypass classification and never restore the owner database through automation. Preserve the
            candidate and diagnostics, create a fresh manual backup of the active database, and investigate missing structure or
            unsupported schema before attempting another guarded restore.
            """,
            "restore blocked", "compatibility", "corrupt", "newer schema", "empty profile"),
        new(
            "troubleshooting.interrupted-update", "Troubleshooting", "Troubleshooting interrupted application update",
            "Preserve transaction evidence and use only the guarded recovery action for the recorded phase.",
            """
            Startup reports the transaction ID, phase, versions and safe recovery action. Export System Diagnostics and retain the
            transaction directory, request/state, rollback snapshot and SQLite backup reference. Do not manually copy staged files into
            the live application.

            Guarded recovery is default-No and validates that the request targets the running installation. It may repair or roll back
            application files and restart. SQLite is never automatically restored. If durable state/request identity is unreadable or
            mismatched, recovery remains blocked and evidence should be retained for support.
            """,
            "interrupted update", "transaction phase", "guarded recovery", "rollback snapshot", "SQLite"),
        new(
            "troubleshooting.publish-failure", "Troubleshooting", "Troubleshooting publishing failure",
            "Identify which generation, verification, credential or transfer stage failed.",
            """
            Separate local report build, Website Preview, local Production, FTPS Test, Website Production, Application Release and
            Application Update. Read the exact log/manifest/plan and target route for the failed stage. A successful earlier stage does
            not prove the later live stage completed.

            Preserve local artifacts, hashes, backup path and transfer result. Confirm the governed host/user credential without logging
            the password. Do not retry live publishing until validation and default-No confirmations describe the intended target.
            After success, inspect the live HTTPS route independently.
            """,
            "publish failure", "stage", "credential", "transfer log", "live site"),
        new(
            "troubleshooting.support-evidence", "Troubleshooting", "Support evidence collection reference",
            "Collect reproducible evidence without exposing secrets or changing the failing state first.",
            """
            Record the running version/release code, local time, exact action, expected result and observed message. Export Verification
            and System Diagnostics, plus the relevant report/export/publish/update/recovery logs, manifests, plans and transaction IDs.
            Include screenshots when layout or visible state matters.

            Never include FTPS passwords, signing private keys or unrelated private data. Preserve original failure evidence before
            recalculation, restore, retry or cleanup. State whether the profile is owner, clean/disposable or recovered and identify the
            exact database path without attaching the canonical owner database unless explicitly governed.
            """,
            "support evidence", "version", "diagnostics", "logs", "screenshots", "secrets"),
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
