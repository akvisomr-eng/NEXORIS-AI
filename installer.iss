#define AppName "NEXORIS AI"
#define AppVersion "0.1.0"
#define AppPublisher "akvisomr-eng"
#define AppExeName "NEXORIS.AI.exe"
[Setup]
AppId={{8F9324E2-57AF-49A4-A2BC-7E25A2F56D10}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\NEXORIS AI
DefaultGroupName=NEXORIS AI
OutputDir=artifacts\installer
OutputBaseFilename=NEXORIS-AI-Setup-{#AppVersion}-win-x64
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=lowest
UninstallDisplayIcon={app}\{#AppExeName}
[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked
[Files]
Source: "artifacts\NEXORIS-AI-win-x64\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
[Icons]
Name: "{group}\NEXORIS AI"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\NEXORIS AI"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon
[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch NEXORIS AI"; Flags: postinstall nowait skipifsilent