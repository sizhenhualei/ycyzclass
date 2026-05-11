using HarmonyLib;

namespace YcyzClass.Platform.Windows;

public static class PatcherEntrance
{
    public static void InstallPatchers()
    {
        var harmony = new Harmony("cn.ycyzclass.app.patchers");
        harmony.PatchAll();
    }
}