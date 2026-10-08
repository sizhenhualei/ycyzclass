using YcyzClass.Enums;

namespace YcyzClass.Models;

public class ApplicationCommand
{
    public bool WaitMutex
    {
        get;
        set;
    } = false;

    public bool Quiet { get; set; } = false;

    public bool PrevSessionMemoryKilled { get; set; } = false;

    public bool DisableManagement { get; set; } = false;

    public string Uri { get; set; } = "";

    public string ExternalPluginPath { get; set; } = "";

    public bool Verbose { get; set; } = false;

    public bool ShowOssWatermark { get; set; } = false;

    public bool Recovery { get; set; } = false;

    public bool Diagnostic { get; set; } = false;
    public bool Safe { get; set; } = false;

    public string ImportV1 { get; set; } = "";
    public string ImportV2 { get; set; } = "";

    public bool SkipOobe { get; set; } = false;

    public string ImportEntries { get; set; } = "0";

    public bool ImportComplete { get; set; } = false;

    public bool Refreshing { get; set; } = false;

    public bool Onboarding { get; set; } = false;

    public bool Autostartup { get; set; } = false;
}