using System;
using System.Diagnostics;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Zeng.CoreLibrary.Toolkit.Windows.Services.SystemMonitor;

namespace Zeng.CoreLibrary.Toolkit.Avalonia.TestApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var systemMonitor = Program.ServiceProvider.GetRequiredService<IPerformanceMonitor>();
    }
}