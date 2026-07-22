# ADR-002 — Purchase Orders are the Source of Inventory

**Status:** Accepted  
**Decision:** Normal physical inventory is created from reconciled Purchase Order Lines. Manual spool creation remains an exception workflow, not the primary source.

This preserves traceability from a physical spool back to its supplier, order and purchase line.
