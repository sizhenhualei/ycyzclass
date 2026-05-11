using System.Collections.ObjectModel;
using YcyzClass.Core.Abstractions.Services;
using YcyzClass.Core.Models.Tutorial;
using YcyzClass.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;

namespace YcyzClass.ViewModels;

public partial class TutorialCenterViewModel(SettingsService settingsService, ITutorialService tutorialService) : ObservableObject
{
   public SettingsService SettingsService { get; } = settingsService;
   public ITutorialService TutorialService { get; } = tutorialService;

   [ObservableProperty] private TutorialGroup? _selectedTutorialGroup;
   [ObservableProperty] private Tutorial? _selectedTutorial;
   
   public ObservableCollection<object> NavigationViewItems { get; } = [];
}