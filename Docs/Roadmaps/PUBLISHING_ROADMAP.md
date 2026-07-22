# Publishing Roadmap

Maintain verified website and report outputs. Future scope includes video idea backlog, title planning, thumbnail assets, publishing order and YouTube scheduling support.

## Website deployment status

Production website publishing is operational as of 2026-07-21:

- Website Preview and Production retain the same canonical renderer.
- Publishing requires explicit FTP over TLS and validates the server certificate.
- Passive-mode production transfer has been validated against `www.iskort.is`.
- Existing remote files are copied into a timestamped backup before replacement.
- The main website, methodology whitepaper and Manufacturers compatibility redirect are published as one governed package.
- The deployed Manufacturers redirect resolves to the canonical `index.html#manufacturers` portal route.

Future publishing work should extend diagnostics, recovery and scheduling only when a concrete workflow need is identified; it must not create a parallel website rendering path.

## Future website localization and branding

- Assess retaining English while adding a complete Icelandic site presentation with an `EN / IS` toggle near the top of the website.
- Define governed translation sources, English fallback behavior, canonical URLs/metadata, report-language boundaries and explicit Verification coverage before implementation.
- Reuse the existing canonical Preview/Production renderer and publishing package; do not introduce separate language-specific website engines.
- Add an approved responsive and accessible 3DPIceland logo/brand asset, with governed source files and consistent placement across supported themes and page sizes.
- Keep this as a future milestone; it is not part of the current Backup and Recovery Center release.

## v42 candidate - Public engineering report integration

The accepted v41.7 report portfolio contains no inherently proprietary
engineering output. Its intended direction is public knowledge-sharing for the
3D-printing community, with methodology, evidence limitations and source
boundaries retained alongside the results.

Candidate website placement:

- Material Engineering Report: per-material `View engineering report` and PDF links.
- Comparison Report: curated comparisons and reusable Material Family presets.
- Manufacturer Report: manufacturer profile/report links inside the Manufacturers portal.
- Test Session Report: raw-input and result-quality transparency after notes are
  approved for publication.
- Printing Recommendation Report: governed application guidance on material pages.
- Material Summary Report: public dataset/version overview.
- Combined Engineering Report Package: optional downloadable release snapshot.

Implementation contract:

- Treat each accepted internal engineering report as the canonical content baseline for its public counterpart. Public reports should preserve the same useful measurements, governed scores, comparisons, charts, rankings, coverage context, interpretation and limitations wherever those fields are public-safe; they must not be reduced to teaser or summary-only versions.
- Remove only explicitly non-public information: purchasing and supplier operations, inventory/stock operations, credentials, device/local paths, raw specimen rows where publication is not approved, and internal/unreviewed notes. Public safety is a field-boundary rule, not a reason to reduce engineering depth.
- Generate static versioned HTML/PDF artifacts through the canonical Preview
  renderer and publish them with the existing guarded production package.
- Use stable MaterialID-based paths and record report version/generated time.
- Add a public-field allowlist; never publish credentials, local filesystem
  paths, purchase/inventory details or unreviewed internal notes.
- Keep manufacturer/product/video links safe and preserve explicit methodology,
  equipment and non-accredited-testing limitations.
- Verify Preview/Production parity, file/link integrity, package size and stale
  report detection before enabling production publication.
- Prefer curated/on-demand export selection if publishing every report for every
  material would make the website package unnecessarily large.
