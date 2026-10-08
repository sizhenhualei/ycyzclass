using System.Reflection;
using System.Runtime.Versioning;
// GitInfo 类型由第三方源生成器 ClassIsland.SimpleGitInfoGenerator 生成，其命名空间固定为 ClassIsland，请勿随本项目重命名。
using ClassIsland;

#if NIX
[assembly: AssemblyVersion("0.0.0.0")]
[assembly: AssemblyInformationalVersion("NIXBUILD+NIXBUILD_LONG_HASH")]
#else
[assembly: AssemblyVersion(GitInfo.Tag)]
[assembly: AssemblyInformationalVersion($"{GitInfo.Tag}+{GitInfo.CommitHash}")]
#endif

[assembly: AssemblyTitle("YcyzClass")]
[assembly: AssemblyProduct("YcyzClass")]
[assembly: AssemblyCompany("司振华蕾 x DeepSeekV4.1")]
[assembly: AssemblyCopyright("Copyright (c) 2026 司振华蕾 x DeepSeekV4.1; based on ClassIsland, Copyright (c) 2024 HelloWRC")]
#if NETCOREAPP
// [assembly: SupportedOSPlatform("Windows")]
#endif
#if Platforms_MacOs
[assembly:SupportedOSPlatform("macos")]
#endif
 
