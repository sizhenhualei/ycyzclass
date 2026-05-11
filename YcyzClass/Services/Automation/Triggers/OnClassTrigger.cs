using System;
using YcyzClass.Core.Abstractions.Automation;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Attributes;

namespace YcyzClass.Services.Automation.Triggers;

[TriggerInfo("ycyzclass.lessons.onClass", "上课时", "\uE47A")]
public class OnClassTrigger(ILessonsService lessonsService) : TriggerBase
{
    private ILessonsService LessonsService { get; } = lessonsService;

    public override void Loaded()
    {
        LessonsService.OnClass += LessonsServiceOnOnClass;
    }
    public override void UnLoaded()
    {
        LessonsService.OnClass -= LessonsServiceOnOnClass;
    }

    private void LessonsServiceOnOnClass(object? sender, EventArgs e)
    {
        Trigger();
    }
}