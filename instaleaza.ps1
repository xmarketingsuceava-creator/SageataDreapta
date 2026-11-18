# Instaleaza Sageata Dreapta pe calculatorul acesta + shortcut pe Desktop.
# Merge in doua feluri:
#  1. Din folderul descarcat (ZIP de pe GitHub): dublu-click pe Instaleaza.bat
#  2. Direct din internet, intr-o fereastra PowerShell:
#       irm https://raw.githubusercontent.com/xmarketingsuceava-creator/SageataDreapta/main/instaleaza.ps1 | iex
$ErrorActionPreference = 'Stop'
$repoRaw = 'https://raw.githubusercontent.com/xmarketingsuceava-creator/SageataDreapta/main'
$dest = Join-Path $env:LOCALAPPDATA 'SageataDreapta'
$exe = Join-Path $dest 'SageataDreapta.exe'

Write-Host ''
Write-Host '  Instalez Sageata Dreapta...' -ForegroundColor Green

# Opreste versiunea veche, daca ruleaza (altfel nu se poate inlocui exe-ul)
Get-Process SageataDreapta -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Milliseconds 300
New-Item -ItemType Directory -Force $dest | Out-Null

$here = if ($PSScriptRoot) { $PSScriptRoot } else { $null }
$local = if ($here) { Join-Path $here 'SageataDreapta.exe' } else { $null }
if ($local -and (Test-Path $local)) {
    Copy-Item $local $exe -Force
    if (Test-Path (Join-Path $here 'CITESTE.txt')) { Copy-Item (Join-Path $here 'CITESTE.txt') $dest -Force }
} else {
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest "$repoRaw/SageataDreapta.exe" -OutFile $exe -UseBasicParsing
    try { Invoke-WebRequest "$repoRaw/CITESTE.txt" -OutFile (Join-Path $dest 'CITESTE.txt') -UseBasicParsing } catch { }
}
Unblock-File $exe -ErrorAction SilentlyContinue

$shell = New-Object -ComObject WScript.Shell
$links = @(
    (Join-Path ([Environment]::GetFolderPath('Desktop')) 'Sageata Dreapta.lnk'),
    (Join-Path ([Environment]::GetFolderPath('Programs')) 'Sageata Dreapta.lnk')
)
foreach ($l in $links) {
    $sc = $shell.CreateShortcut($l)
    $sc.TargetPath = $exe
    $sc.WorkingDirectory = $dest
    $sc.IconLocation = "$exe,0"
    $sc.Description = 'Apasa automat sageata dreapta'
    $sc.Save()
}

Write-Host '  Gata! Ai "Sageata Dreapta" pe Desktop.' -ForegroundColor Green
Write-Host ''
