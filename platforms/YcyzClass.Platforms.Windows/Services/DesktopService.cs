using System.Diagnostics;
using YcyzClass.Core;
using YcyzClass.Helpers;
using YcyzClass.Platforms.Abstraction.Services;
using WindowsShortcutFactory;

namespace YcyzClass.Platform.Windows.Services;

public class DesktopService : IDesktopService
{
    /// <summary>
    /// 开机自启使用的计划任务名称。
    /// 应用以管理员权限运行（应用清单 requireAdministrator），若改用启动文件夹快捷方式，
    /// 每次登录都会弹出 UAC 提示；改为「登录时触发、最高权限」的计划任务后即可静默自启。
    /// </summary>
    private const string AutoStartTaskName = "YcyzClass";

    private static string StartupShortcutPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Startup), "YcyzClass.lnk");

    public bool IsAutoStartEnabled
    {
        get => File.Exists(StartupShortcutPath) || ScheduledTaskExists();
        set
        {
            if (value)
            {
                // 首选计划任务；创建失败（例如当前会话没有管理员权限）时回退到启动文件夹快捷方式
                if (CreateScheduledTask())
                {
                    DeleteStartupShortcut();
                    return;
                }

                using var shortcut = new WindowsShortcut();
                shortcut.Path = AppBase.ExecutingEntrance;
                shortcut.Arguments = "--autostartup";
                shortcut.WorkingDirectory = Path.GetDirectoryName(AppBase.ExecutingEntrance);
                shortcut.Save(StartupShortcutPath);
            }
            else
            {
                DeleteScheduledTask();
                DeleteStartupShortcut();
            }
        }
    }
    public bool IsUrlSchemeRegistered{
        get => UriProtocolRegisterHelper.IsRegistered();
        set
        {
            if (value)
            {
                UriProtocolRegisterHelper.Register();
            }
            else
            {
                UriProtocolRegisterHelper.UnRegister();
            }
        }
    }

    private static bool ScheduledTaskExists() => RunSchtasks($"/Query /TN {AutoStartTaskName}") == 0;

    private static bool CreateScheduledTask()
    {
        // /TR 的整个值需要一对引号，其中的可执行文件路径再单独加转义引号，否则带空格的路径会被拆开
        var arguments =
            $"/Create /TN {AutoStartTaskName} /TR \"\\\"{AppBase.ExecutingEntrance}\\\" --autostartup\" " +
            "/SC ONLOGON /RL HIGHEST /F";
        return RunSchtasks(arguments) == 0;
    }

    private static void DeleteScheduledTask()
    {
        if (!ScheduledTaskExists())
        {
            return;
        }

        RunSchtasks($"/Delete /TN {AutoStartTaskName} /F");
    }

    private static void DeleteStartupShortcut()
    {
        if (File.Exists(StartupShortcutPath))
        {
            File.Delete(StartupShortcutPath);
        }
    }

    private static int RunSchtasks(string arguments)
    {
        try
        {
            using var process = Process.Start(new ProcessStartInfo("schtasks.exe", arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            });
            if (process is null)
            {
                return -1;
            }

            process.WaitForExit(15000);
            return process.HasExited ? process.ExitCode : -1;
        }
        catch
        {
            return -1;
        }
    }
}
