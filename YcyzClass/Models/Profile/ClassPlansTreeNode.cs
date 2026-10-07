using System;
using System.Collections.ObjectModel;
using YcyzClass.Shared.Models.Profile;

namespace YcyzClass.Models.Profile;

public class ClassPlansTreeNode
{
    public Guid Guid { get; set; }
    public bool IsGroup { get; set; }
    
    public ReadOnlyObservableCollection<ClassPlansTreeNode>? SubPlans { get; set; }
    public ClassPlan? ClassPlan { get; set; }
}