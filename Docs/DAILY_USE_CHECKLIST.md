# v50.1 Daily Use Checklist

Use **Help > Documentation > Start-to-finish workflow** for full prerequisites,
save boundaries and cross-tab handoffs. This checklist is the compact owner
sequence.

## Data and testing

1. Create the Purchase Order and every ordered item; review currency/rate.
2. Run **Calculate Landed Costs** and resolve allocation validation.
3. On arrival, run **Receive / Reconcile** and correct counts/check state.
4. Run **Create Materials + Received Spools** for received Filament.
5. Complete draft Material identity and review Inventory spool snapshots.
6. Enter native raw measurements; confirm auto-save and Validation.
7. Use Experimental Series/Runs only for a separate controlled experiment.
8. Clear unintended Materials filters before whole-database analysis.

## Reports and website

1. Save intended public-report and public-test-detail permissions.
2. Build and visually inspect representative reports.
3. Run **Build Public Report Package**.
4. Run **Generate Preview** and inspect `index-test.html`, links and reports.
5. Refresh Verification Center; require Full Data Verification **PASS** and
   Website **READY FOR PUBLISH**. Export the report.
6. If required, publish the isolated Website Test and inspect it.
7. Generate Production only after review and explicit confirmation.
8. Publish Website Production only with separate live FTPS authority.
9. Retain manifest, publish plan, Verification export and completion log.
10. Inspect the live site independently; stop and retain evidence on failure.
