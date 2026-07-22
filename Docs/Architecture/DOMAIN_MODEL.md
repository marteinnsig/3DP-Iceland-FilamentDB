# Purchasing Domain Model

```text
Supplier
   ↓
Purchase Order
   ↓
Purchase Order Line
   ├─ Inventory Category
   ├─ optional MaterialID
   └─ Purchase Documents belong to the order

Filament line
   ↓
Material definition
   ↓
Inventory Spool(s)

Printer / Equipment / Spare Parts / Consumables / Other
   ↓
Future category-specific inventory modules
```

## Ownership

- **Supplier** owns reusable supplier identity and future statistics.
- **Purchase Order** owns transaction-level totals, tax treatment, shipping, import costs and lifecycle.
- **Purchase Order Line** owns description, category, quantity, price, discount and physical unit weight.
- **Material** is optional until a Filament line is reconciled.
- **Inventory Spool** represents the physical spool and its storage location.
- **Purchase Document** references an external file by relative path.
