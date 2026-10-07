using System;
using YcyzClass.Core.Abstractions.Automation;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Attributes;

namespace YcyzClass.Services.Automation.Triggers;

[TriggerInfo("ycyzclass.lessons.onAfterSchool", "放学时", "\ued35")]
public class OnAfterSchoolTrigger(ILessonsService lessonsService) : TriggerBase
{
    private ILessonsService LessonsService { get; } = lessonsService;

    public override void Loaded()
    {
        LessonsService.OnAfterSchool += OnLessonsServiceOnAfterSchool;
    }
    public override void UnLoaded()
    {
        LessonsService.OnAfterSchool -= OnLessonsServiceOnAfterSchool;
    }

    private void OnLessonsServiceOnAfterSchool(object? sender, EventArgs e)
    {
        Trigger();
    }
}