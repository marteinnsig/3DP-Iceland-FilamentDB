# AI Assistant

## v26.0 - AI Assistant

The AI Assistant introduces a visible workspace tab inside the WPF application.

## Visible App Features

- Top-level AI Assistant tab.
- Generate Video Ideas.
- Generate Comparisons.
- Generate Full Assistant Brief.
- Filter-aware output based on the currently visible material rows.

## Purpose

The assistant is designed to sit on top of the existing material database, analytics and YouTube Research Suite.
It turns the current material selection into practical content planning output.

## Current Scope

v26.0 is local and rule-based. It does not call an external AI API.
The goal of this milestone is to create the visible assistant workflow inside the app before adding deeper AI integration later.


## v26.1 - AI Prompt Templates & Saved Sessions

v26.1 expands the AI Assistant into a repeatable workflow.

### Added Features

- Prompt template dropdown.
- Editable prompt template text.
- Generate From Template action.
- Saved sessions stored locally in `ai-assistant-sessions.json`.
- Load and refresh saved sessions.

### Session Storage

Sessions are stored in the user's local application data folder under:

`3DPIcelandLabs/FilamentDbApp/ai-assistant-sessions.json`

Each saved session stores:

- Session title.
- Template name.
- Prompt text.
- Generated output.
- Visible material count.
- Created timestamp.
