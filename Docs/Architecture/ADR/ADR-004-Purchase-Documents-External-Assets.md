# ADR-004 — Purchase Documents are External Assets

**Status:** Accepted  
**Decision:** PDFs, images, receipts, order confirmations and customs documents are stored as files under the FilamentDB data folder. SQLite stores relative paths and document metadata only.

Large binary documents are not stored inside SQLite.
