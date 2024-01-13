

./keys.ps1

Write-Output $BING_SEARCH_V7_KEY

# $allAnimals = cat "./eesti/animals.json" | ConvertFrom-Json
# $allAnimals | ForEach-Object {
#     $animal = $_.en
#     Write-Output $animal
#     ./fetch-bing-image.ps1 $BING_SEARCH_V7_KEY "animal" $animal
# }

# $allFoods = cat "./eesti/food.json" | ConvertFrom-Json
# $allFoods | ForEach-Object {
#     $food = $_.en
#     Write-Output $food
#     ./fetch-bing-image.ps1 $BING_SEARCH_V7_KEY "food" $food
# }

$allFoods = cat "./eesti/transport.json" | ConvertFrom-Json
$allFoods | ForEach-Object {
    Write-Output $_.en
    ./fetch-bing-image.ps1 $BING_SEARCH_V7_KEY "transport" $_.en
}
