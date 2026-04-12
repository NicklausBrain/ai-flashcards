# Tech Stack: AI Flashcards (My1kWordsEe)

## Core Technologies
- **Language:** C# (.NET 10)
- **Frameworks:**
    - **Frontend:** Blazor (Interactive Server/Auto mode)
    - **Backend:** ASP.NET Core
    - **UI Library:** Blazor Bootstrap
- **Testing:**
    - **Unit Testing:** NUnit, xUnit, Moq
    - **E2E Testing:** Playwright (NUnit)

## Data Storage
- **Database:** Azure Cosmos DB (using EF Core Provider) for user authentication and authorization data.
- **Media Storage:** Azure Blob Storage for AI-generated images and audio files.
- **Persistence:** Azure Blob Storage for user-specific data, including favorites, game state, and custom word sets.
- **Local Cache:** JSON files for initial word seed data.

## AI & External Services
- **OpenAI (GPT-4o):** Used for translation verification and complex text/sentence generation.
- **Stability AI (SDXL/SD1.6):** Used for generating contextual images for flashcards.
- **Tartu NLP (neurokone.ee):** Used for high-quality Estonian speech synthesis.
- **Azure Application Insights:** For telemetry and monitoring.

## Infrastructure & DevOps
- **Hosting:** Azure App Service
- **CI/CD:** GitHub Actions (workflows for testing and deployment)
- **Secrets Management:** .NET User Secrets (local) and Azure Key Vault (production)
