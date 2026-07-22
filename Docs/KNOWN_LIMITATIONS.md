# Known Limitations

This file tracks known non-blocking limitations for the v34.0 daily-use period. These are not release blockers unless they affect data integrity, calculations, or website publish readiness.

## Daily-use observation period

The project is entering a usage-mode period. During this time, the highest-value improvements should come from real-world use while adding materials, measuring specimens, exporting the website, and preparing YouTube videos.

## Current known limitations

### Build and runtime verification

Local Visual Studio build and runtime testing remains the final acceptance gate. Verification Center output should be exported after important changes.

### Excel remains a golden reference

The Excel workbook is retained as a reference/backstop during the transition. The application is the intended primary workflow, but Excel can still be used to compare historical logic if unexpected differences appear.

### Website publish workflow

The app exports website HTML, but publishing/uploading to the production web server remains a separate manual process. Review the generated HTML before replacing the production file.

### Usability unknowns

Several usability issues may only appear during real-world repeated data entry. Track them as usage notes rather than starting immediate feature development.

### Reports are useful but secondary

Reporting, PDF and AI review features are available, but the short-term priority is material data collection and website updates. Report polish should wait until enough real usage feedback exists.

## What counts as a blocker

Treat these as blockers:

- Verification Center FAIL.
- Website export missing rows or wrong data.
- Native calculations diverging from expected results.
- MaterialID corruption or duplicate identity behavior.
- Data loss, failed save, or backup failure.

## What does not count as a blocker

These can wait until the next development cycle:

- Cosmetic report layout improvements.
- Extra charts or filters.
- Additional AI review text.
- Minor wording improvements.
- New report templates.

## Multi-instance editing

Multi-instance editing is not supported. Do not edit data in the Visual Studio instance and the Release EXE at the same time. Use one app instance for data entry, then close it before opening another instance.
