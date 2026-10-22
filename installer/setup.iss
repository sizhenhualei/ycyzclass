; YcyzClass 安装包脚本（Inno Setup 6）
;
; 用法：
;   ISCC.exe /DAppVersion=1.4.0 /DSourceDir="<dotnet publish 输出目录>" /DRepoRoot="<仓库根>" /DOutputDir="<输出目录>" installer\setup.iss
;
; SourceDir 指向 dotnet publish 的输出目录（含 YcyzClass.Desktop.exe 与 Plugins\）。
; 本脚本按“扁平布局”安装：应用文件直接放进 {app}，并写入 PackageType=installer，
; 使应用把用户数据放到 %APPDATA%\YcyzClass\Data（与上游安装版一致，卸载不删数据）。
;
; 安装行为（内部使用，不询问）：
;   · 需要管理员权限（安装到 Program Files，并创建“最高权限”开机自启计划任务）
;   · 不显示许可协议页
;   · 自动创建桌面图标、开始菜单项与桌面“快捷换课”图标
;   · 自动创建登录时触发的计划任务 YcyzClass（静默以管理员身份自启）

#define AppName "YcyzClass"
#define AppExeName "YcyzClass.Desktop.exe"
#define AppPublisher "司振华蕾 x DeepSeekV4.1"
#define AppURL "https://github.com/sizhenhualei/ycyzclass/"

#ifndef AppVersion
  #define AppVersion GetCmdParam("AppVersion", "1.4.0")
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
; 不弹语言选择框：直接使用简体中文（系统语言匹配）
ShowLanguageDialog=no
; 与发布说明一致：仅支持 Windows 10 1809 (build 17763) 及以上
MinVersion=10.0.17763
; 需要管理员权限：安装到 Program Files，并创建“最高权限”的开机自启计划任务
; 需要管理员权限：安装到 Program Files，并创建“最高权限”的开机自启计划任务
#ifdef NoAdmin
; 仅用于本地视觉验收：/DNoAdmin 编译成免提权版本（安装位置改为用户目录）
PrivilegesRequired=lowest
#else
PrivilegesRequired=admin
#endif
OutputDir={#OutputDir}
OutputBaseFilename={#AppName}-{#AppVersion}-setup
SolidCompression=yes
WizardStyle=modern
; 品牌化向导外观：深色侧边图 + 应用图标徽标（配色在 [Code] 中统一设置）
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
; 内部使用，不再显示许可协议页（因此不设置 LicenseFile）
#if FileExists(AddBackslash(RepoRoot) + "YcyzClass\Assets\AppLogo.ico")
SetupIconFile={#RepoRoot}\YcyzClass\Assets\AppLogo.ico
#endif

[Messages]
SetupWindowTitle={#AppName} {#AppVersion} 安装程序
WelcomeLabel1=欢迎安装 [name]
WelcomeLabel2=安装程序将把 [name] 安装到本机（需要管理员权限）。%n%n· 自动创建桌面图标与「快捷换课」图标%n· 自动创建开机自启任务，登录后静默以管理员身份启动%n· 随包内置 3 个默认插件：以管理员身份自启动、UIAccess 提权、ExtraIsland%n· 用户数据（课表、配置、日志）保存在 %APPDATA%\YcyzClass\Data，卸载时不会被删除

[Languages]
Name: "chinesesimplified"; MessagesFile: "languages\ChineseSimplified.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
; 不提供可选项：桌面图标、开始菜单项一律创建
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
; 卸载时删除开机自启的计划任务（不存在时 schtasks 返回非零，忽略即可）
Filename: "{sys}\schtasks.exe"; Parameters: "/Delete /TN YcyzClass /F"; Flags: runhidden; RunOnceId: "DelAutoStartTask"

[UninstallDelete]
; PackageType 与桌面「快捷换课」图标由本脚本在安装结束时写入，不属于安装清单，卸载时需显式清理
Type: files; Name: "{app}\PackageType"
Type: files; Name: "{autodesktop}\快捷换课.url"
Type: dirifempty; Name: "{app}"

[Code]
const
  { Delphi TColor 为 $00BBGGRR 顺序 }
  DarkBg      = $00110D0D;   { #0D1111，与主应用深色背景一致 }
  PanelBg     = $001A1616;
  AccentColor = $00FF8D4C;   { #4C8DFF }
  TextColor   = $00F1F1F1;
  MutedColor  = $00B4B4B4;
  SwapShortcutName  = '快捷换课.url';
  AutoStartTaskName = 'YcyzClass';

{ 与应用一致的深色配色：页面背景改为 #0D1111，页内静态文字统一改为浅色 }
procedure ApplyDarkTheme();
var
  I, J: Integer;
  Page: TNewNotebookPage;
  Ctl: TControl;
begin
  WizardForm.Color := DarkBg;
  WizardForm.MainPanel.Color := PanelBg;

  for I := 0 to WizardForm.InnerNotebook.PageCount - 1 do
  begin
    Page := WizardForm.InnerNotebook.Pages[I];
    Page.Color := DarkBg;
    for J := 0 to Page.ControlCount - 1 do
    begin
      Ctl := Page.Controls[J];
      if Ctl is TNewStaticText then
        TNewStaticText(Ctl).Font.Color := TextColor
      else if Ctl is TNewCheckBox then
        TNewCheckBox(Ctl).Font.Color := TextColor
      else if Ctl is TNewRadioButton then
        TNewRadioButton(Ctl).Font.Color := TextColor;
    end;
  end;

  WizardForm.PageNameLabel.Font.Color := AccentColor;
  WizardForm.PageNameLabel.Font.Style := [fsBold];
  WizardForm.PageDescriptionLabel.Font.Color := MutedColor;

  WizardForm.WelcomeLabel1.Font.Color := AccentColor;
  WizardForm.WelcomeLabel1.Font.Size := 15;
  WizardForm.WelcomeLabel1.Font.Style := [fsBold];
  WizardForm.WelcomeLabel2.Font.Color := TextColor;

  WizardForm.FinishedHeadingLabel.Font.Color := AccentColor;
  WizardForm.FinishedHeadingLabel.Font.Style := [fsBold];
end;
procedure InitializeWizard();
begin
  ApplyDarkTheme();

  { 完成页补一句作者与仓库信息 }
  WizardForm.FinishedLabel.Caption := WizardForm.FinishedLabel.Caption + #13#10#13#10 +
    'YcyzClass 由 司振华蕾 x DeepSeekV4.1 维护，基于 ClassIsland 二次开发（GPL-3.0）。' + #13#10 +
    '项目地址：https://github.com/sizhenhualei/ycyzclass';
end;

{ 创建“登录时触发 + 最高权限”的计划任务，避免管理员态应用在登录时弹 UAC }
function CreateAutoStartTask(): Boolean;
var
  Cmd: String;
  ResultCode: Integer;
begin
  Cmd := Format('/Create /TN "%s" /TR "\"%s\" --autostartup" /SC ONLOGON /RL HIGHEST /F', [AutoStartTaskName, ExpandConstant('{app}\{#AppExeName}')]);
  Result := Exec(ExpandConstant('{sys}\schtasks.exe'), Cmd, '', SW_HIDE,
    ewWaitUntilTerminated, ResultCode) and (ResultCode = 0);
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  PackageTypePath: String;
  SwapPath: String;
  SwapContent: String;
begin
  { 标记为安装版：数据目录改用 %APPDATA%\YcyzClass\Data，卸载时不会删除用户数据 }
  if CurStep = ssPostInstall then
  begin
    PackageTypePath := ExpandConstant('{app}\PackageType');
    SaveStringToFile(PackageTypePath, 'installer', False);

    { 桌面「快捷换课」图标：Internet 快捷方式，指向 ycyzclass://app/class-swap }
    SwapPath := ExpandConstant('{autodesktop}\') + SwapShortcutName;
    SwapContent := '[InternetShortcut]' + #13#10 +
      'URL=ycyzclass://app/class-swap' + #13#10 +
      'IconFile=' + ExpandConstant('{app}\{#AppExeName}') + #13#10 +
      'IconIndex=0' + #13#10;
    SaveStringToFile(SwapPath, SwapContent, False);

    CreateAutoStartTask();
  end;
end;
