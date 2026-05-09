# Implementation Plan: Adjective Forms Support

## Phase 1: Data Models & Storage (The "Noun-like" Foundation) [checkpoint: aabb48a]
- [x] Task: Create `AdjectiveDegreeForms` struct to hold Singular and Plural `NounForm` arrays (reusing `NounForm` for its 14-case structure).
- [x] Task: Refactor `AdjectiveForms.cs` to implement `IGrammarForms` and include `Positive`, `Comparative`, and `Superlative` degrees using `AdjectiveDegreeForms`.
- [x] Task: Add static helper methods to `AdjectiveForms` for question strings, similar to the existing `NounForm` helpers.
- [x] Task: Write unit tests in `My1kWordsEe.Tests.Unit` to verify that `AdjectiveForms` can be serialized and deserialized correctly.
- [x] Task: Conductor - User Manual Verification 'Phase 1: Data Models & Storage' (Protocol in workflow.md)

## Phase 2: AI Generation & CQS Integration [checkpoint: f12fa60]
- [x] Task: Write unit tests for `AddEtFormsCommand` to ensure it correctly requests and parses `AdjectiveForms` from OpenAI.
- [x] Task: Verify that `GetOrAddEtFormsCommand` correctly routes requests for adjectives to `AddEtFormsCommand.Invoke<AdjectiveForms>`.
- [x] Task: Conductor - User Manual Verification 'Phase 2: AI Generation & CQS Integration' (Protocol in workflow.md)

## Phase 3: UI Components & Integration
- [ ] Task: Create `AdjectivesGrid.razor` that displays the forms in a tabular format, with a selector (tabs or dropdown) for the three degrees.
- [ ] Task: Update `FormsGrid.razor` to detect adjectives and render the `AdjectivesGrid`.
- [ ] Task: Manual verification on the `WordPage` with a sample adjective (e.g., "suur").
- [ ] Task: Conductor - User Manual Verification 'Phase 3: UI Components & Integration' (Protocol in workflow.md)
