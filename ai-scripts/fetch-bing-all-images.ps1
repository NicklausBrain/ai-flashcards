

./keys.ps1

Write-Output $BING_SEARCH_V7_KEY

$allAnimals = cat "./eesti/animals.json" | ConvertFrom-Json
$allAnimals | ForEach-Object {
    $animal = $_.en
    Write-Output $animal
    # ./fetch-bing-image.ps1 $BING_SEARCH_V7_KEY "animal" $animal
    $imgMeta = cat "./img/animal/bing-$animal.json" | ConvertFrom-Json
    $url = $imgMeta.value[0].thumbnailUrl
    $_ | Add-Member imgUrl $url -Force
}
$allAnimals | ConvertTo-Json > "./eesti/animals.json"

$allFoods = cat "./eesti/food.json" | ConvertFrom-Json
$allFoods | ForEach-Object {
    $food = $_.en
    Write-Output $food
    #./fetch-bing-image.ps1 $BING_SEARCH_V7_KEY "food" $food
    $imgMeta = cat "./img/food/bing-$food.json" | ConvertFrom-Json
    $url = $imgMeta.value[0].thumbnailUrl
    $_ | Add-Member imgUrl $url -Force
}

$allFoods | ConvertTo-Json > "./eesti/food.json"

$allTransport = cat "./eesti/transport.json" | ConvertFrom-Json
$allTransport | ForEach-Object {
    $transport = $_.en
    Write-Output $transport
    #./fetch-bing-image.ps1 $BING_SEARCH_V7_KEY "transport" $_.en
    $imgMeta = cat "./img/transport/bing-$transport.json" | ConvertFrom-Json
    $url = $imgMeta.value[0].thumbnailUrl
    $_ | Add-Member imgUrl $url -Force
}

$allTransport | ConvertTo-Json > "./eesti/transport.json"

$allActivity = cat "./eesti/human-activity.json" | ConvertFrom-Json
$allActivity | ForEach-Object {
    $activity = $_.en
    Write-Output $activity
    #./fetch-bing-image.ps1 $BING_SEARCH_V7_KEY "human activity" $_.en
    $imgMeta = cat "./img/human-activity/bing-$activity.json" | ConvertFrom-Json
    $url = $imgMeta.value[1].thumbnailUrl
    $_ | Add-Member imgUrl $url -Force
}

$allActivity | ConvertTo-Json > "./eesti/human-activity.json"

#$j.value[0].thumbnailUrl 
#$j.value[0].contentUrl 
