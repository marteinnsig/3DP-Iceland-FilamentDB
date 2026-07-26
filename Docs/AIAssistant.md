# AI Assistant

## v47.0.1 - Local Purpose and MaterialID Scope Clarity

The AI Assistant is a deterministic local planning workspace. It does not call
an external AI service and it does not change canonical Materials,
measurements, reports or website data.

The workspace now shows its current canonical Materials scope before
generation:

- Count of active visible rows.
- Count of unique visible MaterialIDs.
- A bounded MaterialID preview.
- An explicit refresh action.

The scope is rebuilt from the same native SQLite-backed Materials projection
used by the current Materials filters. Opening the AI Assistant refreshes the
preview, and the existing Generate actions continue to use that projection.

The editable planning note is included in the generated brief for reference.
It is not interpreted as a free-form AI prompt. Saved sessions remain local
JSON snapshots, while Material Collections retain their existing backwards-
compatible MaterialID and label representation.

The workspace separates its purpose into two visible stages:

1. Generate a local planning brief.
2. Save or reuse an exact MaterialID collection and its local pipeline status.

No external API, credential, schema or collection-storage migration is
introduced by v47.0.1.

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
