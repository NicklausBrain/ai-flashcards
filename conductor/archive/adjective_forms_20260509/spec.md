# Specification: Adjective Forms Support

## Overview
Implement support for Estonian adjective forms, including full declension (14 cases in singular and plural) for three degrees of comparison: Positive, Comparative, and Superlative. This will follow the established pattern for noun forms in the application.

## Functional Requirements
- **Data Models:** Define `AdjectiveForms` and related structures in `My1kWordsEe/Models/Grammar/`. It will include 14 cases for both Singular and Plural, across three degrees: Positive, Comparative, and Superlative.
- **AI Generation:** Update `AddEtFormsCommand.cs` and related logic to enable GPT-4o to generate these forms automatically.
- **UI Components:** 
    - Create `AdjectivesGrid.razor` to display the forms.
    - Update `FormsGrid.razor` to detect adjectives and render the new grid.
- **Persistence:** Ensure `FormsStorageClient.cs` handles the new `AdjectiveForms` type for saving/loading from Azure Blob Storage.

## Non-Functional Requirements
- **Consistency:** Use the same patterns as `NounForms` for naming and structure where possible.
- **Performance:** Ensure the UI remains snappy when loading large sets of forms.

## Acceptance Criteria
- [ ] Adjectives on the `WordPage` display a comprehensive forms table.
- [ ] All 14 cases are present for both singular and plural in all three degrees.
- [ ] Forms are correctly cached in Blob Storage after AI generation.
- [ ] The UI allows easy navigation between degrees (Positive, Comparative, Superlative).

## Out of Scope
- Games specifically for adjectives (these can be added in a future track).
- Handling of multiple word senses for adjectives (standard single-sense focus).
