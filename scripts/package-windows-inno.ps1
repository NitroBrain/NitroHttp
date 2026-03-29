param(
    [string]$Version = "1.0.0",
    [string]$Project = "NitroHttp.csproj",
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [string]$AppName = "NitroHttp",
    [string]$Publisher = "NitroHttp"
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptDir "..")

$publishDir = Join-Path $repoRoot "publish\$Runtime"
$distDir = Join-Path $repoRoot "dist"
$issPath = Join-Path $scriptDir "windows-installer.iss"

if (-not (Test-Path $issPath)) {
    throw "Inno Setup script not found: $issPath"
}

if (-not (Test-Path $distDir)) {
    New-Item -ItemType Directory -Path $distDir | Out-Null
}

Write-Host "Publishing $AppName for $Runtime..."
dotnet publish $Project `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    /p:PublishSingleFile=true `
    /p:IncludeNativeLibrariesForSelfExtract=true `
    /p:PublishTrimmed=false `
    -o $publishDir

$isccPath = $null
$isccCommand = Get-Command iscc.exe -ErrorAction SilentlyContinue
if ($isccCommand) {
    $isccPath = $isccCommand.Path
} else {
    $candidatePaths = @(
        (Join-Path ${env:ProgramFiles(x86)} "Inno Setup 6\ISCC.exe"),
        (Join-Path $env:ProgramFiles "Inno Setup 6\ISCC.exe")
    )

    foreach ($candidate in $candidatePaths) {
        if ($candidate -and (Test-Path $candidate)) {
            $isccPath = $candidate
            break
        }
    }
}

if (-not $isccPath) {
    throw "Inno Setup Compiler (iscc.exe) was not found. Install Inno Setup 6 and add iscc.exe to PATH, or keep the default install location."
}

$sourceDirArg = ($publishDir -replace '/', '\\')
$outputDirArg = ($distDir -replace '/', '\\')

Write-Host "Building installer with Inno Setup..."
& $isccPath `
    "/DAppName=$AppName" `
    "/DAppVersion=$Version" `
    "/DAppPublisher=$Publisher" `
    "/DAppExeName=$AppName.exe" `
    "/DSourceDir=$sourceDirArg" `
    "/DOutputDir=$outputDirArg" `
    $issPath

Write-Host "Done. Installer output is in: $distDir"
