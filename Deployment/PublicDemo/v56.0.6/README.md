# 3DPIceland Public Demo Dataset v56.0.6

This package contains a schema-v38 SQLite demo for the 3DPIceland Engineering
Platform.

The 36 material, manufacturer, product-line, marketing-name, color, variant
and MaterialID identities are fictional. Base-material names and CF/GF
reinforcement remain generic engineering taxonomy. Tensile, Impact and
Stiffness values are real owner-approved comparative measurements disclosed
under fictional identities.

Pseudonymization is not anonymization. Distinctive measurement patterns may
support re-identification. The dataset is for application evaluation,
workflow demonstrations and comparative engineering exploration; it does not
replace manufacturer datasheets or accredited laboratory testing.

The package excludes purchasing, suppliers, Inventory, Usage, quotes, printer
profiles, notes, URLs, paths, credentials, deployment identity, Production
settings and the private source-to-demo mapping.

## Files

- `3DPIceland-Public-Demo-v56.0.6.sqlite` - governed schema-v38 demo database.
- `manifest.json` - public-safe release identity, counts and SQLite hash.
- `SHA256SUMS.txt` - hashes for every payload file in the ZIP.
- `README.md` - this disclosure and usage boundary.

Verify `SHA256SUMS.txt` before use. Keep this demo separate from an owner
database and the canonical tester seed. No Production or FTPS publication is
authorized by this local package.
