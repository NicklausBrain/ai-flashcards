# ai-flashcards

flashcards for Estonian words and sentences memoization

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
