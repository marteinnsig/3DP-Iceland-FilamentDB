# ADR-006 — Purchasing Uses Inventory Categories

**Status:** Accepted  
**Decision:** Every Purchase Order Line has an `InventoryCategory`.

Supported foundation categories:

- Filament
- Printer
- Equipment
- Spare Parts
- Consumables
- Other

The category selects the downstream workflow. In v38.4.0 only Filament creates Materials and Inventory Spools. Other categories remain fully recorded for future inventory modules.
