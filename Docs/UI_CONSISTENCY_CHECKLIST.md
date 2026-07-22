# UI Consistency Checklist

Use this checklist for every build that changes the visible interface.

## Control layout
- [ ] Common buttons use consistent height, padding and spacing.
- [ ] TextBox and ComboBox controls align vertically within the same row.
- [ ] Labels align with their controls and do not wrap unexpectedly.
- [ ] Group headings and section spacing follow the same visual rhythm.
- [ ] Narrow-window and minimum-window layouts remain usable.

## Keyboard and mouse behavior
- [ ] Tab and Shift+Tab move in a logical workflow order.
- [ ] Enter behavior is deliberate and does not trigger unexpected actions.
- [ ] Mouse caret placement remains native, including after the final character.
- [ ] Select All occurs only for deliberate keyboard/focus workflows.
- [ ] Escape closes or clears only where the action is obvious and safe.

## Tooltips and wording
- [ ] Ambiguous actions have concise tooltips.
- [ ] Obvious labels do not have redundant tooltips.
- [ ] Filter placeholders clearly describe the unfiltered state.
- [ ] Button text uses consistent verbs and capitalization.
- [ ] Tooltips describe the result of the action, not implementation details.

## Dialogs
- [ ] Dialog title identifies the task or problem.
- [ ] Message text states what happened and what the user should do next.
- [ ] Destructive actions require explicit confirmation.
- [ ] Button order and wording are consistent with Windows conventions.
- [ ] Information, warning and error icons match the severity.

## Status messages
- [ ] Long-running actions announce when they start.
- [ ] Completion messages describe the result and relevant counts.
- [ ] Errors do not leave stale success text visible.
- [ ] Auto-save and last-saved indicators remain accurate.
- [ ] Messages use consistent sentence capitalization and punctuation.

## Icons and visual polish
- [ ] Icons share one visual family and consistent dimensions.
- [ ] Icons do not replace necessary text on unfamiliar actions.
- [ ] Contrast remains readable in normal Windows display settings.
- [ ] DataGrid headers, tabs, status bars and section borders look consistent.
- [ ] No visual change alters data, calculations, export or editing behavior.

## Regression check
- [ ] Add, Duplicate, Archive, Unarchive and Delete.
- [ ] Search, filters and Clear Filters.
- [ ] Material Detail editing and mouse caret placement.
- [ ] Tensile, Impact and Stiffness data entry.
- [ ] Website Export, Reports and Verification Center entry points.

## Status Messages & Final Polish
- Use short, action-first status wording.
- Use one punctuation pattern consistently.
- Show filenames rather than long full paths in compact status areas.
- Distinguish in-progress, success, no-change, and failure states clearly.
- Verify header and footer labels at normal and scaled Windows display settings.

## Non-blocking success feedback
- Routine successful actions use the shared transient status message.
- Success feedback returns to `Ready.` after approximately three seconds.
- Confirmation dialogs remain for destructive or replacement actions.
- Warning and error dialogs remain visible until acknowledged.
- Normal user-facing messages avoid internal storage implementation terminology.
