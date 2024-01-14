param(
    [Parameter(Mandatory = $true)]
    [string]$TEXT)

$headers = @{
    "accept" = "audio/wav"
}
$response = Invoke-WebRequest -Uri "https://api.tartunlp.ai/text-to-speech/v2" `
    -Method Post `
    -Headers $headers `
    -ContentType "application/json" `
    -Body "{`n  `"text`": `"$TEXT`",`n  `"speaker`": `"mari`",`n  `"speed`": 0.64`n}" `
    -OutFile "./eesti/audio/$TEXT.wav"

#mkdir "./eesti/audio"

#$response.Content > "./eesti/audio/$TEXT.wav"

return $response