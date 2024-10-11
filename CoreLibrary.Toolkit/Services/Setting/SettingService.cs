using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Core.BasicObjects;
using CoreLibrary.Toolkit.Services.Setting.Structs;
using MessagePack;

namespace CoreLibrary.Toolkit.Services.Setting;

public sealed class SettingsBuilder
{
    internal SettingService SettingService { get; }
    internal Dictionary<string, SettingConfig> Settings { get; } = [];
    internal Dictionary<string, SettingGroupBuilder> Groups { get; } = [];

    internal SettingsBuilder(SettingService settingService)
    {
        SettingService = settingService;
    }

    public sealed class SettingGroupBuilder
    {
        internal string GroupKey { get; }
        internal Dictionary<string, SettingConfig> Settings { get; } = [];
        internal Dictionary<string, SettingPartBuilder> Parts { get; } = [];

        internal SettingGroupBuilder(string groupKey)
        {
            GroupKey = groupKey;
        }

        public sealed class SettingPartBuilder
        {
            internal string GroupKey { get; }
            internal string PartKey { get; }
            internal Dictionary<string, SettingConfig> Settings { get; } = [];

            internal SettingPartBuilder(string groupKey, string partKey)
            {
                GroupKey = groupKey;
                PartKey = partKey;
            }

            public SettingPartBuilder ConfigureSetting(
                string key,
                SettingValue settingValue,
                AttachedArgs? attachedArgs = null
            )
            {
                Settings[key] = new(GetSettingKey(key), settingValue, attachedArgs);
                return this;
            }

            internal string GetSettingKey(string settingKey) =>
                string.Format("{0}.{1}.{2}", GroupKey, PartKey, settingKey);
        }

        public SettingPartBuilder ConfigurePart(string partKey)
        {
            Settings.Remove(partKey);
            Parts.Remove(partKey);
            var newPart = new SettingPartBuilder(GroupKey, partKey);
            Parts.Add(partKey, newPart);
            return newPart;
        }

        public SettingGroupBuilder ConfigureSetting(
            string key,
            SettingValue settingValue,
            AttachedArgs? attachedArgs = null
        )
        {
            Settings.Remove(key);
            Parts.Remove(key);
            Settings[key] = new(GetSettingKey(key), settingValue, attachedArgs);
            return this;
        }

        internal string GetSettingKey(string settingKey) => string.Format("{0}.{1}", GroupKey, settingKey);
    }

    public SettingGroupBuilder ConfigureGroup(string groupKey)
    {
        Settings.Remove(groupKey);
        Groups.Remove(groupKey);
        var newGroup = new SettingGroupBuilder(groupKey);
        Groups.Add(groupKey, newGroup);
        return newGroup;
    }

    public SettingsBuilder ConfigureSetting(string key, SettingValue settingValue, AttachedArgs? attachedArgs = null)
    {
        Settings.Remove(key);
        Groups.Remove(key);
        Settings[key] = new(GetSettingKey(key), settingValue, attachedArgs);
        return this;
    }

    internal string GetSettingKey(string settingKey) => settingKey;

    internal SettingCollectionNode BuildSettings() =>
        new(
            "Root",
            Settings
                .Select(kv => new KeyValuePair<string, SettingNode>(kv.Key, new SettingConfigNode(kv.Value)))
                .Concat(
                    Groups.Select(kv => new KeyValuePair<string, SettingNode>(
                        kv.Key,
                        new SettingCollectionNode(
                            kv.Key,
                            kv.Value.Settings.Select(kv => new KeyValuePair<string, SettingNode>(
                                    kv.Key,
                                    new SettingConfigNode(kv.Value)
                                ))
                                .Concat(
                                    kv.Value.Parts.Select(kv => new KeyValuePair<string, SettingNode>(
                                        kv.Key,
                                        new SettingCollectionNode(
                                            kv.Key,
                                            kv.Value.Settings.Select(kv => new KeyValuePair<string, SettingNode>(
                                                    kv.Key,
                                                    new SettingConfigNode(kv.Value)
                                                ))
                                                .ToFrozenDictionary()
                                        )
                                    ))
                                )
                                .ToFrozenDictionary()
                        )
                    ))
                )
                .ToFrozenDictionary()
        );
}

internal sealed class SettingService : DisposableObject, ISettingService
{
    private SettingCollectionNode? _settings;

    public SettingCollectionNode Settings =>
        _settings ?? throw new InvalidOperationException($"设置服务未初始化，请先调用 {nameof(BuildSettings)}");

    public void BuildSettings(Action<SettingsBuilder> builder)
    {
        var settingBuilder = new SettingsBuilder(this);
        builder(settingBuilder);
        _settings = settingBuilder.BuildSettings();
    }

    public void SetSettingValue(string key, object value)
    {
        if (GetSettingConfig(key) is SettingConfig config)
        {
            config.SettingValue.Value = value;
            config.SettingValue.ApplyChange();
        }
    }

    public object? GetSettingValue(string key)
    {
        return GetSettingConfig(key)?.SettingValue.Value;
    }

    public T? GetSettingValue<T>(string key)
    {
        if (GetSettingValue(key) is T value)
        {
            return value;
        }
        return default;
    }

    public void SaveSettings(string filePath)
    {
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        if (!File.Exists(filePath))
            File.Create(filePath).Close();

        var settingDatas = CollectToRecord();

        File.WriteAllBytes(filePath, MessagePackSerializer.Serialize(settingDatas));
    }

    public async Task SaveSettingsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        using FileStream fileStream = new(filePath, FileMode.OpenOrCreate);
        var settingDatas = CollectToRecord();
        await MessagePackSerializer.SerializeAsync(fileStream, settingDatas, cancellationToken: cancellationToken);
    }

    public void ReadSettings(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        try
        {
            foreach (
                var settingData in MessagePackSerializer.Deserialize<IEnumerable<SettingRecord>>(
                    File.ReadAllBytes(filePath)
                )
            )
            {
                if (GetSettingConfig(settingData.Key) is SettingConfig config)
                {
                    config.SettingValue.InitValue(settingData.Value);
                }
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }

    public async Task ReadSettingsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            return;

        await Task.Run(
            async () =>
            {
                try
                {
                    using FileStream fs = File.OpenRead(filePath);
                    foreach (
                        var settingData in await MessagePackSerializer.DeserializeAsync<IEnumerable<SettingRecord>>(
                            fs,
                            cancellationToken: cancellationToken
                        )
                    )
                    {
                        if (GetSettingConfig(settingData.Key) is SettingConfig config)
                        {
                            config.SettingValue.InitValue(settingData.Value);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            },
            cancellationToken
        );
    }

    public void TryExecuteAllCommands()
    {
        Settings.ForEach(node =>
        {
            if (node is SettingConfigNode configNode)
            {
                configNode.Config.SettingValue.ApplyChange();
            }
        });
    }

    private SettingConfig? GetSettingConfig(string key)
    {
        string[] path = key.Split('.');
        var root = Settings;
        for (int i = 0; i < path.Length - 1; i++)
        {
            if (root.Values.TryGetValue(path[i], out var node) && node is SettingCollectionNode child)
            {
                root = child;
            }
            else
            {
                return null;
            }
        }
        if (root.Values.TryGetValue(path[^1], out var value) && value is SettingConfigNode configNode)
        {
            return configNode.Config;
        }

        return null;
    }

    private IEnumerable<SettingRecord> CollectToRecord()
    {
        List<SettingRecord> records = [];

        Settings.ForEach(node =>
        {
            if (node is SettingConfigNode configNode)
            {
                records.Add(new SettingRecord(configNode.Config.Key, configNode.Config.SettingValue.Value));
            }
        });

        return records;
    }

    protected override void DisposeManagedResource()
    {
        _settings = null;
    }

    protected override void DisposeUnmanagedResource() { }

    protected override void OnDisposed() { }
}
