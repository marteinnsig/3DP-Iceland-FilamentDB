# Governed Public Demo Dataset Contract

## Status and ownership

- Increment: v56.0.1.
- State: accepted for v56.0.1; v56.0.2 read-only inspection is current.
- Canonical application release: v55.0.6.
- Database schema: v38.
- Public demo ownership is independent from the canonical tester seed.
- Production, FTPS and public download publication remain unauthorized.

This contract governs a distributable SQLite demo containing real
owner-approved measurements under disclosed fictional identities. It does not
authorize reading the active owner database, generating an artifact, running a
demo migration or publishing a file.

## Threat model

The public artifact must prevent disclosure of:

- real manufacturer, product-line, marketing-name, SKU or MaterialID identity;
- purchasing, supplier, order, invoice, pricing, shipping or tax information;
- Inventory, storage, quantity, remaining weight, batch or usage history;
- customer, quote, printer, operator or prepared-by identity;
- notes, free-form descriptions, filenames, local paths or recovery evidence;
- URLs, hostnames, IP addresses, credentials, tokens or deployment settings;
- source workbooks, source paths, automation profiles, logs or SQLite sidecars;
- the private mapping between source and fictional identities.

Pseudonymization is not anonymization. A distinctive measurement pattern can
still support re-identification. Every source material and its retained
measurement distribution therefore require explicit owner approval.

## Governed source boundary

The future inspector may accept only one explicitly named owner-created,
read-only input:

1. a Manual Backup SQLite file; or
2. a governed Excel recovery package.

It must never discover or open the configured owner database, LocalAppData,
canonical tester seed, backups folder or another implicit source. The source
must be copied into disposable inspection storage before any read. Source
integrity, schema v38 and SHA-256 must be recorded before inspection.

The active owner database and `C:\Seed-Database\filamentdb.sqlite` are outside
this workflow.

## Private owner allowlist

The owner approves 30–40 exact source MaterialIDs in a private governed
allowlist. Raw source identities and their fingerprints must not be committed
to the public repository or included in the demo package.

Each approved entry must declare:

- a stable demo slot, for example `DEMO-MAT-001`;
- the exact private source MaterialID;
- the approved measurement domains;
- the fictional manufacturer and product-family grouping;
- the approved generic base-material class;
- whether any archived or unlinked behavior is intentionally demonstrated;
- an owner decision of `Approved` or `Rejected`;
- a short reason when a distinctive measurement pattern is accepted.

The allowlist is immutable for one artifact version. Adding, removing or
reordering materials requires a new allowlist version and regenerated evidence.

## Allowed public data

Only positively classified fields may contain rows or values:

- minimal `AppMeta` schema and public-demo release markers;
- fictional `Manufacturers` identities and reviewed generic descriptions;
- reviewed generic `BaseMaterialCatalog` identities and engineering guidance;
- `NativeMaterialManagerRows` with fictional identities, public material
  classification, tested flags and governed print settings;
- approved native tensile, impact and stiffness samples/results;
- required `TestSummaryValues` derived from the approved demo measurements;
- seeded lookup or experiment definitions required for runtime compatibility;
- explicitly approved synthetic experiment fixtures, if later required.

Generic engineering labels such as PLA or PETG may remain only when the owner
approves them and they do not disclose a commercial identity.

## Required transformations

- Assign fixed `DEMO-MAT-###`, `DEMO-MFR-###` and other demo-namespace IDs.
- Rewrite every retained relationship through one versioned mapping registry.
- Preserve approved same-manufacturer, product-family and base-material groups.
- Use fixed demo timestamps and invariant, normalized numeric strings.
- Replace provenance with controlled public-demo wording.
- Replace free-form validation text with governed stock text or omit it.
- Default public-report and public-test-detail flags to false.
- Default archived state to active unless an archived case is explicitly
  approved for a bounded runtime test.
- Recompute derived summaries from retained measurements; never copy stale
  summaries from a different subset.

The private mapping registry and any secret used to fingerprint source
identities stay outside the repository and artifact.

## Always-empty or excluded domains

Runtime-required tables may exist with zero rows, but these domains must carry
no owner data:

- Imports and legacy Excel source metadata;
- legacy Materials and legacy measurement projections;
- Video Idea Queue;
- Inventory and Usage;
- printer profiles and Print Job Quotes;
- Purchase Orders, lines, suppliers and purchase documents;
- Deployment settings and credentials;
- owner website templates and Production identity;
- free-form measurement notes;
- experiments, runs and measurements unless separately approved as synthetic.

Native Settings may contain only explicit built-in constants required for
deterministic engineering results. Owner pricing, currency, prepared-by,
deployment and path settings are prohibited.

## Fail-closed inspection contract

The v56.0.2 inspector must stop without producing SQLite when it finds:

- a source other than the explicit owner-created input;
- a schema other than the accepted v38 contract;
- an unknown or unclassified table, column, view or trigger;
- a missing, duplicated, orphaned or out-of-allowlist relationship;
- a non-approved source MaterialID or measurement domain;
- a denylisted column containing data;
- an unreadable TEXT, BLOB, JSON or HTML value;
- a path, URL, email, credential marker or private identity;
- a WAL, SHM, journal, backup, log or unexpected package file.

Inspection output is a dry-run manifest only. It contains classifications,
counts, hashes and failures, never raw private values.

## Future artifact acceptance

Later increments must prove:

- exact schema v38 and governed table/column classifications;
- `PRAGMA integrity_check` is `ok`;
- `PRAGMA foreign_key_check` returns no rows;
- exact expected row counts and relationship coverage;
- no unknown files or SQLite sidecars;
- two independent generations are byte-identical by SHA-256;
- stable logical and per-table hashes;
- Full Data Verification passes with no N/A results;
- restart, Materials filters, rankings, collections and local reports work;
- the source demo artifact remains byte-identical before and after runtime;
- manual privacy, usefulness, disclosure and HTML/PDF review passes.

The package may contain only the versioned SQLite file, a governed README,
manifest and checksum. Installer, updater, website-report and canonical tester
packages must not absorb it.

## Publication boundary

Local generation and acceptance do not authorize publication. A future
download requires:

1. accepted privacy, usefulness, runtime and report evidence;
2. an exact local artifact plan and pre-publish SHA-256;
3. an explicit owner-approved Production package action;
4. separate explicit authority for live FTPS transfer;
5. remote download and hash verification;
6. stable-route activation last.

Every Production and FTPS prompt remains default-No. Automation must hard-block
both actions.

## v56.0.1 completion condition

v56.0.1 closes only when the owner:

- approves this privacy and provenance contract;
- provides or identifies one explicit owner-created read-only source;
- approves the private 30–40 MaterialID allowlist and its demo grouping;
- accepts the re-identification risk for every retained real measurement set.

No inspector or generator implementation may begin before those conditions are
recorded.

## v56.0.1 accepted evidence

The owner accepted the contract, the exact 36-Material private allowlist and
the re-identification risk of retaining real measurements.

- Approved source schema: v38.
- Source SHA-256:
  `74943C492AE0FD06DABD7485D648222F87EE059343C2E3F6EAC298116F6D14F8`.
- SQLite integrity: `ok`.
- Foreign-key violations: zero.
- Allowlist entries: 36 unique source IDs and 36 unique demo IDs.
- Coverage: 10 fictional manufacturer groups and 11 base-material classes.
- Measurements: 712 tensile, 718 impact and 36 stiffness rows.
- Archived materials: zero.
- Missing Manufacturer/Base Material links: zero.
- Private registry SHA-256:
  `A26A8406F2219DE035AEE6F24ECE3676037FD7880C01266009CACBF027BA9A7B`.

Raw source identities, the owner path and the mapping remain gitignored and
must never enter tracked documentation, manifests or public artifacts.

v56.0.1 is complete. v56.0.2 may implement only the accepted fail-closed,
read-only inspector and dry-run dependency manifest. It must not generate a
demo SQLite database.

## v56.0.2 accepted inspector

`App/DemoDatasetTool` is a separate operator CLI. It does not reference the WPF
application, discover a configured profile, run migrations, create backups or
reuse the canonical tester seed.

The only accepted command shape is:

```text
3DPIcelandDemoDatasetTool inspect
  --inspection-root <governed-artifacts-root>
  --source <explicit-disposable-copy.bak>
  --allowlist <explicit-private-allowlist.json>
  --output <new-manifest.json>
```

All four paths are required. The source, allowlist and output must be direct
children of a regular `artifacts/v56-source-inspection` root. Inputs must be
distinct regular files, output must not already exist and source must use
`.bak` or `.sqlite`. Reparse inputs, aliases and implicit paths are rejected.

The inspector:

- hashes the source before and after inspection;
- pins the complete private registry hash independently of approval booleans;
- validates every entry's domains, fictional groups, base class and risk grant;
- binds the private registry to its approved source SHA-256 and schema;
- opens SQLite immutable, read-only, pooling-disabled and `query_only`;
- requires schema v38, integrity `ok` and zero foreign-key violations;
- pins the exact tables, indexes, triggers and normalized schema SQL hash;
- validates all 36 Materials and required Manufacturer/Base Material links;
- requires exact accepted group and measurement counts plus archived=0;
- scans every retained measurement value as bounded invariant numeric content;
- classifies every known table as `RETAIN`, `TRANSFORM`, `RECOMPUTE` or `EMPTY`;
- reports excluded private source rows as counts without rejecting their
  legitimate presence in the owner-created source;
- writes only a deterministic, privacy-safe dry-run JSON manifest.

The pinned accepted post-retirement v38 source shape contains 25 tables,
17 indexes and 6 triggers. Retired legacy import/Excel tables are not silently
accepted merely because their historical `CREATE TABLE` definitions remain in
migration source; their presence changes the schema hash and fails inspection.

The manifest contains no paths, source MaterialIDs, fictional mapping,
manufacturer/product identities or matched private values. It records only
contract identity, hashes, schema/integrity results, classifications, counts,
relationship coverage, ordered failure codes and its logical SHA-256.

The internal `self-test` command uses no database. AutomationRunner is unchanged
because v56.0.2 has no application runtime workflow and must never authorize a
private source. Full demo runtime automation belongs to v56.0.5.

Help is unchanged because this CLI is not an in-application user surface.
Operator usage and safety ownership remain in this governed contract.

Owner acceptance confirms the hardened dry-run inspector, exact count/closure
checks, pinned source/registry/schema hashes, deterministic privacy-safe
manifest and fail-closed drift probes. v56.0.2 is complete.

v56.0.3 may now define stable fictional identity and transformation behavior.
It must not generate SQLite, publish artifacts or expose the private registry.
