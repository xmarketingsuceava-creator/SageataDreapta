# Instaleaza Sageata Dreapta pe calculatorul acesta + shortcut pe Desktop.
# Se porneste cu dublu-click pe Instaleaza.bat, din folderul descarcat de pe GitHub (Code -> Download ZIP).
$ErrorActionPreference = 'Stop'
$here = $PSScriptRoot
$src = Join-Path $here 'SageataDreapta.exe'
$dest = Join-Path $env:LOCALAPPDATA 'SageataDreapta'
$exe = Join-Path $dest 'SageataDreapta.exe'

Write-Host ''
if (-not (Test-Path $src)) {
    Write-Host '  Nu gasesc SageataDreapta.exe langa installer.' -ForegroundColor Red
    Write-Host '  Dezarhiveaza intai tot ZIP-ul, apoi porneste Instaleaza.bat din folderul dezarhivat.'
    exit 1
}
Write-Host '  Instalez Sageata Dreapta...' -ForegroundColor Green

# Inchide versiunea veche, daca ruleaza, ca sa poata fi inlocuita
Get-Process SageataDreapta -ErrorAction SilentlyContinue | ForEach-Object { $_.CloseMainWindow() | Out-Null; $_.WaitForExit(3000) | Out-Null }

New-Item -ItemType Directory -Force $dest | Out-Null
Copy-Item $src $exe -Force
Unblock-File $exe -ErrorAction SilentlyContinue
$readme = Join-Path $here 'CITESTE.txt'
if (Test-Path $readme) { Copy-Item $readme $dest -Force }

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

Write-Host ''
Write-Host '  Gata! Ai "Sageata Dreapta" pe Desktop.' -ForegroundColor Green
Write-Host '  Poti sterge folderul descarcat.'
Write-Host ''
Write-Host '  Daca antivirusul il sterge: deschide "Windows Security" -> "Virus & threat' -ForegroundColor Yellow
Write-Host '  protection" -> "Protection history", gaseste Sageata Dreapta si apasa' -ForegroundColor Yellow
Write-Host '  "Allow" / "Restore". (scrie si in CITESTE.txt)' -ForegroundColor Yellow
