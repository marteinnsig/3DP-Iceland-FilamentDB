# v70.0.5 — Izod and Charpy Website Details (accepted)

The main Filament Database website now puts the saved Izod/Charpy measurement summaries in each method bar's existing Legacy Impact-style hover/click tooltip. The static bottom list and its unused generator are removed. Saved public-safe evidence is attached by MaterialID and exact method, using the same report text formatter. Chart values and scoring still use verified summaries; report/PDF evidence remains supported.

Dates, mean, SD, CV%, Samples, Confidence, no-break/invalid/excluded counts and comparative-testing limitations remain available. Group views identify each constituent material without pooling specimen statistics. Filters apply to details; redraw dismisses an old pinned pendulum tooltip. Long pinned details scroll. Enter/Space opens details and Escape closes them.

## Acceptance and automation assessment

- Debug and Release isolated solution builds PASS, zero errors. Restore emitted NU1900 because sandbox NuGet access was unavailable; separate online vulnerable-package audit completed successfully and reported no vulnerable packages.
- Help coverage/drift and release documentation audit PASS. `website.preview` remains the stable Help owner; ledger updated. No WPF editor, control or AutomationId changed.
- Existing deterministic PendulumPresentationVerification is extended to check saved evidence, method isolation, dates, statistics and private-note exclusion. Synthetic C# contract and real WebsiteHtmlRendererService projection PASS.
- Headless Edge over the real rendered synthetic website PASS: hover, click, Enter/Space, Escape, group membership, filtering, stale-pin dismissal, mobile viewport popup bounds and no JavaScript errors. Screenshot inspected. Browser viewport evidence does not establish physical touch-device or accessibility acceptance.
- `node Tools/Test-PendulumWebsiteDetails.cjs` PASS: escaping, duplicate display names with distinct MaterialIDs, constituent grouping, filters, null/zero and stale-pin dismissal. This durable check complements the existing Full Verification contract; no new scenario authorization or AutomationId is needed.
- Full Data Verification PASS 483/483 in the exact Debug candidate with a validated disposable marker. Existing reports runner failed before verification on two navigation paths (popup guard in the sandbox; NavigateThermalDeflectionMeasurementsTab timeout on the desktop). These failures are retained and are not reported as reports acceptance. Computer Use opened Help → Verification Center directly, exported TXT/JSON evidence, and closed only the disposable candidate. No safety guard was changed.
- Exact final business-state hash matches the recorded runtime baseline: `2B09F8539C2B3481ED1969B28F513D5E245C53B08765BAE5F99D4354F55DE4CF`. Canonical seed SHA-256 remains `CEF1F9D5142578BBFE91D37A79BF11A6255CE550B8CA388D8582BF2D7D332D09`. The existing runner read-only logical hash implementation performed this comparison after shutdown; database file bytes differ because volatile timestamps are excluded by the business-state contract. Owner accepted the reviewed tooltip presentation; this does not claim a real-device touch or accessibility test.
- Read-only subagent review found the stale pinned-tooltip case; correction is included and browser-tested.

## Exact owner preview checklist

1. **Where:** open `D:\CodexProjects\3DPIceland-App\.private\v70-tooltips\index-test.html` in a browser. This fixture uses synthetic materials and the real website renderer.
   **Action:** scroll to **Izod impact strength**, then hover the **Synthetic Yellow PLA** bar.
   **Expected result:** the popup shows Izod details, 2026-11-05, statistics and the comparative-testing limitation. Hover the corresponding **Charpy impact strength** bar; it shows only Charpy details dated 2026-11-04.
2. **Where:** the same browser chart.
   **Action:** click a bar to pin, click again to close; focus a bar with Tab, press Enter or Space, then Escape.
   **Expected result:** popup opens and closes; longer pinned details can scroll.
3. **Where:** the website's **Chart mode** selector and **Search** field.
   **Action:** select **Average by manufacturer** and inspect the Izod popup; then return to **All filament samples** and search `Yellow`.
   **Expected result:** grouped details identify both constituent materials separately; filtered details show only Yellow. Changing the selection dismisses a previously pinned popup. The old bottom list is absent.
4. **Where:** candidate `D:\CodexProjects\3DPIceland-App\.private\v70-tooltips\App\3DPIcelandFilamentDB.exe` → **Website Export** → **Generate Preview**, when normal preview prerequisites are satisfied.
   **Action:** open the generated `index-test.html` and inspect representative real Izod/Charpy bars.
   **Expected result:** the same behavior with saved measurements and readable details, with no duplicate list at the bottom.
   **Do not click:** Generate Production, Publish Website Production, FTPS, Restore, Recalculate, Delete or Update. These steps require none of them.

5. **Where:** candidate → **Help** → **Documentation** (the Help window, not the website).
   **Action:** search for `Website Preview reference` and read its Izod/Charpy paragraph.
   **Expected result:** hover/click/keyboard details, pinned scrolling and separate grouped statistics match the preview.

Owner approved the tooltip presentation on 2026-11-05 and explicitly requested normal Release promotion. All 54 governed application files were copied unchanged from the retained v70.0.5 Release candidate; each size and SHA-256 matches its accepted manifest. Previous governed files are retained at `.private/v70-tooltips/pre-promotion-release`. DLL SHA-256: `90F26AE118CE82B2891A2FB11DBC3B8D4C6F0C6D73BFC2BFD4FC994B3C3F2CA8`. Owner database, WebView2 state, other existing Release contents and live website are preserved. No installer/update package or FTPS publication occurred.


## Retained local evidence

`.private/v70-tooltips/`: candidate `App/`, exact candidate file manifest, real-renderer synthetic `index-test.html`, `izod-tooltip.png`, browser probe/result and C# fixture. These are retained outside disposable build output for owner review. Canonical seed and owner data are unchanged.
