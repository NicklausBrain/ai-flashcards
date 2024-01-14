# $allAnimals = cat "./eesti/animals.json" | ConvertFrom-Json
# $allAnimals | ForEach-Object {
#     $animal = $_.et
#     Write-Output $animal
#     ./fetch-et-speech.ps1 $animal
# }

$transport = cat "./eesti/transport.json" | ConvertFrom-Json
$transport | ForEach-Object {
    $t = $_.et
    Write-Output $t
    ./fetch-et-speech.ps1 $t
}

$food = cat "./eesti/food.json" | ConvertFrom-Json
$food | ForEach-Object {
    $f = $_.et
    Write-Output $f
    ./fetch-et-speech.ps1 $f
}

$activity = cat "./eesti/human-activity.json" | ConvertFrom-Json
$activity | ForEach-Object {
    $a = $_.et
    Write-Output $a
    ./fetch-et-speech.ps1 $a
}