# ai-flashcards

flashcards for Estonian words and sentences memoization

## Grind Words Game

The "Grind Words" game is a new feature that helps users master Estonian vocabulary through rapid, repeated exposure and recall. It presents a series of words in quick succession, challenging users to recall meanings or translations under time pressure.

### Usage
- Access the Grind Words game from the main navigation or games section.
- Select a word set or difficulty level.
- The game will present words one by one; respond as quickly as possible.
- Immediate feedback is provided after each word.
- At the end, review your performance and retry missed words.

### Development Notes
- Implemented as a Blazor component in `Components/Games/GrindWords.razor`.
- Uses services from `Services/Cqs/` for word selection and scoring.
- Follows the CQS pattern for separation of game logic and UI.
- New conventions: All new games should be placed in `Components/Games/` and follow the CQS pattern for logic separation.


## ./My1kWordsEe

- current development
- https://my-1k-ee.azurewebsites.net/
- `gpt-4o` for texts
- [stability.ai](https://platform.stability.ai/docs/api-reference#tag/SDXL-and-SD1.6) for images
- [tartu nlp](https://neurokone.ee/) for speech
- [blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) + [blazor bootstrap](https://demos.blazorbootstrap.com/)

## Local setup

- [dotnet 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- setup the api keys

```
dotnet user-secrets set "Secrets:StabilityAiKey" "XYZ"
dotnet user-secrets set "Secrets:OpenAiKey" "XYZ"
dotnet user-secrets set "Secrets:AzureCosmosConnectionString" "XYZ"
dotnet user-secrets set "Secrets:AzureBlobConnectionString" "XYZ"
dotnet user-secrets set "Secrets:AppInsightsConnectionString" "XYZ"
```

## ./source-data

- most used Estonian words utilized as seed data to generate initial app content
