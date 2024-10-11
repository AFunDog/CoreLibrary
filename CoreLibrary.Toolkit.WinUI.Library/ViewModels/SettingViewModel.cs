using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CoreLibrary.Toolkit.Services.Setting;
using Windows.Media.Core;

namespace CoreServicesWinUILibrary.ViewModels
{
    internal sealed record GroupInfo(string GroupKey, List<SettingNodeInfo> Settings);

    internal sealed record SettingNodeInfo(SettingConfiguration Configuration, List<SettingInfo> Kids)
        : SettingInfo(Configuration);

    internal record SettingInfo(SettingConfiguration Configuration);

    internal sealed partial class SettingViewModel : ObservableRecipient
    {
        private readonly ISettingService _settingService;

        [ObservableProperty]
        private ObservableCollection<string> _groupInfos;

        [ObservableProperty]
        private ObservableCollection<GroupInfo> _settings = App.Settings.Value;

        public SettingViewModel(ISettingService settingService)
        {
            _settingService = settingService;
            _groupInfos = new(_settingService.GroupInfos.Keys);
        }
    }
}
