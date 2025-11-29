$headers = @{
  "Authorization" = "Bearer $OPENAI_API_KEY"
}
$prompt = "Top 10 topics or word categories when you learn a new language"
$response = Invoke-WebRequest -Uri "https://api.openai.com/v1/chat/completions" `
  -Method Post `
  -Headers $headers `
  -ContentType "application/json" `
  -Body "{
      `"model`": `"gpt-3.5-turbo-16k-0613`",
      `"messages`": [
        {
            `"role`": `"user`",
            `"content`": `"$prompt`"
        }
        ],
        `"temperature`": 1,
        `"max_tokens`": 256,
        `"top_p`": 1,
        `"frequency_penalty`": 0,
        `"presence_penalty`": 0`n}"

return $response

