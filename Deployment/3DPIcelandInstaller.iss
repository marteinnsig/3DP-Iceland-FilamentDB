#ifndef SourceDir
  #error SourceDir must be supplied by the governed deployment build.
#endif
#ifndef OutputDir
  #error OutputDir must be supplied by the governed deployment build.
#endif
#ifndef AppVersion
  #error AppVersion must be supplied by the governed deployment build.
#endif

#define AppName "3DPIceland Engineering Platform"
#define AppExe "3DPIcelandFilamentDB.exe"

[Setup]
AppId={{3D7A0D23-DC7D-4A67-95F0-3D1CE9D43700}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=3DPIceland Labs
AppPublisherURL=https://www.iskort.is/3dp/
AppSupportURL=https://www.iskort.is/3dp/
DefaultDirName={localappdata}\Programs\3DPIceland Engineering Platform
DefaultGroupName=3DPIceland Engineering Platform
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir={#OutputDir}
OutputBaseFilename=3DPIceland-Setup-x64-v{#AppVersion}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
SetupIconFile={#SourceDir}\Assets\3dp-iceland-labs-icon.ico
UninstallDisplayIcon={app}\{#AppExe}
CloseApplications=yes
RestartApplications=no
LicenseFile={#SourceDir}\LICENSE

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\3DPIceland Engineering Platform"; Filename: "{app}\{#AppExe}"; WorkingDir: "{app}"
Name: "{autodesktop}\3DPIceland Engineering Platform"; Filename: "{app}\{#AppExe}"; WorkingDir: "{app}"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"; Flags: unchecked

[Run]
Filename: "{app}\{#AppExe}"; Description: "Launch 3DPIceland Engineering Platform"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; Deliberately empty. Inno removes only files installed under {app}; SQLite,
; backups, storage configuration and update transaction evidence live elsewhere.
