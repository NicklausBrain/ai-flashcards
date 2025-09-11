# Project: AI Flashcards

## Project Overview

This is a Blazor web application that helps users learn Estonian words through flashcards and interactive games. The application uses a variety of AI services to generate content and provide feedback to the user.

**Main Technologies:**

*   **Frontend:** Blazor, Blazor Bootstrap
*   **Backend:** .NET 9, ASP.NET Core
*   **Database:** Azure Cosmos DB (for user data), Azure Blob Storage (for media)
*   **AI Services:**
    *   **OpenAI (gpt-4o):** For text generation and translation checking.
    *   **Stability AI:** For image generation.
    *   **Tartu NLP:** For speech synthesis.

**Architecture:**

The project follows a modern ASP.NET Core architecture with a clear separation of concerns.

*   **`My1kWordsEe`:** The main Blazor application.
    *   **`Components`:** Contains the Blazor components for pages, layouts, and UI elements.
    *   **`Models`:** Defines the data structures for the application, including games, words, and user data.
    *   **`Services`:** Contains the business logic for the application, including services for interacting with AI APIs, managing data, and handling game logic. The project uses a Command and Query Separation (CQS) pattern to organize its services.
    *   **`Program.cs`:** The application's entry point, where services are configured and the application is initialized.
*   **`blazor-app`:** A deprecated proof-of-concept version of the application.
*   **`data`:** Contains data files and scripts for fetching data.

## Building and Running

To build and run the project, you will need to have the .NET 9 SDK installed.

1.  **Configure User Secrets:** The project uses user secrets to store API keys for the AI services. You will need to create a `secrets.json` file in the `My1kWordsEe` project directory with the following structure:

    ```json
    {
      "Secrets:OpenAi:ApiKey": "YOUR_OPENAI_API_KEY",
      "Secrets:StabilityAi:ApiKey": "YOUR_STABILITY_AI_API_KEY",
      "Secrets:Azure:Blob:ConnectionString": "YOUR_AZURE_BLOB_STORAGE_CONNECTION_STRING",
      "Secrets:AzureCosmosConnectionString": "YOUR_AZURE_COSMOS_DB_CONNECTION_STRING"
    }
    ```

2.  **Run the application:**

    ```bash
    dotnet run --project My1kWordsEe/My1kWordsEe.csproj
    ```

## Development Conventions

*   **Coding Style:** The project follows standard C# and .NET coding conventions.
*   **Testing:** The project includes unit tests (`My1kWordsEe.Tests.Unit`) and end-to-end tests (`My1kWordsEe.Tests.E2E`).
*   **CQS:** The project uses a Command and Query Separation (CQS) pattern to separate read and write operations. This helps to keep the code organized and maintainable.
*   **Dependency Injection:** The project makes extensive use of dependency injection to manage dependencies between services.
