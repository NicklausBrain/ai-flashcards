param(
    [Parameter(Mandatory = $true)]
    [string]$TEXT)

$headers = @{
    "accept"       = "audio/wav";
    "Content-Type" = "application/json; charset=utf-8";
}

mkdir "./eesti/audio"

if (Test-Path "./eesti/audio/$TEXT.wav") {
    Write-Output "'$TEXT' is here"
    return;
}
else {
    $response = Invoke-WebRequest -Uri "https://api.tartunlp.ai/text-to-speech/v2" `
        -Method Post `
        -Headers $headers `
        -ContentType "application/json" `
        -Body "{`n  `"text`": `"$TEXT`",`n  `"speaker`": `"mari`",`n  `"speed`": 0.64`n}" `
        -OutFile "./eesti/audio/$TEXT.wav"
    return $response
}
