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
