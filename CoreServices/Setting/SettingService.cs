using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreServices.Setting.Structs;
using CoreServices.Template;
using MessagePack;

namespace CoreServices.Setting;

/// <summary>
/// 设置服务
/// </summary>
public sealed class SettingService : DisposableTemplate, ISettingService
{
    private readonly Dictionary<string, SettingValue> _settings = [];

    public IDictionary<string, SettingValue> Settings => _settings.AsReadOnly();

    /// <summary>
    /// 设置服务组装器
    /// <para>
    /// 用于配置初始默认设置
    /// </para>
    /// </summary>
    public sealed class SettingServiceBuilder
        : DisposableTemplate,
            ISettingService.ISettingServiceBuilder
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
        public ISettingService.ISettingServiceBuilder Configure(
            string key,
            SettingValue settingValue
        )
        {
            _service._settings.TryAdd(key, settingValue);
            return this;
        }

        protected override void DestoryManagedResource() { }

        protected override void DestoryUnmanagedResource() { }
    }

    /// <summary>
    /// 初始化设置服务
    /// </summary>
    /// <param name="builder">设置服务组装器</param>
    public void BuildSettings(Action<ISettingService.ISettingServiceBuilder> builder)
    {
        builder(new SettingServiceBuilder(this));
    }

    /// <summary>
    /// 获取设置
    /// </summary>
    /// <param name="key">设置键</param>
    /// <param name="settingValue">设置值</param>
    /// <returns>若有匹配的设置键则返回 <see href="true"/> ，反之返回 <see href="false"/></returns>
    public SettingValue? GetSetting(string key) =>
        _settings.TryGetValue(key, out var settingValue) ? settingValue : null;
    public object GetSettingValue(string key,object defaultValue) =>
        _settings.TryGetValue(key, out var settingValue) ? settingValue.Value : defaultValue;
    public void SetSettingValue(string key ,object value)
    {
        if(_settings.TryGetValue(key, out var settingValue) && settingValue.Value.GetType() == value.GetType())
        {
            settingValue.Value = value;
        }
    }

    /// <summary>
    /// 保存设置到指定文件
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
    public void SaveSettings(string path)
    {
        if (!File.Exists(path))
            File.Create(path).Close();

        SettingData[] settingDatas =
        [
            .. from ss in _settings
            select new SettingData() { Key = ss.Key, Value = ss.Value.Value }
        ];

        File.WriteAllBytes(path, MessagePackSerializer.Serialize(settingDatas));
    }

    /// <summary>
    /// 从指定文件读入设置
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns></returns>
    public void ReadSettings(string path)
    {
        if (!File.Exists(path))
            return;
        foreach (
            var settingData in MessagePackSerializer.Deserialize<SettingData[]>(
                File.ReadAllBytes(path)
            )
        )
        {
            if (_settings.TryGetValue(settingData.Key, out var settingValue))
            {
                settingValue.InitValue(settingData.Value);
            }
        }
    }
    public void TryExecuteAllCommands()
    {
        foreach((_,var setting) in _settings)
        {
            setting.Value = setting.Value;
        }
    }
    protected override void DestoryManagedResource()
    {
        _settings.Clear();
    }

    protected override void DestoryUnmanagedResource() { }
}
