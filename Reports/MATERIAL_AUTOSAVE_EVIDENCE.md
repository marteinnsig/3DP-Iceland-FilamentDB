# Materials autosave evidence — 2026-10-05

## Accepted refresh — 2026-10-11

- Owner confirms committed save, normal restart, real five-minute retry, active typing/focus and Help.
- Solution Debug/Release PASS, zero warnings/errors; Help, release documentation and diff gates PASS.
- Disposable CRUD profile `20261011180607-c8ed4367` PASS; Full Data Verification 483/483 PASS.
- Periodic callback persistence, validation rejection, retained errors, recovery and restart checks PASS.
- Baseline/final business SHA-256 both `2B09F8539C2B3481ED1969B28F513D5E245C53B08765BAE5F99D4354F55DE4CF`.
- Normal Release EXE SHA-256 `F2CA51FDA0FA395530591A2DDC83462840B954DCCC36FD8C314A6DBA31A918D4`.
- Normal Release DLL SHA-256 `CD38A7AA7D70EAF27C7FC9190E011D84C047CD5653E764787DC869EDEDCD20E8`.
- Retained evidence: `Reports/MaterialAutosave/2026-10-11` (JSON/TXT and screenshots).
- Initial sandbox build failed writing existing MSBuild cache; authorized builds outside that restriction passed.
- Accepted profile temporarily retained for review; older successful profiles eligible for governed cleanup.
- Existing committed-cell save path and editor lifecycle preserved. Timer calls only silent Materials save,
  skips restore/debounce and stops when window closes. No schema, raw reading, owner-data or FTPS changes.

## Prior candidate evidence — preserved history

State: candidate tested; owner runtime and Help acceptance pending. No commit/push or external publication.

- Final Debug and isolated Release builds PASS, zero warnings/errors. A normal Release rebuild was blocked
  by the owned acceptance process; final Release used isolated ArtifactsPath instead. No process was stopped.
- Help control coverage, release documentation audit and git diff --check PASS.
- Preliminary CRUD profile `20261005203115-d0607ff5`: PASS; Full Verification 482/482.
- Final exact-candidate CRUD profile `20261005203428-3b6f3b4f`: PASS; Full Verification 482/482.
- Real retry callback persisted fixture notes, blocked invalid Product Line, retained failure messages
  through statistics refresh/unrelated save, then recovered valid state and restored original notes.
- Measurement-failure status while Materials is dirty is also covered. Independent read-only review
  identified this precedence issue; final code preserves module errors and clears them only after success.
- Final baseline/final business SHA-256 both:
  `2B09F8539C2B3481ED1969B28F513D5E245C53B08765BAE5F99D4354F55DE4CF`.
- Canonical seed read-only SHA-256:
  `CEF1F9D5142578BBFE91D37A79BF11A6255CE550B8CA388D8582BF2D7D332D09`.
- Final application DLL SHA-256:
  `E50B73BF48D3C6D7D4980AEAD73A4D4073B41E56C305322FF3BDBC352FFFE45B`.
- Final candidate: `.private/material-autosave/candidate`; retained JSON/TXT evidence:
  `.private/material-autosave/evidence`; complete disposable profiles remain available for acceptance.
- Exact tested candidate copied to `App/FilamentDbApp/bin/Release/net9.0-windows`; DLL parity verified.
  Completed `artifacts/material-autosave` build output removed; artifacts is empty.
- No owner database access, schema change, measurement calculation change, seed replacement or FTPS.
- Callback invocation avoids a five-minute sleep; actual timer scheduling and active typing/focus remain
  owner checks in `Docs/MATERIAL_AUTOSAVE_ACCEPTANCE.md`.
