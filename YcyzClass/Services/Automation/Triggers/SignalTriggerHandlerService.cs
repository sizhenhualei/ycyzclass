using System;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Models.Automation.Triggers;
using YcyzClass.Models.EventArgs;

namespace YcyzClass.Services.Automation.Triggers;

public class SignalTriggerHandlerService
{
    public event EventHandler<SignalTriggerEventArgs>? Handled;

    public void EmitSignal(string name, bool revert)
    {
        Handled?.Invoke(this, new SignalTriggerEventArgs(name, revert));
    }

    public SignalTriggerHandlerService(IActionService actionService)
    {
        actionService.RegisterActionHandler("ycyzclass.broadcastSignal", (o, guid) =>
        {
            if (o is SignalTriggerSettings settings)
            {
                EmitSignal(settings.SignalName, settings.IsRevert);
            }
        });
        actionService.RegisterRevertHandler("ycyzclass.broadcastSignal", (o, guid) =>
        {
            if (o is SignalTriggerSettings settings)
            {
                EmitSignal(settings.SignalName, !settings.IsRevert);
            }
        });
    }
}