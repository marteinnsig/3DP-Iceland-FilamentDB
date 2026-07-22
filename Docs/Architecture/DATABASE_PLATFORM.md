# Native Database Platform

## Responsibility

The Native Database Platform owns durable local state and identity. SQLite is the source of truth for the native application.

## Ownership

- SQLite database lifecycle
- Material rows
- MaterialID persistence
- Native import state
- Auto-save behavior
- Backup-safe workflows

## Rules

- MaterialID is the canonical engineering key.
- Material storage must not be bypassed by downstream systems.
- Import/export changes must preserve auto-save and backup safety.
- Database changes require regression verification.

## Downstream contract

The Engineering Platform reads native rows by MaterialID and produces verified engineering outputs. Downstream platforms must not treat Excel exports or HTML payloads as the source of truth.

## v40 Experimental Testing
ExperimentDefinitions is the SQLite catalog. MaterialExperiments stores MaterialID-linked controlled variants and optional BaselineMaterialId references.
