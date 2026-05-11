{
  lib,
  stdenv,
  fetchurl,
  autoPatchelfHook,
  makeShellWrapper,
  dpkg,
  fontconfig,
  hicolor-icon-theme,
  lttng-ust_2_12,
  libx11,
  libice,
  libsm,
  libxfixes,
  icu,
  openssl,
  alsa-lib,
}:
stdenv.mkDerivation rec {
  pname = "ycyzclass-bin";
  version = "2.0.2.0";
  src =
    {
      x86_64-linux = fetchurl {
        url = "https://github.com/YcyzClass/YcyzClass/releases/download/${version}/YcyzClass_app_linux_x64_selfContained_deb.deb";
        hash = "sha256-HR47u6ESQJaepeq5YScPLhnGnX7VMEtjEql92Ew6XTc=";
      };
      aarch64-linux = fetchurl {
        url = "https://github.com/YcyzClass/YcyzClass/releases/download/${version}/YcyzClass_app_linux_arm64_selfContained_deb.deb";
        hash = "sha256-Ao++dp+Z8cUOrnwwl7N9hxzXd2Wgy17gfgnXfGF3GK0=";
      };
    }
    .${stdenv.hostPlatform.system} or (throw "Unsupported system: ${stdenv.hostPlatform.system}");
  nativeBuildInputs = [
    autoPatchelfHook
    makeShellWrapper
    dpkg
    stdenv.cc.cc.lib
    lttng-ust_2_12
  ];
  buildInputs = [
    fontconfig
    hicolor-icon-theme
  ];
  installPhase = ''
    runHook preInstall
    mkdir -p $out/bin
    cp -r opt/apps $out/opt
    cp -r usr/share $out/share
    printf "deb" > "$out/opt/cn.ycyzclass.app/PackageType"
    substituteInPlace $out/share/applications/cn.ycyzclass.app.desktop \
      --replace-fail "/opt/apps/cn.ycyzclass.app/files/bin/YcyzClass.Desktop" "ycyzclass-bin"
    makeShellWrapper $out/opt/cn.ycyzclass.app/files/bin/YcyzClass.Desktop $out/bin/ycyzclass-bin \
      --set YcyzClass_PackageRoot "$out/opt/cn.ycyzclass.app" \
      --prefix LD_LIBRARY_PATH : "${
        lib.makeLibraryPath [
          icu
          libx11
          libice
          libsm
          libxfixes
          openssl
          alsa-lib
        ]
      }"
    runHook postInstall
  '';
}
