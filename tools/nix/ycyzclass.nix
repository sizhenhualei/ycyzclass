{
  dotnetCorePackages,
  buildDotnetModule,
  libx11,
  libice,
  libsm,
  libxfixes,
  fontconfig,
  git,
  stdenv,
  autoPatchelfHook,
  makeDesktopItem,
}:
let
  desktopItem = makeDesktopItem {
    type = "Application";
    name = "cn.ycyzclass.app";
    desktopName = "YcyzClass";
    icon = "cn.ycyzclass.app";
    exec = "ycyzclass";
    terminal = false;
    startupNotify = true;
    comment = "功能强大、可定制、跨平台的大屏课表显示工具。";
    categories = [
      "Education"
      "Office"
    ];
  };
in
buildDotnetModule {
  pname = "ycyzclass";
  version = "2.0.2.0";
  projectFile = "./YcyzClass.Desktop/YcyzClass.Desktop.csproj";
  dotnet-sdk =
    with dotnetCorePackages;
    (combinePackages [
      sdk_9_0
      sdk_8_0
      sdk_6_0
    ]);
  dotnet-runtime = dotnetCorePackages.runtime_8_0;
  src = ../../.;
  # nix build .#ycyzclass.passthru.fetch-deps && ./result ./tools/nix/deps.json
  # 生成完后可能需要手动修改
  nugetDeps = ./deps.json;
  doCheck = true;
  dotnetBuildFlags = [
    "--property:NIX=true"
  ];
  runtimeDeps = [
    libx11
    libice
    libsm
    libxfixes
    fontconfig
  ];
  executables = [ "YcyzClass.Desktop" ];
  nativeBuildInputs = [
    git
    stdenv.cc.cc.lib
    autoPatchelfHook
  ];
  postInstall = ''
    mkdir -p $out/share/applications
    cp ${desktopItem}/share/applications/cn.ycyzclass.app.desktop $out/share/applications/cn.ycyzclass.app.desktop 
    mkdir -p $out/share/icons/hicolor/scalable/apps/
    cp YcyzClass.Desktop/Assets/AppLogo.svg $out/share/icons/hicolor/scalable/apps/cn.ycyzclass.app.svg
    printf deb > $out/lib/ycyzclass/PackageType
  '';
  postFixup = ''
    mv $out/bin/YcyzClass.Desktop $out/bin/ycyzclass
  '';
  packNupkg = false;
}
