# Implementation Plan - Word Grind Feature

## Phase 1: Data Models and Persistence [checkpoint: 814aa91]
Goal: Establish the underlying data structures and storage mechanisms for user-defined Word Sets using Azure Blob Storage.

- [x] Task: Define `WordSet` model. 814aa91
    - [x] Create `My1kWordsEe/Models/WordSet.cs` with properties: `Id` (string/guid), `Name` (string), `Words` (List<string>), `CreatedAt` (DateTime), `UpdatedAt` (DateTime).
- [x] Task: Implement `WordSetStorageClient`. 814aa91
    - [x] Create `My1kWordsEe/Services/Db/WordSetStorageClient.cs`.
    - [x] Inject `AzureStorageClient` and `IConfiguration`.
    - [x] Implement CRUD operations using Azure Blob Storage (container: `word-sets`).
    - [x] Map each `WordSet` to a JSON blob in the container.
- [x] Task: TDD - Test `WordSetStorageClient`. 814aa91
    - [x] Write unit tests in `My1kWordsEe.Tests.Unit/Services/Db/WordSetStorageClientTests.cs` (mocking `AzureStorageClient`) to verify saving, retrieving, listing, and deleting word sets.
    - [x] Ensure tests pass.
- [x] Task: Conductor - User Manual Verification 'Phase 1: Data Models and Persistence' (Protocol in workflow.md) 814aa91

## Phase 2: Game Logic and Generation [checkpoint: 208c3e9]
Goal: Implement the core logic for generating "Word Grind" games using AI and managing game state.

- [x] Task: Define `WordGrindGame` and `WordGrindGameData` models. 208c3e9
    - [x] Create `My1kWordsEe/Models/Games/WordGrindGame.cs` and related data structures.
- [x] Task: Implement `WordGrindGameFactory`. 208c3e9
    - [x] Create `My1kWordsEe/Models/Games/Generation/WordGrindGameFactory.cs`.
    - [x] Implement AI prompt for generating Estonian sentences with blanks for specific words.
    - [x] Integrate with `OpenAiClient`.
    - [x] Implement caching for generated game data using `GameStorageClient` (which also uses Azure Blob Storage).
- [x] Task: TDD - Test `WordGrindGameFactory` and `WordGrindGame`. 208c3e9
    - [x] Write unit tests in `My1kWordsEe.Tests.Unit/Models/Games/WordGrindGameTests.cs` for scoring and submission logic.
    - [x] Write unit tests for the factory (mocking OpenAI) to verify correct mapping and caching.
    - [x] Ensure tests pass.
- [x] Task: Conductor - User Manual Verification 'Phase 2: Game Logic and Generation' (Protocol in workflow.md) 208c3e9

## Phase 3: UI Implementation [checkpoint: e5a32b1]
Goal: Create the user interface for managing word sets and playing the Word Grind game.

- [x] Task: Create `WordSetsPage.razor` for Word Set management. e5a32b1
    - [x] List existing sets.
    - [x] Provide a form (text area) to create/edit a set.
    - [x] Implement delete functionality.
- [x] Task: Create `WordGrindGamePage.razor` for playing. e5a32b1
    - [x] Use a batch layout (all sentences on one page) consistent with `EtNoun3FormsGamePage.razor`.
    - [x] Implement `MaskWord` helper or reuse existing logic.
    - [x] Handle submission and display final score/feedback.
- [x] Task: Integration and Navigation. e5a32b1
    - [x] Add "Word Grind" to the `NavMenu.razor`.
    - [x] Ensure seamless navigation between management and game pages.
- [x] Task: Conductor - User Manual Verification 'Phase 3: UI Implementation' (Protocol in workflow.md) e5a32b1
