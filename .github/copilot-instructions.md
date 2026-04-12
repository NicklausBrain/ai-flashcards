# Copilot Instructions for ai-flashcards

## Build, Test, and Lint Commands

- **Build main app:**
  ```bash
  dotnet build My1kWordsEe/My1kWordsEe.csproj
  ```
- **Run all unit tests:**
  ```bash
  dotnet test My1kWordsEe.Tests.Unit/My1kWordsEe.Tests.Unit.csproj
  ```
- **Run all E2E tests:**
  ```bash
  dotnet test My1kWordsEe.Tests.E2E/My1kWordsEe.Tests.E2E.csproj
  ```
- **Run a single unit test:**
  ```bash
  dotnet test My1kWordsEe.Tests.Unit/My1kWordsEe.Tests.Unit.csproj --filter "FullyQualifiedName~<TestClassName>"
  ```
- **Run a single E2E test:**
  ```bash
  dotnet test My1kWordsEe.Tests.E2E/My1kWordsEe.Tests.E2E.csproj --filter "FullyQualifiedName~<TestClassName>"
  ```
- **No dedicated linter configured.**

## High-Level Architecture

- **Blazor Web Application** (My1kWordsEe):
  - Uses Blazor and Blazor Bootstrap for UI.
  - Main entry: `Program.cs` configures services, secrets, and telemetry.
  - **CQS Pattern:** Services are organized into Command and Query Separation for business logic.
  - **AI Integrations:**
    - OpenAI (text/translation)
    - Stability AI (image generation)
    - Tartu NLP (speech synthesis)
  - **Azure Cosmos DB** for user data, **Azure Blob Storage** for media.
  - **Dependency Injection** is used throughout for services.

- **Testing:**
  - Unit tests: `My1kWordsEe.Tests.Unit` (xUnit)
  - E2E tests: `My1kWordsEe.Tests.E2E` (NUnit, Playwright)

- **Scripts:**
  - PowerShell scripts in `/scripts` for data fetching and bug fixing.

## Key Conventions

- **Secrets:** Use `dotnet user-secrets` to set API keys (see README for keys required).
- **Source Data:** Place seed Estonian word lists in `/source-data`.
- **Service Organization:**
  - `Services/Cqs/` for command/query logic
  - `Services/Db/` for database access
  - `Services/Scoped/` for scoped services
- **Models:** All data structures are in `Models/`.
- **Components:** UI elements and pages are in `Components/`.

---

## Grind Words Game

- **Purpose:** Rapid-fire vocabulary recall game for Estonian words.
- **Location:** `Components/Games/GrindWords.razor` (UI), logic in `Services/Cqs/`.
- **How to Use:**
  - Navigate to the Grind Words game in the app.
  - Choose a word set or difficulty.
  - Respond to each word prompt quickly; feedback is immediate.
  - Review and retry missed words at the end.
- **Development Conventions:**
  - Place new games in `Components/Games/`.
  - Use CQS for game logic separation.
  - Use dependency injection for all services.

This file summarizes build/test commands, architecture, and conventions for Copilot and other AI agents. Adjust or request more coverage if needed.
