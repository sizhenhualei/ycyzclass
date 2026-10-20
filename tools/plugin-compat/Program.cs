using System.Text.RegularExpressions;
using Mono.Cecil;

namespace PluginCompat;

/// <summary>
/// 把按上游 ClassIsland ABI 编译的插件二进制改写为 YcyzClass ABI：
///  - 程序集引用 ClassIsland.* -> YcyzClass.*
///  - 类型引用命名空间 ClassIsland.* -> YcyzClass.*
///  - 字符串字面量中的 avares://ClassIsland / ClassIsland.xxx / classisland://
/// analyze 模式只做内存改写并检查所有对 YcyzClass 程序集的类型/成员引用是否真实存在。
/// </summary>
internal static class Program
{
    private const string From = "ClassIsland";
    private const string To = "YcyzClass";

    private static int Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("usage: plugincompat <analyze|convert> <pluginDir> <appDir|outDir>");
            return 2;
        }

        var mode = args[0].ToLowerInvariant();
        var pluginDir = Path.GetFullPath(args[1]);
        var target = Path.GetFullPath(args[2]);
        if (!Directory.Exists(pluginDir))
        {
            Console.WriteLine($"pluginDir not found: {pluginDir}");
            return 2;
        }

        return mode switch
        {
            "analyze" => Analyze(pluginDir, target),
            "convert" => ConvertDir(pluginDir, target),
            _ => 2,
        };
    }

    private static Dictionary<string, AssemblyDefinition> LoadAppAssemblies(string appDir)
    {
        var result = new Dictionary<string, AssemblyDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (var path in Directory.EnumerateFiles(appDir, "*.dll", SearchOption.TopDirectoryOnly))
        {
            try
            {
                var asm = AssemblyDefinition.ReadAssembly(path);
                result[asm.Name.Name] = asm;
            }
            catch
            {
                // 原生 dll 或无法读取的文件，忽略
            }
        }

        return result;
    }

    private static int Analyze(string pluginDir, string appDir)
    {
        var appAsms = LoadAppAssemblies(appDir);
        Console.WriteLine($"app assemblies loaded: {appAsms.Count} from {appDir}");
        var missingTypes = new List<string>();
        var missingMembers = new List<string>();
        var scanned = 0;

        foreach (var path in Directory.EnumerateFiles(pluginDir, "*.dll", SearchOption.AllDirectories))
        {
            using var mod = TryRead(path);
            if (mod is null)
            {
                continue;
            }

            var rel = Path.GetRelativePath(pluginDir, path);
            if (NeedsRewrite(mod))
            {
                Rewrite(mod);
            }

            if (!mod.GetTypeReferences().Any(x => x.Scope is AssemblyNameReference s && s.Name.StartsWith(To, StringComparison.Ordinal)))
            {
                continue; // 与本项目 ABI 无关的程序集
            }

            scanned++;

            foreach (var tr in mod.GetTypeReferences())
            {
                if (tr.Scope is not AssemblyNameReference scope || !scope.Name.StartsWith(To, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!appAsms.TryGetValue(scope.Name, out var asm))
                {
                    missingTypes.Add($"{rel}: 缺少程序集 {scope.Name}（{tr.FullName}）");
                    continue;
                }

                var elem = tr.GetElementType();
                if (asm.MainModule.GetType(elem.FullName) is null)
                {
                    missingTypes.Add($"{rel}: 缺少类型 {elem.FullName}");
                }
            }

            foreach (var mr in mod.GetMemberReferences())
            {
                if (mr.DeclaringType?.Scope is not AssemblyNameReference scope2 || !scope2.Name.StartsWith(To, StringComparison.Ordinal))
                {
                    continue;
                }

                if (!appAsms.TryGetValue(scope2.Name, out var asm2))
                {
                    continue;
                }

                var declType = asm2.MainModule.GetType(mr.DeclaringType.GetElementType().FullName);
                if (declType is null)
                {
                    continue; // 已在类型检查里报告
                }

                var found = mr switch
                {
                    MethodReference m => declType.Methods.Any(x => x.Name == m.Name && x.Parameters.Count == m.Parameters.Count),
                    FieldReference f => declType.Fields.Any(x => x.Name == f.Name),
                    PropertyReference p => declType.Properties.Any(x => x.Name == p.Name),
                    EventReference e => declType.Events.Any(x => x.Name == e.Name),
                    _ => true,
                };
                if (!found)
                {
                    missingMembers.Add($"{rel}: 缺少成员 {mr.DeclaringType.FullName}::{mr.Name}");
                }
            }
        }

        var distinctTypes = missingTypes.Distinct().ToList();
        var distinctMembers = missingMembers.Distinct().ToList();
        Console.WriteLine($"scanned {scanned} plugin assemblies");
        Console.WriteLine($"missing types: {distinctTypes.Count}");
        foreach (var m in distinctTypes) Console.WriteLine("  T " + m);
        Console.WriteLine($"missing members: {distinctMembers.Count}");
        foreach (var m in distinctMembers) Console.WriteLine("  M " + m);
        return distinctTypes.Count + distinctMembers.Count == 0 ? 0 : 1;
    }

    private static int ConvertDir(string pluginDir, string outDir)
    {
        Directory.CreateDirectory(outDir);
        var rewritten = 0;
        var copied = 0;
        foreach (var src in Directory.EnumerateFiles(pluginDir, "*", SearchOption.AllDirectories))
        {
            var rel = Path.GetRelativePath(pluginDir, src);
            if (rel.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase))
            {
                continue; // 符号文件与改写后的二进制不匹配，跳过
            }

            var dst = Path.Combine(outDir, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(dst)!);

            if (src.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                using var mod = TryRead(src);
                if (mod is not null && NeedsRewrite(mod))
                {
                    Rewrite(mod);
                    mod.Write(dst);
                    rewritten++;
                    Console.WriteLine($"  rewrite {rel}");
                    continue;
                }
            }

            File.Copy(src, dst, true);
            copied++;
        }

        Console.WriteLine($"converted: rewritten={rewritten} copied={copied} -> {outDir}");
        return 0;
    }

    private static ModuleDefinition? TryRead(string path)
    {
        try
        {
            return ModuleDefinition.ReadModule(path, new ReaderParameters
            {
                AssemblyResolver = new NullResolver(),
                InMemory = true,
            });
        }
        catch
        {
            return null;
        }
    }

    private static bool NeedsRewrite(ModuleDefinition mod)
    {
        if (mod.AssemblyReferences.Any(x => IsBrand(x.Name)))
        {
            return true;
        }

        foreach (var tr in mod.GetTypeReferences())
        {
            if (IsBrand(tr.Namespace))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsBrand(string? value) =>
        !string.IsNullOrEmpty(value) &&
        (value == From || value.StartsWith(From + ".", StringComparison.Ordinal));

    private static string RewriteNamespace(string? ns) =>
        IsBrand(ns) ? To + ns![From.Length..] : ns ?? string.Empty;

    private static void Rewrite(ModuleDefinition mod)
    {
        foreach (var anr in mod.AssemblyReferences)
        {
            if (IsBrand(anr.Name))
            {
                anr.Name = To + anr.Name[From.Length..];
            }
        }

        foreach (var tr in mod.GetTypeReferences())
        {
            if (IsBrand(tr.Namespace))
            {
                tr.Namespace = RewriteNamespace(tr.Namespace);
            }
        }

        foreach (var et in mod.ExportedTypes)
        {
            if (IsBrand(et.Namespace))
            {
                et.Namespace = RewriteNamespace(et.Namespace);
            }
        }

        foreach (var type in mod.GetTypes())
        {
            foreach (var method in type.Methods)
            {
                if (!method.HasBody)
                {
                    continue;
                }

                foreach (var ins in method.Body.Instructions)
                {
                    if (ins.Operand is string s && s.Contains(From, StringComparison.Ordinal))
                    {
                        var rewritten = RewriteString(s);
                        if (rewritten != s)
                        {
                            ins.Operand = rewritten;
                        }
                    }
                }
            }
        }
    }

    private static string RewriteString(string value)
    {
        var s = value
            .Replace("avares://" + From, "avares://" + To, StringComparison.Ordinal)
            .Replace("resm://" + From, "resm://" + To, StringComparison.Ordinal)
            .Replace("classisland://", "ycyzclass://", StringComparison.OrdinalIgnoreCase);
        s = Regex.Replace(s, @"\bClassIsland(?=\.)", To);
        return s;
    }

    private sealed class NullResolver : IAssemblyResolver
    {
        public AssemblyDefinition? Resolve(AssemblyNameReference name) => null;

        public AssemblyDefinition? Resolve(AssemblyNameReference name, ReaderParameters parameters) => null;

        public void Dispose()
        {
        }
    }
}
