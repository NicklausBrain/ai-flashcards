# Specification: Cache Noun Forms Game

## Problem
The `EtNoun3FormsGame` is currently generated on-the-fly by `EtNoun3FormsGameFactory` using OpenAI (GPT-4o). This consumes LLM credits every time a user plays the game, even if the word has been processed before.

## Goal
Implement a caching mechanism using `AzureStorageClient` to store and reuse generated `EtNoun3FormsGame` objects. This will reduce API costs and improve performance by avoiding redundant LLM calls.

## Proposed Solution
1. **Storage Location:** Use Azure Blob Storage (via `AzureStorageClient`) to store the JSON representation of the `EtNoun3FormsGame`.
2. **Keying:** The cache key should be based on the word and potentially its sense index.
3. **Logic Flow:**
    - When a game is requested for a word:
        1. Check if the game is already cached in Blob Storage.
        2. If found, deserialize and return it.
        3. If not found, generate it using the existing `EtNoun3FormsGameFactory` (OpenAI).
        4. Save the newly generated game to Blob Storage.
        5. Return the generated game.
4. **Integration:** Update `EtNoun3FormsGameFactory` (or a service calling it) to incorporate this logic.

## Acceptance Criteria
- `EtNoun3FormsGame` objects are successfully saved to Azure Blob Storage.
- Subsequent requests for the same word retrieve the game from the cache instead of calling the OpenAI API.
- Unit tests verify the "cache hit" and "cache miss" scenarios.
- The system correctly handles cases where the cache is empty or inaccessible.
