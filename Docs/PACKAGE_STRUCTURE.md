# Package Structure

The release ZIP is intentionally kept lean and predictable.

## Root

The package root contains the project-level files required by users and source distributions:

- `README.md`: project overview, build entry points and public platform links.
- `LICENSE`: canonical GNU General Public License v3.0 only text for original project code.
- `THIRD-PARTY-NOTICES.md`: dependency license inventory and retained third-party notices.

The repository may additionally contain source folders, the solution shortcut and version-control metadata. Do not add per-release notes or generated output to the root.

## App/

Contains the complete buildable Visual Studio solution, source code, project files, scripts, and required application assets. Files in this folder should not be removed merely because their purpose is not immediately visible; the C# source, XAML, services, models, assets, solution, and project file are needed to build or maintain the application.

Generated build folders such as `bin/`, `obj/`, `.vs/`, publish output, temporary files, user settings, and database files must never be included.

## Docs/

Contains only current canonical documentation.

- `BUILD_NOTES.md`: current build summary and What to Test list; overwritten each release.
- `CHANGELOG.md`: concise chronological product changes.
- `BUILD_HISTORY.md`: build-by-build engineering history.
- `MILESTONES.md`: consolidated detailed milestone history, newest entries first.
- `PROJECT_HISTORY.md`: larger project narrative and milestones.
- `VERSION.txt`: current release identity.
- Other files document current architecture, workflows, features, usage, and known limitations.

Do not create a new `BUILD_NOTES_v...` or `MILESTONE_v...` file for every release. Historical information belongs in the consolidated canonical history documents.

## Reports/

Contains `README.md` and the consolidated `VERIFICATION_HISTORY.md`. Static audits and release verification entries are added to the top of that file; do not create per-version report files.

## Release cleanup rule

Before packaging each release:

1. Remove generated build output and temporary files.
2. Replace `Docs/BUILD_NOTES.md` with the current release notes.
3. Append the release to `CHANGELOG.md` and `BUILD_HISTORY.md`.
4. Add the completed milestone to the top of `Docs/MILESTONES.md` when milestone detail is needed.
5. Add release verification to the top of `Reports/VERIFICATION_HISTORY.md`.
6. Confirm that no version-specific build-note, milestone, audit or verification files were added.
7. Confirm that `LICENSE`, `README.md` and `THIRD-PARTY-NOTICES.md` are present and agree on `GPL-3.0-only`.
