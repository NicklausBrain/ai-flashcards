# Project: AI Flashcards

## Project Overview

This is a Blazor web application that helps users learn Estonian words through flashcards and interactive games. The application uses a variety of AI services to generate content and provide feedback to the user.

**Main Technologies:**

- **Frontend:** Blazor, Blazor Bootstrap
- **Backend:** .NET 10, ASP.NET Core
- **Database:** Azure Cosmos DB (for user data), Azure Blob Storage (for media)
- **AI Services:**
  - **OpenAI (gpt-4.1):** For text generation and translation checking.
  - **Stability AI:** For image generation.
  - **Tartu NLP:** For speech synthesis.

**Architecture:**

The project follows a modern ASP.NET Core architecture organized as vertical feature slices rather than horizontal `Models`/`Services` layers.

- **`My1kWordsEe`:** The main Blazor application.
  - **`Feature/`:** Vertical feature slices (`Favorites`, `Games`, `Grammar`, `Samples`, `Words`). Each slice co-locates its models, CQS commands/queries, storage clients, state containers, and Razor `Pages`.
  - **`Common/`:** Cross-cutting infrastructure shared across features — `Ai` (OpenAI, Stability AI, Tartu NLP clients), `Media`, `Storage` (Azure Blob), plus shared helpers and extensions.
  - **`Components/`:** App-wide Blazor UI (`Account`, `Layout`, `Pages`) plus `App.razor` and `Routes.razor`.
  - **`AuthData/`:** ASP.NET Core Identity `ApplicationDbContext` and `ApplicationUser` (backed by Cosmos DB).
  - **`Program.cs`:** The application's entry point, where services are registered (DI) and the app is configured.

The project uses a Command and Query Separation (CQS) pattern to separate read and write operations within each feature.

## Building and Running

To build and run the project, you will need to have the .NET 10 SDK installed.

1.  **Configure User Secrets:** The project uses `dotnet user-secrets` to store API keys and connection strings. All secrets are required at startup — the app throws on missing values. From the `My1kWordsEe` project directory, run:

    ```bash
    dotnet user-secrets set "Secrets:OpenAiKey" "YOUR_OPENAI_API_KEY"
    dotnet user-secrets set "Secrets:StabilityAiKey" "YOUR_STABILITY_AI_API_KEY"
    dotnet user-secrets set "Secrets:AzureBlobConnectionString" "YOUR_AZURE_BLOB_STORAGE_CONNECTION_STRING"
    dotnet user-secrets set "Secrets:AzureCosmosConnectionString" "YOUR_AZURE_COSMOS_DB_CONNECTION_STRING"
    dotnet user-secrets set "Secrets:AppInsightsConnectionString" "YOUR_APPLICATION_INSIGHTS_CONNECTION_STRING"
    ```

2.  **Run the application:**

    ```bash
    dotnet run --project My1kWordsEe/My1kWordsEe.csproj
    ```

## Development Conventions

- **Coding Style:** The project follows standard C# and .NET coding conventions.
- **Testing:** The project includes unit tests (`My1kWordsEe.Tests.Unit`) and end-to-end tests (`My1kWordsEe.Tests.E2E`).
- **CQS:** The project uses a Command and Query Separation (CQS) pattern to separate read and write operations. This helps to keep the code organized and maintainable.
- **Dependency Injection:** The project makes extensive use of dependency injection to manage dependencies between services.

## Test Coverage

- Coverage is collected with `coverlet.collector` using repo-level settings in `CodeCoverage.runsettings`.
- A dedicated CI workflow (`.github/workflows/coverage_my-1k-ee.yml`) enforces the minimum line-coverage gate.
- Local coverage run (unit tests):

  ```bash
  dotnet test My1kWordsEe.Tests.Unit/My1kWordsEe.Tests.Unit.csproj --configuration Release --collect:"XPlat Code Coverage" --settings CodeCoverage.runsettings
  ```

- Optional local HTML report (requires tool):

  ```bash
  dotnet tool install --global dotnet-reportgenerator-globaltool
  reportgenerator -reports:"My1kWordsEe.Tests.Unit/TestResults/**/coverage.cobertura.xml" -targetdir:"My1kWordsEe.Tests.Unit/TestResults/CoverageReport" -reporttypes:"HtmlInline;MarkdownSummaryGithub;Cobertura"
  ```

## Keeping Docs in Sync

- Treat this file (and `.github/copilot-instructions.md`) as living docs; When a change alters build/test commands, project structure, dependencies, secrets, or AI models, update both in the same PR.
- During code review, flag any drift between the code and these docs and propose the reconciling edit.
