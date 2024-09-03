using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CoreLibrary.Toolkit.WinUI.Contracts;
using CoreLibrary.Toolkit.WinUI.Services;
using CoreServices.Localization;
using CoreServices.Setting;
using CoreServices.Setting.Structs;
using CoreServicesWinUILibrary.Structs;
using CoreServicesWinUILibrary.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CoreServicesWinUILibrary
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static App Instance => (Current as App) ?? new();

        public static T GetService<T>()
            where T : class
        {
            return (Instance.ServiceProvider.GetService(typeof(T)) as T)
                ?? throw new ArgumentException($"未找到服务 {typeof(T)}");
        }

        internal static Lazy<ObservableCollection<GroupInfo>> Settings { get; } =
            new(
                () =>
                    new(
                        GetService<ISettingService>()
                            .GroupInfos.Where(groupKey => groupKey.Value.Count > 0)
                            .Select(groupKey => new GroupInfo(
                                groupKey.Key,
                                new(
                                    groupKey.Value.Select(nodeKey => new SettingNodeInfo(
                                        GetService<ISettingService>().Settings[nodeKey.Key],
                                        [
                                            .. nodeKey.Value.Select(key => new SettingInfo(
                                                GetService<ISettingService>().Settings[key]
                                            ))
                                        ]
                                    ))
                                )
                            ))
                    )
            );

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            var builder = new ServiceCollection()
                .AddSingleton<ISettingService, SettingService>()
                .AddSingleton<ILocalizeService, LocalizeService>()
                .AddSingleton<INavigateService, NavigateService>()
                .AddTransient<SettingViewModel>();

            ServiceProvider = builder.BuildServiceProvider();
        }

        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            InitSettingService();

            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;

        private void InitSettingService()
        {
            GetService<ISettingService>()
                .BuildSettings(builder =>
                {
                    builder
                        .ConfigureGroup("主题")
                        .ConfigureSetting(
                            new(
                                "主题色",
                                new SettingValueEnum(
                                    0,
                                    [new("默认", 0), new("深色", 1), new("浅色", 2)],
                                    new SettingValueCommand((s, e) => { })
                                ),
                                new SettingAttachedArgs("\uE790")
                            ),
                            "主题"
                        )
                        .ConfigureGroup("关于")
                        .ConfigureSetting(
                            new(
                                "应用信息",
                                new SettingValue("WinUILibrary", new SettingValueCommand((s, e) => { })),
                                new SettingAttachedArgs("\uE71D")
                            ),
                            "关于"
                        )
                        .ConfigureSetting(
                            new("版本号", new SettingValue("1.0.0", new SettingValueCommand((s, e) => { }))),
                            "关于",
                            "应用信息"
                        );
                });

            if (!Settings.IsValueCreated)
            {
                _ = Settings.Value;
            }
        }
    }
}
