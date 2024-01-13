

./keys.ps1

Write-Output $BING_SEARCH_V7_KEY

$allAnimals = cat "./eesti/animals.json" | ConvertFrom-Json
$allAnimals | ForEach-Object {
    $animal = $_.en
    Write-Output $animal
    ./fetch-bing-image.ps1 $BING_SEARCH_V7_KEY "animal" $animal
}
