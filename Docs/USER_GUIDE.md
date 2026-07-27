# 3DPIceland Engineering Platform User Guide

Version: v50.2.1 — Data, Cost and Configuration Reference

The canonical operational guide is built into the application:

1. Open **Help > Documentation** for the central Help window.
2. Press **F1** from a top-level tab to open the same window at the relevant
   topic.
3. Use the search field for workflows such as purchase, measurements, reports,
   publish, verification or recovery.

The Help window is local and works offline. It covers the complete workflow
and now includes exhaustive references for Materials, Manufacturers, Purchase
Orders, Inventory, Usage, Printers, Print Job Quotes, Base Materials and
Settings Manager. Remaining per-tab reference coverage is delivered through
the recorded v50.2.2-v50.2.4 increments.

Important safety boundaries:

- ECB rates are optional references for new unsaved purchase data only.
- Saved purchases, inventory lots and material costs retain their historical
  rate evidence. Saved quotes are not automatically recalculated from a later
  rate and can be explicitly deleted from quote history.
- Production and FTPS require explicit authority and default to No.
- Run Verification Center and require PASS before release or publishing.
- Automation must use disposable profiles and must never mutate the canonical
  owner database.

This file is a packaged discovery pointer. The in-app catalog is authoritative
for current navigation and workflow guidance.
