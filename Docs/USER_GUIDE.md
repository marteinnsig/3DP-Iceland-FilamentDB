# 3DPIceland Engineering Platform User Guide

Version: v50.1.0 — Start-to-finish Workflow Guide

The canonical operational guide is built into the application:

1. Open **Help > Documentation** for the central Help window.
2. Press **F1** from a top-level tab to open the same window at the relevant
   topic.
3. Use the search field for workflows such as purchase, measurements, reports,
   publish, verification or recovery.

The Help window is local and works offline. It covers the complete workflow
from Purchase Orders and Inventory through Materials, measurements, analysis,
reports, Website Preview, guarded Production publishing and recovery.

Important safety boundaries:

- ECB rates are optional references for new unsaved purchase data only.
- Saved purchases, inventory lots, material costs and quotes are immutable
  historical snapshots and are never recalculated from a later rate.
- Production and FTPS require explicit authority and default to No.
- Run Verification Center and require PASS before release or publishing.
- Automation must use disposable profiles and must never mutate the canonical
  owner database.

This file is a packaged discovery pointer. The in-app catalog is authoritative
for current navigation and workflow guidance.
