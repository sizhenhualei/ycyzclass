using System.Collections.ObjectModel;
using YcyzClass.Core.Models.Components;

namespace YcyzClass.Core.Abstractions.Models;

/// <summary>
/// 代表一个容器组件设置。
/// </summary>
public interface IComponentContainerSettings
{
    /// <summary>
    /// 组件容器包含的组件
    /// </summary>
    public ObservableCollection<ComponentSettings> Children { get; set; }
}