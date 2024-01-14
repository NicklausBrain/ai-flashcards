$allAnimals = cat "./eesti/animals.json" | ConvertFrom-Json
$allAnimals | ForEach-Object {
    $animal = $_.et
    Write-Output $animal
    ./fetch-et-speech.ps1 $animal
}