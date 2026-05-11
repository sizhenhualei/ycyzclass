using System.Diagnostics;
using System.Security.Principal;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace MigrateFromClassIsland;

class Program
{
    private static readonly string OldAppName = "ClassIsland";
    private static readonly string NewAppName = "YcyzClass";
    private static readonly string OldProcessName = "ClassIsland";
    private static readonly string NewProcessName = "YcyzClass";

    // Known data paths for ClassIsland
    private static readonly string OldAppData = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ClassIsland");
    private static readonly string NewAppData = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YcyzClass");

    private static readonly string OldLocalAppData = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ClassIsland");
    private static readonly string NewLocalAppData = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YcyzClass");

    static int Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("========================================");
        Console.WriteLine("  YcyzClass 迁移工具 v1.0.0");
        Console.WriteLine("  从 ClassIsland 迁移到 YcyzClass");
        Console.WriteLine("  作者: hualei x mimoai");
        Console.WriteLine("========================================");
        Console.WriteLine();

        // Check admin rights
        if (!IsAdministrator())
        {
            Console.WriteLine("[错误] 此工具需要管理员权限运行。");
            Console.WriteLine("请右键以管理员身份运行。");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
            return 1;
        }

        try
        {
            // Step 1: Stop ClassIsland process
            Console.WriteLine("[步骤 1/5] 停止 ClassIsland 进程...");
            StopProcess(OldProcessName);
            StopProcess(NewProcessName); // Also stop new one if running
            Console.WriteLine("  完成。");
            Console.WriteLine();

            // Step 2: Backup old data
            Console.WriteLine("[步骤 2/5] 备份 ClassIsland 数据...");
            string backupDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"ClassIsland_Backup_{DateTime.Now:yyyyMMdd_HHmmss}");
            BackupDirectory(OldAppData, backupDir, "AppData");
            BackupDirectory(OldLocalAppData, backupDir, "LocalAppData");
            Console.WriteLine($"  备份保存到: {backupDir}");
            Console.WriteLine();

            // Step 3: Copy new version (assume exe is in same directory)
            Console.WriteLine("[步骤 3/5] 部署 YcyzClass...");
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string targetDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "YcyzClass");
            CopyDirectory(currentDir, targetDir, new[] { "MigrateFromClassIsland.exe", "MigrateFromClassIsland.dll", "MigrateFromClassIsland.deps.json", "MigrateFromClassIsland.runtimeconfig.json" });
            Console.WriteLine($"  YcyzClass 已部署到: {targetDir}");
            Console.WriteLine();

            // Step 4: Migrate and rename config
            Console.WriteLine("[步骤 4/5] 迁移配置数据...");
            if (Directory.Exists(OldAppData))
            {
                CopyDirectory(OldAppData, NewAppData, Array.Empty<string>(), true);
                RenameConfigFiles(NewAppData);
                Console.WriteLine("  AppData 配置已迁移。");
            }
            if (Directory.Exists(OldLocalAppData))
            {
                CopyDirectory(OldLocalAppData, NewLocalAppData, Array.Empty<string>(), true);
                RenameConfigFiles(NewLocalAppData);
                Console.WriteLine("  LocalAppData 配置已迁移。");
            }
            Console.WriteLine();

            // Step 5: Create shortcut
            Console.WriteLine("[步骤 5/5] 创建桌面快捷方式...");
            string exePath = Path.Combine(targetDir, "YcyzClass.Desktop.exe");
            if (File.Exists(exePath))
            {
                CreateShortcut(exePath);
                Console.WriteLine("  桌面快捷方式已创建。");
            }
            else
            {
                Console.WriteLine("  [警告] 未找到 YcyzClass.Desktop.exe，跳过快捷方式创建。");
            }
            Console.WriteLine();

            Console.WriteLine("========================================");
            Console.WriteLine("  迁移完成！");
            Console.WriteLine("  旧版数据已备份到桌面。");
            Console.WriteLine("  YcyzClass 已安装到 Program Files。");
            Console.WriteLine("========================================");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[错误] 迁移失败: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
            return 1;
        }

        Console.WriteLine("按任意键退出...");
        Console.ReadKey();
        return 0;
    }

    private static bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private static void StopProcess(string processName)
    {
        foreach (var proc in Process.GetProcessesByName(processName))
        {
            try
            {
                Console.WriteLine($"  正在停止进程: {proc.ProcessName} (PID: {proc.Id})");
                proc.Kill();
                proc.WaitForExit(5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [警告] 无法停止进程 {proc.ProcessName}: {ex.Message}");
            }
        }
    }

    private static void BackupDirectory(string source, string backupRoot, string subName)
    {
        if (!Directory.Exists(source))
        {
            Console.WriteLine($"  目录不存在，跳过备份: {source}");
            return;
        }
        string target = Path.Combine(backupRoot, subName);
        CopyDirectory(source, target, Array.Empty<string>());
        Console.WriteLine($"  已备份: {source} -> {target}");
    }

    private static void CopyDirectory(string source, string target, string[] excludeFiles, bool overwrite = false)
    {
        if (!Directory.Exists(source)) return;
        Directory.CreateDirectory(target);

        foreach (var file in Directory.GetFiles(source))
        {
            string fileName = Path.GetFileName(file);
            if (excludeFiles.Contains(fileName, StringComparer.OrdinalIgnoreCase)) continue;
            string destFile = Path.Combine(target, fileName);
            File.Copy(file, destFile, overwrite);
        }

        foreach (var dir in Directory.GetDirectories(source))
        {
            string dirName = Path.GetFileName(dir);
            string destDir = Path.Combine(target, dirName);
            CopyDirectory(dir, destDir, excludeFiles, overwrite);
        }
    }

    private static void RenameConfigFiles(string directory)
    {
        if (!Directory.Exists(directory)) return;

        // Rename files containing "ClassIsland" in name
        foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            string fileName = Path.GetFileName(file);
            if (fileName.Contains("ClassIsland", StringComparison.OrdinalIgnoreCase))
            {
                string newFileName = fileName
                    .Replace("ClassIsland", "YcyzClass", StringComparison.OrdinalIgnoreCase)
                    .Replace("classisland", "ycyzclass", StringComparison.OrdinalIgnoreCase);
                string newPath = Path.Combine(Path.GetDirectoryName(file)!, newFileName);
                try
                {
                    if (File.Exists(newPath)) File.Delete(newPath);
                    File.Move(file, newPath);
                }
                catch { /* skip if locked */ }
            }
        }

        // Replace content in config files
        string[] configExts = { ".json", ".xml", ".yml", ".yaml", ".ini", ".config", ".txt", ".md" };
        foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            string ext = Path.GetExtension(file).ToLowerInvariant();
            if (!configExts.Contains(ext)) continue;

            try
            {
                string content = File.ReadAllText(file);
                string newContent = content
                    .Replace("ClassIsland", "YcyzClass")
                    .Replace("classIsland", "ycyzClass")
                    .Replace("classisland", "ycyzclass");
                if (content != newContent)
                {
                    File.WriteAllText(file, newContent);
                }
            }
            catch { /* skip if locked */ }
        }
    }

    private static void CreateShortcut(string targetExe)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string shortcutPath = Path.Combine(desktopPath, "YcyzClass.lnk");

        // Use PowerShell to create shortcut (works without COM reference)
        string ps = $@"
$ws = New-Object -ComObject WScript.Shell
$sc = $ws.CreateShortcut('{shortcutPath}')
$sc.TargetPath = '{targetExe}'
$sc.WorkingDirectory = '{Path.GetDirectoryName(targetExe)}'
$sc.Description = 'YcyzClass - 课表显示工具'
$sc.Save()
";
        var psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -Command "{ps.Replace(""", "\"")}"",
            UseShellExecute = false,
            CreateNoWindow = true
        };
        var proc = Process.Start(psi);
        proc?.WaitForExit(10000);
    }
}
