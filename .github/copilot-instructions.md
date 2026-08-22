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
- **Run unit tests with coverage (Cobertura):**
  ```bash
  dotnet test My1kWordsEe.Tests.Unit/My1kWordsEe.Tests.Unit.csproj --configuration Release --collect:"XPlat Code Coverage" --settings CodeCoverage.runsettings
  ```
- **Generate local coverage report (optional):**
  ```bash
  dotnet tool install --global dotnet-reportgenerator-globaltool
  reportgenerator -reports:"My1kWordsEe.Tests.Unit/TestResults/**/coverage.cobertura.xml" -targetdir:"My1kWordsEe.Tests.Unit/TestResults/CoverageReport" -reporttypes:"HtmlInline;MarkdownSummaryGithub;Cobertura"
  ```
- **No dedicated linter configured.**

## High-Level Architecture

- **Blazor Web Application** (My1kWordsEe):
  - Uses Blazor and Blazor Bootstrap for UI.
  - Main entry: `Program.cs` configures services, secrets, and telemetry.
  - **Vertical Feature Slices:** Code is organized under `Feature/` (`Favorites`, `Games`, `Grammar`, `Samples`, `Words`) rather than horizontal `Models`/`Services` layers. Each slice co-locates its models, CQS commands/queries, storage clients, state containers, and Razor `Pages`.
  - **CQS Pattern:** Business logic is organized into Command and Query Separation within each feature slice.
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
- **Feature Slices:** Feature code lives under `Feature/<slice>/`, co-locating models, CQS commands/queries, storage clients, state containers, and Razor `Pages`.
- **Shared Infrastructure:** Cross-cutting code lives in `Common/` — `Ai` (OpenAI, Stability AI, Tartu NLP clients), `Media`, and `Storage` (Azure Blob).
- **App-wide UI:** Shared Blazor components (`Account`, `Layout`, `Pages`) are in `Components/`; feature-specific pages live in each slice's `Pages/` folder.
- **Auth:** ASP.NET Core Identity (`ApplicationDbContext`, `ApplicationUser`) lives in `AuthData/`, backed by Cosmos DB.

## Keeping Docs in Sync

- Treat this file and `AGENTS.md` as living docs. When a change alters build/test commands, project structure, dependencies, secrets, or AI models, update both in the same PR.
- During code review, flag any drift between the code and these docs and propose the reconciling edit.
- Coverage standards: use `CodeCoverage.runsettings` for collection and keep CI coverage gate configuration in `.github/workflows/coverage_my-1k-ee.yml` aligned with current quality targets.

---

This file summarizes build/test commands, architecture, and conventions for Copilot and other AI agents. Adjust or request more coverage if needed.
