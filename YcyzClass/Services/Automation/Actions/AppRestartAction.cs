using System.Threading.Tasks;
using YcyzClass.Core;
using YcyzClass.Core.Abstractions.Automation;
using YcyzClass.Core.Attributes;
using YcyzClass.Models.Actions;
namespace YcyzClass.Services.Automation.Actions;

[ActionInfo("ycyzclass.app.restart", "重启 YcyzClass", "\ue0bd", addDefaultToMenu:false)]
public class AppRestartAction : ActionBase<AppRestartActionSettings>
{
    protected override async Task OnInvoke()
    {
        AppBase.Current.Restart(Settings.Value);
    }
}