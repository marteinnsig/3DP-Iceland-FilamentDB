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
database and the canonical tester seed.

## Install the demo

1. Extract `3DPIceland-Public-Demo-v56.0.6.sqlite` from this ZIP.
2. In 3DPIceland, open **File > Backup and Recovery Center...**.
3. Choose **Restore SQLite Backup** and select the extracted `.sqlite` file.
4. Confirm the exact file and the default-No prompt. The application verifies
   it, saves the current profile as a pre-restore recovery backup, restores the
   demo and restarts.

## Remove the demo and return to your own data

1. Open **File > Backup and Recovery Center...** and choose **Refresh**.
2. Select the **Pre-SQLite restore recovery** row created when the demo was
   installed. Match its timestamp, full path and row counts.
3. Choose **Verify Selected**, then **Restore Selected**.
4. Confirm only that exact backup. It restores the prior owner data, or the
   original healthy empty profile on a new installation, and restarts.

Never overwrite or delete the active `filamentdb.sqlite` manually. **Choose
Storage Folder** moves the current canonical database; it does not switch
datasets.
