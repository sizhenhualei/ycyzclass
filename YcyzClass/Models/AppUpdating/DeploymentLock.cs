using System.Collections.Generic;

namespace YcyzClass.Models.AppUpdating;

public class DeploymentLock
{
    public Dictionary<string, List<string>> ExistedFiles { get; set; } = [];

    public string SubChannel { get; set; } = "";

    public byte[] FileMapSha512 { get; set; } = [];
}