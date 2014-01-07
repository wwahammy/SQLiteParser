Param (
    $variables = @{},   
    $artifacts = @{},
    $scriptPath,
    $buildFolder,
    $srcFolder,
    $outFolder,
    $tempFolder,
    $projectName,
    $projectVersion,
    $projectBuildNumber,
    $secureNuGetApiKey,
    $nugetApiUri
)

# list all artifacts
foreach($artifact in $artifacts.values)
{
    Write-Output "Artifact: $($artifact.name)"
    Write-Output "Type: $($artifact.type)"
    Write-Output "Path: $($artifact.path)"
    Write-Output "Url: $($artifact.url)"
}

cd $srcFolder
Write-Output "Source Folder: $srcFolder"
Write-Output ".\.nuget\nuget.exe push $($artifacts[0].path) $secureNuGetApiKey -Source $($nugetApiUri)"
.\.nuget\nuget.exe push $($artifacts[0].path) $($secureNuGetApiKey) -Source $($nugetApiUri)
