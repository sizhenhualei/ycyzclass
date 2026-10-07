using System.Threading.Tasks;
using YcyzClass.Core;
using YcyzClass.Core.Abstractions.Automation;
using YcyzClass.Core.Attributes;
namespace YcyzClass.Services.Automation.Actions;

[ActionInfo("ycyzclass.app.quit", "退出 YcyzClass", "\ue0df", addDefaultToMenu:false)]
public class AppQuitAction : ActionBase
{
    protected override async Task OnInvoke()
    {
        AppBase.Current.Stop();
    }
}