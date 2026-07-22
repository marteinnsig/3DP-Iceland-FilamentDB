# ADR-007 — Purchase Orders are Category-Agnostic

**Status:** Accepted  
**Decision:** One Purchase Order represents the supplier transaction exactly as purchased, including mixed orders.

A Prusa order containing a printer and filament remains one order. Filament lines enter the Materials/Spool workflow; the printer line remains a Printer-category purchase line until Equipment Inventory is implemented.
