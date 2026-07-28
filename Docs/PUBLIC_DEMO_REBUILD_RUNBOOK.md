# Public Demo Dataset Compatibility Rebuild Runbook

## Purpose

Use this runbook for `v59.0.1` after the final v59 application and installer
bytes are accepted. The goal is to rebuild and revalidate the governed demo
dataset without repeating the full v56 privacy research when its approved
inputs and contracts have not changed.

`Docs/PUBLIC_DEMO_DATASET_CONTRACT.md` remains authoritative. This runbook is a
short execution order, not a replacement privacy or publication contract.

## Choose the correct path first

Use the fast path only when all of these remain unchanged:

- approved owner-created source SHA-256 and schema v38;
- private 36-Material allowlist and registry SHA-256;
- tracked public fictional-identity transformation spec;
- allowed/empty table and column classifications;
- deterministic legacy Impact correction contract;
- public package inventory and privacy denylist.

If every item is unchanged, do not repeat material selection, privacy research,
identity design or owner measurement-risk approval.

Stop and reopen the affected v56 contract stage when any source, allowlist,
schema, mapping, transformation, measurement correction, public field or
package-content rule changes. Never silently refresh a pinned hash.

## Fast compatibility rebuild

1. Freeze the exact accepted v59 application, installer and portable bytes.
2. Verify the pinned private source, allowlist and public transformation hashes
   before generation. Keep the owner database and canonical tester seed closed.
3. Run the accepted read-only inspection and transformation validation twice.
   Require matching logical manifests and zero privacy/schema/relationship
   failures.
4. Build two fresh SQLite outputs in separate disposable directories. Require
   byte-identical files, schema v38, `integrity_check=ok`, zero foreign-key
   violations and the accepted row/relationship/privacy contracts.
5. Recheck the deterministic Impact correction: 718/718 values within 0-100,
   36 fully tested Materials and no source mutation.
6. Build two ZIP packages with fixed entry timestamps and exactly four files:
   SQLite, `README.md`, `manifest.json` and `SHA256SUMS.txt`.
7. Require byte-identical ZIPs, expanded-file allowlisting, internal checksum
   parity and no paths, source identities, private mapping or SQLite sidecars.
8. On a clean disposable machine/profile, install the exact accepted v59
   installer, restore the rebuilt demo through the supported Restore workflow,
   restart and run Full Data Verification.
9. Recheck Materials, filters, rankings, collections and all local demo report
   pages. Confirm no MaterialID exposure, clipping, overlap or row fragmentation.
10. Confirm uninstall/reinstall preserves user data and the rebuilt demo remains
    restorable by the exact stable installer and portable application bytes.
11. Record final SQLite/ZIP byte counts and SHA-256, update package/release
    documentation and obtain owner compatibility acceptance.

## Publication boundary

The owner's standing major-release authorization includes the exact accepted
v59.0.1 governed public-demo ZIP and its public manifest/checksum artifacts.
After rebuild/runtime acceptance, publish the immutable demo route first and
the stable demo route last, with remote backup, rollback evidence and
independent HTTPS download/hash verification.

The authorization does not include the private registry, source backup, owner
identities, a loose SQLite file or other website/report artifacts.

## Completion condition

`v59.0.1` closes only when the rebuilt A/B SQLite and ZIP artifacts are
deterministic, the exact v59 installer/portable restore path passes, Full Data
Verification passes, privacy/report review passes and the owner accepts the
new governed hashes. Final closure also requires guarded FTPS publication and
independent stable/versioned HTTPS hash verification of the accepted demo ZIP.
