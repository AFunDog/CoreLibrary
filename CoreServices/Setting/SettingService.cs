using CoreServices.Setting.Structs;
using CoreServices.Template;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.Setting;

public sealed class SettingService : DisposableTemplate
{
    private readonly Dictionary<string, SettingValue> _settings = [];

    public IEnumerable<SettingValue> Settings => _settings.Values;

    public sealed class SettingServiceBuilder(SettingService service)
    {
        private readonly SettingService _service = service;

        public bool AddSetting(string key, SettingValue settingValue)
        {
            if (_service._settings.TryGetValue(key, out var value))
            {
                return false;
            }
            else
            {
                _service._settings.Add(key, settingValue);
                return true;
            }
        }
    }

    public void Init(Action<SettingServiceBuilder> builder)
    {
        builder(new(this));
    }

    /// <summary>
    /// 尝试获取设置
    /// </summary>
    /// <param name="key"></param>
    /// <param name="settingValue"></param>
    /// <returns></returns>
    public bool TryGetSetting(
        string key,
        [NotNullWhen(true)] out SettingValue? settingValue) => _settings.TryGetValue(key, out settingValue);

    public bool SaveSettings(string path)
    {
        if (!Path.Exists(path))
            return false;

        SettingData[] settingDatas = [..from ss in _settings
                                        select new SettingData(){ Key = ss.Key,Value = ss.Value.Value}];

        File.WriteAllBytes(path, MessagePackSerializer.Serialize(settingDatas));
        return false;
    }

    protected override void DestoryManagedResource()
    {
        _settings.Clear();
    }

    protected override void DestoryUnmanagedResource()
    {

    }
}

