# Build and Release Workflow

## Build note format

Every zip/build delivery should start with a short version-first build note:

```text
v25.6.5 - Full Documentation Audit
```

## Release naming

Use clear versioned zip names:

```text
3DPIceland_FilamentDB_WPF_v25_6_5_full_documentation_audit.zip
```

## Release rules

- Every packaging command must declare `Candidate` or `Production`; an artifact is
  not canonical merely because it was built successfully.
- `Production` packaging requires a clean Git worktree. Candidate packaging may
  use a dirty tree only when explicitly requested for pre-release verification.
- Existing signed ZIP, feed, installer, portable ZIP or deployment-plan artifacts
  are never overwritten silently. Use a new empty output directory.
- Feature builds should add visible app functionality or meaningful project documentation.
- Avoid marker-only or foundation-only releases unless explicitly needed.
- Build errors should be fixed before continuing to the next milestone.
- Build fixes should use patch versions, for example `v25.6.1`.
- Changelog/release notes belong in `Docs/`, not as loose root-level v25 `.txt` files.
- Root should contain only project-level files such as `README.md`, `LICENSE`, `THIRD-PARTY-NOTICES.md`, solution files, scripts and source folders.
- Every distributable build must include the canonical `LICENSE` and `THIRD-PARTY-NOTICES.md` files beside the executable.
- Dependency upgrades must include a review and refresh of the third-party license inventory.

## Recommended GitHub practice

- Commit after every successful local build.
- Use GitHub releases for major milestones.
- Keep `Docs/CHANGELOG.md` updated when a version is completed.
- Keep the repository license identifier set to `GPL-3.0-only` unless the copyright holder explicitly approves a future relicensing decision.


## Lean package documentation policy

- `Docs/BUILD_NOTES.md` is replaced for every release; do not create per-version build-note files.
- Append durable history to `Docs/CHANGELOG.md` and `Docs/BUILD_HISTORY.md`.
- Add completed milestone detail to the top of `Docs/MILESTONES.md`; do not create per-version milestone files or recreate `Docs/Milestones/`.
- Add release verification to the top of `Reports/VERIFICATION_HISTORY.md`; do not create per-version audit, verification or implementation-report files.
- Do not package `bin/`, `obj/`, `.vs/`, publish output, temporary data, databases, or user-specific settings.
- See `Docs/PACKAGE_STRUCTURE.md` for the canonical package layout.

## Canonical first-install route

The public stable installer, portable ZIP and update feed remain on the accepted
v43 deployment baseline until the exact tested **v49.0.0** Candidate bytes pass
fresh-install and guarded-update runtime acceptance, clean-tree Production
promotion and a separate verified publish.

The v49.0.0 signed package supports the governed schema-v29 public baseline
through the current schema v37. `BuildInfo.MinimumUpdateDatabaseSchema` and
`BuildInfo.CurrentDatabaseSchema` are the shared release-compatibility contract
for application startup, packaging and independent package verification.

Application-file mutation remains default-No, the ECDSA P-256/SHA-256 package
signature and exact governed inventory remain mandatory, and SQLite is never
restored automatically. Clean-profile readiness does not replace the canonical
schema-v37 owner-data Full Data Verification gate.

## Deterministic v44.0 release gates

Build scripts accept `-ReleaseState Candidate|Production` and print the state in
their result. `App/test_release_gates.ps1` validates:

- clean-tree policy for Production and the NuGet transitive vulnerability result;
- BOM-less `latest.json`, exact ZIP bytes and SHA-256;
- trusted ECDSA signature, exact governed inventory and SQLite schema through the
  production application verifier;
- optional installer/portable bytes and SHA-256; and
- versioned-route-first/stable-route-last deployment plus package-first/feed-last
  update activation contracts.

Run packaging into new empty directories. Production promotion is a separate
decision after all static/package gates and any required installer/updater runtime
acceptance pass.

`App/promote_release_candidate.ps1` is the byte-preserving promotion path for a
runtime-accepted direct installer/portable Candidate. It requires a clean Git
tree, rejects an existing Production target, rechecks every recorded byte count
and SHA-256, copies the tested binaries unchanged, and writes BOM-less Production
metadata with the exact promotion commit. Run the Production release gates
against the promoted output before any FTPS publish action.

Publish the Production application deployment plan before the Production update
feed. The application release publisher activates versioned installer/portable
routes before their stable `/downloads` routes. The update publisher transfers
the signed ZIP before activating `/updates/latest.json`.

Authenticode remains deferred while distribution is private. The internal
package is protected by the trusted ECDSA signature, but Windows can still show
**Unknown publisher** for the Setup EXE or executable. Users must obtain packages
only from the governed 3DPIceland route and verify the published identity; ECDSA
package trust does not suppress the Windows warning.
