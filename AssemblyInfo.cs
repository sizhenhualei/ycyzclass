using System.Reflection;
using System.Runtime.Versioning;
using YcyzClass;

#if NIX
[assembly: AssemblyVersion("0.0.0.0")]
[assembly: AssemblyInformationalVersion("NIXBUILD+NIXBUILD_LONG_HASH")]
#else
[assembly: AssemblyVersion(GitInfo.Tag)]
[assembly: AssemblyInformationalVersion($"{GitInfo.Tag}+{GitInfo.CommitHash}")]
#endif

[assembly: AssemblyTitle("YcyzClass")]
[assembly: AssemblyProduct("YcyzClass")]
#if NETCOREAPP
// [assembly: SupportedOSPlatform("Windows")]
#endif
#if Platforms_MacOs
[assembly:SupportedOSPlatform("macos")]
#endif
 
