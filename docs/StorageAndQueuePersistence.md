# v19.7 Storage Location & Queue Persistence

- The application now defaults the SQLite database to `Documents\3DPIceland Labs\FilamentDB`.
- The folder can be changed from File → Choose Storage Folder or the toolbar.
- The app copies the existing database to the selected folder when possible.
- Video ideas from Recommendations are persisted in SQLite and reload after closing/reopening the app.
- The database remains a local SQLite file, so it can be backed up by OneDrive if the selected folder is synced.
