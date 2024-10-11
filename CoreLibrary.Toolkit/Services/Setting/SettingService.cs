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
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Core.BasicObjects;
using CoreLibrary.Toolkit.Services.Setting.Structs;
using MessagePack;

namespace CoreLibrary.Toolkit.Services.Setting;

public sealed class SettingsBuilder
{
    // 配置项目 Key 不能包含这个字符
    const char PathSeparator = '.';

    private readonly string _key;
    private readonly List<SettingNode> _nodes = [];

    public SettingsBuilder(string key = "")
    {
        _key = key;
    }

    public SettingsBuilder ConfigSetting(SettingConfig settingConfig)
    {
        if (settingConfig.Key.Contains(PathSeparator))
            return this;

        _nodes.Add(new SettingConfigNode(settingConfig));
        return this;
    }

    public SettingsBuilder ConfigSettings(string key, Action<SettingsBuilder> childrenBuilder)
    {
        if (key.Contains(PathSeparator))
            return this;

        var children = new SettingsBuilder(key);
        childrenBuilder(children);
        _nodes.Add(children.BuildSettings());
        return this;
    }

    public SettingCollectionNode BuildSettings()
    {
        return new SettingCollectionNode(_key, _nodes.ToDictionary(node => node.Key));
    }
}

internal sealed class SettingService : DisposableObject, ISettingService
{
    private SettingCollectionNode? _settings;

    public SettingCollectionNode Settings =>
        _settings ?? throw new InvalidOperationException($"设置服务未初始化，请先调用 {nameof(BuildSettings)}");

    public void BuildSettings(Action<SettingsBuilder> builder)
    {
        var settingBuilder = new SettingsBuilder();
        builder(settingBuilder);
        _settings = settingBuilder.BuildSettings();
    }

    public void SetSettingValue<T>(string key, T value)
        where T : notnull
    {
        if (GetSettingConfig(key) is SettingConfig config)
        {
            config.SettingValue.Value = value;
            config.SettingValue.ApplyChange();
        }
    }

    public SettingValue? GetSettingValue(string key)
    {
        return GetSettingConfig(key)?.SettingValue;
    }

    public T? GetSettingValue<T>(string key)
    {
        if (GetSettingValue(key) is SettingValue settingValue && settingValue.Value is T value)
        {
            return value;
        }
        return default;
    }

    public void SaveSettings(string filePath)
    {
        var dirPath = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
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

        SettingCollectionNodeToRecords("", Settings);

        return records;

        void SettingCollectionNodeToRecords(string path, SettingCollectionNode collectionNode)
        {
            foreach ((var key, var settingNode) in collectionNode.Values)
            {
                var curPath = string.IsNullOrEmpty(path) ? key : string.Format("{0}.{1}", path, key);
                switch (settingNode)
                {
                    case SettingConfigNode configNode:
                        records.Add(new(curPath, configNode.Config.SettingValue.Value));
                        break;
                    case SettingCollectionNode children:
                        SettingCollectionNodeToRecords(curPath, children);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    protected override void DisposeManagedResource()
    {
        _settings = null;
    }

    protected override void DisposeUnmanagedResource() { }

    protected override void OnDisposed() { }
}
