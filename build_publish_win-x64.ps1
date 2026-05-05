$ErrorActionPreference = "Stop"

$projectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Join-Path $projectRoot "src\Explorer_FolderView_Reset_Tool\Explorer_FolderView_Reset_Tool.csproj"
$publishDir = Join-Path $projectRoot "publish\win-x64-single-exe"

Write-Host "Project: $projectPath"
Write-Host "PublishDir: $publishDir"

dotnet restore $projectPath
dotnet build $projectPath -c Release -p:Platform="Any CPU" --no-restore
dotnet publish $projectPath -c Release -r win-x64 --self-contained true -p:Platform="Any CPU" -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o $publishDir --no-build

Write-Host ""
Write-Host "Publish completed."
Write-Host "Output: $publishDir"
