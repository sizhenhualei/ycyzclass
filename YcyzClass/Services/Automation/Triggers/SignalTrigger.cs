using YcyzClass.Core.Abstractions.Automation;
using YcyzClass.Core.Attributes;
using YcyzClass.Models.Automation.Triggers;
using YcyzClass.Models.EventArgs;

namespace YcyzClass.Services.Automation.Triggers;

[TriggerInfo("ycyzclass.signal", "收到信号时", "\ue40e")]
public class SignalTrigger(SignalTriggerHandlerService signalTriggerHandlerService) : TriggerBase<SignalTriggerSettings>
{
    public SignalTriggerHandlerService SignalTriggerHandlerService { get; } = signalTriggerHandlerService;

    public override void Loaded()
    {
        SignalTriggerHandlerService.Handled += SignalTriggerHandlerServiceOnHandled;
    }


    public override void UnLoaded()
    {
        SignalTriggerHandlerService.Handled -= SignalTriggerHandlerServiceOnHandled;
    }

    private void SignalTriggerHandlerServiceOnHandled(object? sender, SignalTriggerEventArgs e)
    {
        if (e.SignalName != Settings.SignalName) return;

        if (e.Revert)
        {
            TriggerRevert();
        }
        else
        {
            Trigger();
        }
    }
}