# Platform Data Flow

```text
Purchasing Platform
Purchase Order → Receiving → Category routing

Filament
Purchase Line → Link/Create Material → Inventory Spools

Materials Platform
Material definition → Engineering measurements

Engineering Platform
Measurements → Native calculations → Verification Center → Material Summary

Publishing Platform
Verified Material Summary → Website / Reports / Certificates / future video workflow
```

SQLite remains the source of truth. Only verified engineering summaries flow into publishing outputs.
