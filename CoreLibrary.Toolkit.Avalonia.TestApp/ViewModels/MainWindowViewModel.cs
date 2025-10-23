using System.Collections.Generic;
using Lucide.Avalonia;
using Zeng.CoreLibrary.Toolkit.Avalonia.Structs;

namespace Zeng.CoreLibrary.Toolkit.Avalonia.TestApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    public List<MenuItemData> NavigationItems { get; } = [new("主页", new LucideIcon() { Kind = LucideIconKind.House })];
}