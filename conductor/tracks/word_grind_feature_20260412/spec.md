# Specification: Word Grind Feature

## Overview
The "Word Grind" feature is a new interactive game designed to help users practice a specific set of Estonian words. Users can define their own lists of words ("Word Sets") and then play a "cloze test" style game where they fill in blanks in AI-generated Estonian sentences.

## User Persona
- **Beginner to Intermediate Estonian Learner:** Wants to focus on specific vocabulary they find difficult.
- **Student:** Wants to practice words from a specific lesson or topic.

## Functional Requirements

### 1. Word Set Management
- **Create/Edit Word Sets:** A dedicated interface where users can enter a list of Estonian words into a text area. The input should support various delimiters (comma, newline, etc.).
- **Persistence:** Word Sets must be saved to the database (Azure Cosmos DB) so users can revisit them.
- **List Word Sets:** A page to view all saved word sets and select one for "grinding."
- **Delete Word Sets:** Ability to remove no longer needed sets.

### 2. "Word Grind" Game Generation
- **Sentence Generation:** For each word in a selected Word Set, the system will use OpenAI (GPT-4o) to generate a simple, natural Estonian sentence (A1-A2 level).
- **Cloze Test Creation:** The generated sentence will have the target word replaced with a blank (underscores).
- **Exact Match:** The sentence must use the *exact* form of the word provided in the Word Set.
- **Translation:** Each Estonian sentence should also have its English translation for context.

### 3. Game Interaction (Word Grind Page)
- **Batch Display:** All words and their corresponding sentences from the set are displayed on a single page.
- **User Input:** For each sentence, a text input field is provided for the user to type the missing word.
- **On Submission Feedback:** Feedback (correct/incorrect) is only provided after the user clicks the "Submit" button.
- **Scoring:** The system calculates a total score (e.g., 8/10 correct).
- **Give Up:** An option to reveal all correct answers and end the game.

## Non-Functional Requirements
- **Performance:** Sentence generation should be as fast as possible, with caching for frequently used words/sentences.
- **UI/UX:** The interface should be consistent with the existing "Noun 3 Forms" game, using a clean, card-based layout.
- **Error Handling:** Graceful handling of API failures (e.g., OpenAI timeout) with informative error messages.

## Acceptance Criteria
- [ ] Users can navigate to a "Word Grind" management page.
- [ ] Users can create a Word Set by pasting a list of words.
- [ ] Saved Word Sets appear in a list and can be edited or deleted.
- [ ] Selecting a Word Set starts the "Word Grind" game.
- [ ] The game displays one sentence per word with a blank.
- [ ] Submitting the game correctly identifies correct and incorrect answers.
- [ ] The total score is displayed upon submission.
- [ ] All data is persisted correctly in Azure Cosmos DB.

## Out of Scope
- Support for English-to-Estonian "Word Grind" (Estonian sentences with Estonian blanks only).
- Handling of different grammatical forms (exact word match only).
- Integration with external flashcard services (e.g., Anki).
