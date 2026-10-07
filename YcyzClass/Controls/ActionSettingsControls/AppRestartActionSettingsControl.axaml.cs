using YcyzClass.Core.Abstractions.Controls;
using YcyzClass.Models.Actions;
namespace YcyzClass.Controls.ActionSettingsControls;

public partial class AppRestartActionSettingsControl : ActionSettingsControlBase<AppRestartActionSettings>
{
    public AppRestartActionSettingsControl()
    {
        InitializeComponent();
        DataContext = this;
    }
}
