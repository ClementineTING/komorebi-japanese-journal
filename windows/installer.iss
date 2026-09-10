[Setup]
AppId={{1660A30D-D8BC-4E45-B37C-83C5B123CA61}
AppName=木漏れ日-日语学习唯美手帐
AppVersion=1.1.0
AppPublisher=Komorebi
AppPublisherURL=https://github.com/ClementineTING/komorebi-japanese-journal
DefaultDirName={localappdata}\Programs\Komorebi
DefaultGroupName=Komorebi
UninstallDisplayIcon={app}\Komorebi.exe
OutputDir=..\build
OutputBaseFilename=Komorebi-1.1.0-Windows-x64-Setup
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0.17763
WizardStyle=modern
[Files]
Source: "bin\Release\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\build\MicrosoftEdgeWebview2Setup.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall
[Icons]
Name: "{group}\Komorebi"; Filename: "{app}\Komorebi.exe"
Name: "{autodesktop}\Komorebi"; Filename: "{app}\Komorebi.exe"; Tasks: desktopicon
[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; Flags: unchecked
[Run]
Filename: "{tmp}\MicrosoftEdgeWebview2Setup.exe"; Parameters: "/silent /install"; StatusMsg: "Installing Microsoft WebView2 Runtime (internet required)..."; Check: NeedsWebView2; Flags: waituntilterminated
Filename: "{app}\Komorebi.exe"; Description: "Launch Komorebi"; Flags: nowait postinstall skipifsilent
[Code]
function NeedsWebView2: Boolean;
var Version: String;
begin
  Result := not (
    (RegQueryStringValue(HKCU, 'Software\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}', 'pv', Version) and (Version <> '') and (Version <> '0.0.0.0')) or
    (RegQueryStringValue(HKLM32, 'Software\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}', 'pv', Version) and (Version <> '') and (Version <> '0.0.0.0')) or
    (RegQueryStringValue(HKLM64, 'Software\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}', 'pv', Version) and (Version <> '') and (Version <> '0.0.0.0')));
end;
function InitializeSetup: Boolean;
begin
  Result := True;
  if NeedsWebView2 and not WizardSilent then
    Result := MsgBox('Komorebi needs Microsoft WebView2 Runtime. Setup will install it from Microsoft and requires an internet connection. Continue?', mbConfirmation, MB_YESNO) = IDYES;
end;
