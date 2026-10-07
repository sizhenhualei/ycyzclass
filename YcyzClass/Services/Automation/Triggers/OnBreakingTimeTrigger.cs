using System;
using YcyzClass.Core.Abstractions.Automation;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Attributes;

namespace YcyzClass.Services.Automation.Triggers;

[TriggerInfo("ycyzclass.lessons.onBreakingTime", "课间休息时", "\ue4c4")]
public class OnBreakingTimeTrigger(ILessonsService lessonsService) : TriggerBase
{
    private ILessonsService LessonsService { get; } = lessonsService;

    public override void Loaded()
    {
        LessonsService.OnBreakingTime += LessonsServiceOnOnBreakingTime;
    }
    public override void UnLoaded()
    {
        LessonsService.OnBreakingTime -= LessonsServiceOnOnBreakingTime;
    }

    private void LessonsServiceOnOnBreakingTime(object? sender, EventArgs e)
    {
        Trigger();
    }
}