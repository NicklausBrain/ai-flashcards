param(
  [Parameter(Mandatory=$true)]
  [string]$OPENAI_API_KEY,
  [Parameter(Mandatory=$true)]
  [string]$CATEGORY) 

$headers = @{
  "Authorization" = "Bearer $OPENAI_API_KEY"
}

$prompt = @"
Top 100 words in category of $CATEGORY. 2 languages: english and estonian.
Return result in JSON array format:
[{
  "en": "word in english"
  "et": "word in estonian"
}]
"@ | ConvertTo-Json

$body = "{
  `"model`": `"gpt-3.5-turbo-16k-0613`",
  `"messages`": [
    {
        `"role`": `"user`",
        `"content`": $prompt
    }
    ],
    `"temperature`": 1,
    
    `"top_p`": 1,
    `"frequency_penalty`": 0,
    `"presence_penalty`": 0`n}"
Write-Output $body
$response = Invoke-WebRequest -Uri "https://api.openai.com/v1/chat/completions" `
  -Method Post `
  -Headers $headers `
  -ContentType "application/json" `
  -Body $body

$content = $response.Content | ConvertFrom-Json
$innerContent = $content.choices[0].message.content

$innerContent > "./eesti/$CATEGORY.json"

return $response;
