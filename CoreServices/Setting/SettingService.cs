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

/// <summary>
/// 设置服务
/// </summary>
public sealed class SettingService : DisposableTemplate
{
    private readonly Dictionary<string, SettingValue> _settings = [];

    public IEnumerable<SettingValue> Settings => _settings.Values;

    /// <summary>
    /// 设置服务组装器
    /// <para>
    /// 用于配置初始默认设置
    /// </para>
    /// </summary>
    public sealed class SettingServiceBuilder
    {
        private readonly SettingService _service;

        internal SettingServiceBuilder(SettingService service)
        {
            _service = service;
        }

        /// <summary>
        /// 添加初始默认设置
        /// </summary>
        /// <param name="key">设置键</param>
        /// <param name="settingValue">设置值</param>
        /// <returns></returns>
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

    /// <summary>
    /// 初始化设置服务
    /// </summary>
    /// <param name="builder">设置服务组装器</param>
    public void Init(Action<SettingServiceBuilder> builder)
    {
        builder(new(this));
    }

    /// <summary>
    /// 尝试获取设置
    /// </summary>
    /// <param name="key">设置键</param>
    /// <param name="settingValue">设置值</param>
    /// <returns>若有匹配的设置键则返回 <see href="true"/> ，反之返回 <see href="false"/></returns>
    public bool TryGetSetting(
        string key,
        [NotNullWhen(true)] out SettingValue? settingValue) => _settings.TryGetValue(key, out settingValue);

    /// <summary>
    /// 保存设置到指定文件
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
    public bool SaveSettings(string path)
    {
        if (!Path.Exists(path))
            return false;

        SettingData[] settingDatas = [..from ss in _settings
                                        select new SettingData(){ Key = ss.Key,Value = ss.Value.Value}];

        File.WriteAllBytes(path, MessagePackSerializer.Serialize(settingDatas));
        return false;
    }
    /// <summary>
    /// 从指定文件读入设置
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
    public bool ReadSettings(string path)
    {
        if (!Path.Exists(path))
            return false;
        foreach (var settingData in MessagePackSerializer.Deserialize<SettingData[]>(File.ReadAllBytes(path)))
        {
            if (_settings.TryGetValue(settingData.Key, out var settingValue))
            {
                settingValue.InitValue(settingData.Value);
            }
        }

        return true;
    }
    protected override void DestoryManagedResource()
    {
        _settings.Clear();
    }

    protected override void DestoryUnmanagedResource()
    {

    }
}

