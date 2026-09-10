$ErrorActionPreference = 'Stop'
Set-Location "$PSScriptRoot/.."
New-Item -ItemType Directory -Force build | Out-Null
dotnet build windows/Komorebi.csproj -c Release
if ($LASTEXITCODE -ne 0) { throw 'Windows compilation failed' }
Invoke-WebRequest 'https://go.microsoft.com/fwlink/p/?LinkId=2124703' -OutFile 'build/MicrosoftEdgeWebview2Setup.exe'
$signature = Get-AuthenticodeSignature 'build/MicrosoftEdgeWebview2Setup.exe'
if ($signature.Status -ne 'Valid' -or $signature.SignerCertificate.Subject -notmatch 'O=Microsoft Corporation') { throw 'Invalid Microsoft runtime bootstrapper signature' }
$iscc = "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
if (!(Test-Path $iscc)) { throw 'Install Inno Setup 6 before building the Windows installer.' }
& $iscc windows/installer.iss
if ($LASTEXITCODE -ne 0) { throw 'Installer compilation failed' }
