param(
  [Parameter(Mandatory = $true)]
  [string]$BING_SEARCH_V7_KEY,
  [Parameter(Mandatory = $true)]
  [string]$CATEGORY,
  [Parameter(Mandatory = $true)]
  [string]$WORD) 


$response = Invoke-WebRequest -Uri "https://api.bing.microsoft.com/v7.0/images/search?q=$WORD+$CATEGORY" `
  -Headers @{'Ocp-Apim-Subscription-Key' = $BING_SEARCH_V7_KEY }

mkdir "./img/$CATEGORY"
$response.Content > "./img/$CATEGORY/bing-$WORD.json"

return $response

