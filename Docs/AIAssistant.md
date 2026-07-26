# AI Assistant

## v47.0.3 - Stable Coverage Identity Candidate

Coverage entries can now own optional stable `CollectionId` and `MaterialKey`
values while retaining their existing collection-title and material-label
snapshots. Status lookup prefers stable identity and falls back to exact legacy
title/label matching, so supported old JSON remains readable.

`Bind Exact Legacy Coverage...` previews only unambiguous candidates: one exact
collection title and one exact material label inside that collection. The
confirmation defaults to No. Unmatched or ambiguous entries remain unchanged;
there is no fuzzy or silent remapping.

New status writes use stable identity. Clearing status and deleting a collection
recognize both stable ownership and exact legacy ownership. Collection deletion
explicitly reports that related coverage will also be removed.

The deterministic tester checks isolated empty coverage identity state without
invoking binding or modifying personal AppData. No SQLite schema changes or
external AI calls are introduced.

## v47.0.2 - Collection Workflow Clarity

Material Collection saving now exposes its exact action and scope before local
JSON is changed.

- The collection title resolves to `Create new` or `Update existing`.
- A read-only preview shows visible-row count, unique MaterialID count, existing
  saved membership and up to 20 MaterialIDs.
- Create and update both use an explicit default-No confirmation.
- The confirmation repeats a bounded MaterialID preview.
- Updating replaces only the selected collection's saved membership.
- Existing pipeline status metadata is not silently deleted.

Selecting an existing collection loads its exact title into the editor and
marks the action as an update. Typing a unique title changes the action to
create. Cancelling either confirmation writes nothing. A cancelled update
shows the unchanged persisted membership and labels the current-filter scope
as a discarded proposal.

The deterministic tester invokes preview only. It never creates or updates
personal AppData collections. Existing session, collection and coverage JSON
formats remain unchanged.

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
