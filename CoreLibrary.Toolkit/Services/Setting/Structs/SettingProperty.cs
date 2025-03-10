using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.Services.Setting.Structs;

public class SettingProperty
{
    public string Name { get; }
    public object? DefValue { get; }

    public string Token { get; }

    protected SettingProperty(string name, object? defValue, string token)
    {
        Name = name;
        DefValue = defValue;
        Token = token;
    }
}

public sealed class SettingProperty<T> : SettingProperty
{
    public SettingProperty(Type ownerType, string name, T defValue)
        : base(name, defValue, $"{ownerType.FullName}.{typeof(T).FullName}.{name}") { }

    public T GetValue(ISettingService settingService)
    {
        return settingService.GetValue(this);
    }

    public void SetValue(ISettingService settingService, T value)
    {
        settingService.SetValue(this, value);
    }
}
