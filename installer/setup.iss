; YcyzClass 安装包脚本（Inno Setup 6）
;
; 用法：
;   ISCC.exe /DAppVersion=1.3.0 /DSourceDir="<dotnet publish 输出目录>" /DRepoRoot="<仓库根>" /DOutputDir="<输出目录>" installer\setup.iss
;
; SourceDir 指向 dotnet publish 的输出目录（含 YcyzClass.Desktop.exe 与 Plugins\）。
; 本脚本按“扁平布局”安装：应用文件直接放进 {app}，并写入 PackageType=installer，
; 使应用把用户数据放到 %APPDATA%\YcyzClass\Data（与上游安装版一致，卸载不删数据）。

#define AppName "YcyzClass"
#define AppExeName "YcyzClass.Desktop.exe"
#define AppPublisher "司振华蕾 x DeepSeekV4.1"
#define AppURL "https://github.com/sizhenhualei/ycyzclass/"

#ifndef AppVersion
  #define AppVersion GetCmdParam("AppVersion", "1.0.0")
#endif

#ifndef SourceDir
  #error 请通过 /DSourceDir=<发布输出目录> 指定待打包的文件目录
#endif

#ifndef OutputDir
  #define OutputDir "..\..\out_installer"
#endif

#ifndef RepoRoot
  #define RepoRoot GetEnv("GITHUB_WORKSPACE")
#endif

[Setup]
; 使用独立的 AppId，避免与上游 ClassIsland 的安装互相覆盖
AppId={{7E2A5D31-4C8B-4F19-9A24-6B0D8C1E5F72}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
VersionInfoVersion={#AppVersion}
VersionInfoCompany={#AppPublisher}
VersionInfoDescription={#AppName} 安装程序
VersionInfoProductName={#AppName}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
; 与发布说明一致：仅支持 Windows 10 1809 (build 17763) 及以上
MinVersion=10.0.17763
; 用户级安装（默认装到 %LOCALAPPDATA%\Programs\YcyzClass），不需要管理员权限
PrivilegesRequired=lowest
OutputDir={#OutputDir}
OutputBaseFilename={#AppName}-{#AppVersion}-setup
SolidCompression=yes
WizardStyle=modern
; 品牌化向导外观：深色侧边栏图 + 应用图标徽标
WizardImageStretch=yes
WizardResizable=no
#if FileExists(AddBackslash(RepoRoot) + "installer\assets\wizard-large.bmp")
WizardImageFile={#RepoRoot}\installer\assets\wizard-large.bmp
#endif
#if FileExists(AddBackslash(RepoRoot) + "installer\assets\wizard-small.bmp")
WizardSmallImageFile={#RepoRoot}\installer\assets\wizard-small.bmp
#endif
UninstallDisplayIcon={app}\{#AppExeName}
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
#if FileExists(AddBackslash(RepoRoot) + "LICENSE.txt")
LicenseFile={#RepoRoot}\LICENSE.txt
#endif
#if FileExists(AddBackslash(RepoRoot) + "YcyzClass\Assets\AppLogo.ico")
SetupIconFile={#RepoRoot}\YcyzClass\Assets\AppLogo.ico
#endif

[Messages]
SetupWindowTitle={#AppName} {#AppVersion} 安装程序
WelcomeLabel1=欢迎安装 [name]
WelcomeLabel2=本向导将把 [name] 安装到您的电脑上，安装过程不需要管理员权限。%n%n· 随包内置 3 个默认插件：以管理员身份自启动、UIAccess 提权、ExtraIsland%n· 用户数据（课表、配置、日志）保存在 %APPDATA%\YcyzClass\Data，卸载时不会被删除%n· 安装完成后可从桌面或开始菜单启动

[Languages]
Name: "chinesesimplified"; MessagesFile: "languages\ChineseSimplified.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
; 默认勾选：首次安装后直接在桌面看到图标，不必再进安装目录找 exe
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; PackageType 由本脚本在安装结束时写入，不属于安装清单，卸载时需显式清理
Type: files; Name: "{app}\PackageType"
Type: dirifempty; Name: "{app}"

[Code]
const
  { Delphi TColor 为 $00BBGGRR 顺序 }
  AccentColor = $00FF8D4C;   { #4C8DFF }
  MutedColor  = $00666666;

procedure InitializeWizard();
begin
  { 品牌配色：标题与页码标签使用主题蓝，描述文字使用次要灰 }
  WizardForm.WelcomeLabel1.Font.Color := AccentColor;
  WizardForm.WelcomeLabel1.Font.Size := 15;
  WizardForm.WelcomeLabel1.Font.Style := [fsBold];
  WizardForm.WelcomeLabel2.Font.Color := MutedColor;

  WizardForm.PageNameLabel.Font.Color := AccentColor;
  WizardForm.PageNameLabel.Font.Style := [fsBold];
  WizardForm.PageDescriptionLabel.Font.Color := MutedColor;

  WizardForm.FinishedHeadingLabel.Font.Color := AccentColor;
  WizardForm.FinishedHeadingLabel.Font.Style := [fsBold];
  WizardForm.FinishedLabel.Font.Color := MutedColor;

  { 完成页补一句作者与仓库信息 }
  WizardForm.FinishedLabel.Caption := WizardForm.FinishedLabel.Caption + #13#10#13#10 +
    'YcyzClass 由 司振华蕾 x DeepSeekV4.1 维护，基于 ClassIsland 二次开发（GPL-3.0）。' + #13#10 +
    '项目地址：https://github.com/sizhenhualei/ycyzclass';
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  PackageTypePath: String;
begin
  { 标记为安装版：数据目录改用 %APPDATA%\YcyzClass\Data，卸载时不会删除用户数据 }
  if CurStep = ssPostInstall then
  begin
    PackageTypePath := ExpandConstant('{app}\PackageType');
    SaveStringToFile(PackageTypePath, 'installer', False);
  end;
end;
