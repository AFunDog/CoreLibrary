using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using CoreLibrary.Core.Contacts;
using CoreLibrary.Toolkit.Services.Setting.Structs;
using MessagePack;

namespace CoreLibrary.Toolkit.Services.Setting;

internal sealed class SettingService : DisposableObject, ISettingService
{
    private Dictionary<string, object?> Properties { get; } = [];

    public void RegisterModel(Type modelType)
    {
        var properties = modelType
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(field => field.FieldType.IsSubclassOf(typeof(SettingProperty)))
            .Select(filed => (SettingProperty)filed.GetValue(null)!);
        foreach (var property in properties)
        {
            if (Properties.ContainsKey(property.Token) is false)
                Properties.Add(property.Token, property.DefValue);
        }
    }

    public T GetValue<T>(SettingProperty<T> property)
    {
        if (Properties.TryGetValue(property.Token, out var value))
        {
            return (T)value!;
        }
        return (T)property.DefValue!;
    }

    public void SetValue<T>(SettingProperty<T> property, T value)
    {
        Properties[property.Token] = value;
    }

    public void SaveData(string filePath)
    {
        using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
        MessagePackSerializer.Serialize(
            fs,
            Properties.Select(tokenAndValue => new SettingData(tokenAndValue.Key, tokenAndValue.Value)),
            MessagePack.Resolvers.ContractlessStandardResolverAllowPrivate.Options
        );
    }

    public void LoadData(string filePath)
    {
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var datas = MessagePackSerializer.Deserialize<IEnumerable<SettingData>>(
            fs,
            MessagePack.Resolvers.ContractlessStandardResolverAllowPrivate.Options
        );
        foreach (var data in datas)
        {
            Properties[data.Token] = data.Value;
        }
    }

    protected override void DisposeManagedResource() { }

    protected override void DisposeUnmanagedResource() { }

    protected override void OnDisposed() { }
}
