using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using CoreServices.Setting.Structs;
using CoreServices.Template;
using MessagePack;

namespace CoreServices.Setting;

public sealed class SettingService : DisposableTemplate, ISettingService
{
    private const string DefaultGroup = "Default";

    private readonly Dictionary<string, SettingConfiguration> _settings = [];
    private readonly Dictionary<string, Dictionary<string, List<string>>> _groupInfos = new() { [DefaultGroup] = [] };

    public IReadOnlyDictionary<string, SettingConfiguration> Settings => _settings.AsReadOnly();

    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyCollection<string>>> GroupInfos =>
        _groupInfos
            .Select(kv => new KeyValuePair<string, IReadOnlyDictionary<string, IReadOnlyCollection<string>>>(
                kv.Key,
                kv.Value.Select(kv2 => new KeyValuePair<string, IReadOnlyCollection<string>>(
                        kv2.Key,
                        kv2.Value.AsReadOnly()
                    ))
                    .ToDictionary()
            ))
            .ToDictionary();

    public sealed class SettingServiceBuilder : DisposableTemplate, ISettingService.ISettingServiceBuilder
    {
        private readonly SettingService _service;

        internal SettingServiceBuilder(SettingService service)
        {
            _service = service;
        }

        public ISettingService.ISettingServiceBuilder ConfigureGroup(string groupKey)
        {
            _service._groupInfos.TryAdd(groupKey, []);
            return this;
        }

        public ISettingService.ISettingServiceBuilder ConfigureSetting(
            SettingConfiguration settingConfiguration,
            string? groupKey = null,
            string? ownerKey = null
        )
        {
            _service._settings.TryAdd(settingConfiguration.OnlyKey, settingConfiguration);
            if (string.IsNullOrEmpty(groupKey) || !_service._groupInfos.ContainsKey(groupKey))
            {
                groupKey = DefaultGroup;
            }
            if (_service._groupInfos.TryGetValue(groupKey, out var members))
            {
                if (!string.IsNullOrEmpty(ownerKey) && members.TryGetValue(ownerKey, out var kids))
                {
                    kids.Add(settingConfiguration.OnlyKey);
                }
                else
                {
                    members.TryAdd(settingConfiguration.OnlyKey, []);
                }
            }
            return this;
        }

        protected override void DestoryManagedResource() { }

        protected override void DestoryUnmanagedResource() { }
    }

    public void BuildSettings(Action<ISettingService.ISettingServiceBuilder> builder)
    {
        builder(new SettingServiceBuilder(this));
    }

    public void SetSettingValue(string key, object value)
    {
        if (_settings.TryGetValue(key, out var settingConfiguration))
        {
            settingConfiguration.SettingValue.Value = value;
            settingConfiguration.SettingValue.ApplyChange();
        }
    }

    public object? GetSettingValue(string key)
    {
        if (_settings.TryGetValue(key, out var settingConfiguration))
        {
            return settingConfiguration.SettingValue.Value;
        }
        return null;
    }

    public void SaveSettings(string filePath)
    {
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        if (!File.Exists(filePath))
            File.Create(filePath).Close();

        var settingDatas = _settings.Select(kv =>
        {
            return new SettingRecord(kv.Key, kv.Value.SettingValue.InternalValue);
        });

        File.WriteAllBytes(filePath, MessagePackSerializer.Serialize(settingDatas));
    }

    public async Task SaveSettingsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        using FileStream fileStream = new(filePath, FileMode.OpenOrCreate);
        var settingDatas = _settings.Select(kv =>
        {
            return new SettingRecord(kv.Key, kv.Value.SettingValue.InternalValue);
        });
        await MessagePackSerializer.SerializeAsync(fileStream, settingDatas, cancellationToken: cancellationToken);
    }

    public void ReadSettings(string dirPath)
    {
        if (!Directory.Exists(dirPath))
            return;
        foreach (var file in Directory.GetFiles(dirPath))
        {
            try
            {
                foreach (
                    var settingData in MessagePackSerializer.Deserialize<IEnumerable<SettingRecord>>(
                        File.ReadAllBytes(file)
                    )
                )
                {
                    if (_settings.TryGetValue(settingData.Key, out var settingConfiguration))
                    {
                        settingConfiguration.SettingValue.InitValue(settingData.Value);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }

    public async Task ReadSettingsAsync(string dirPath, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(dirPath))
            return;
        foreach (var file in Directory.GetFiles(dirPath))
        {
            try
            {
                using FileStream fileStream = new(file, FileMode.Open);
                foreach (
                    var settingData in await MessagePackSerializer.DeserializeAsync<IEnumerable<SettingRecord>>(
                        fileStream,
                        cancellationToken: cancellationToken
                    )
                )
                {
                    if (_settings.TryGetValue(settingData.Key, out var settingConfiguration))
                    {
                        settingConfiguration.SettingValue.InitValue(settingData.Value);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }

    public void TryExecuteAllCommands()
    {
        foreach ((_, var settingConfiguration) in _settings)
        {
            settingConfiguration.SettingValue.ApplyChange();
        }
    }

    protected override void DestoryManagedResource()
    {
        _settings.Clear();
        _groupInfos.Clear();
    }

    protected override void DestoryUnmanagedResource() { }
}
