using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Services.Setting.Structs;
using MessagePack.Formatters;

namespace CoreLibrary.Toolkit.Services.Setting;

/// <summary>
/// <see cref="ISettingService"/> 设置服务
/// </summary>
public interface ISettingService
{
    /// <summary>
    /// 新建 <see cref="ISettingService"/> 的实现实例
    /// </summary>
    public static ISettingService Implement => new SettingService();

    void RegisterModel(Type modelType);

    T GetValue<T>(SettingProperty<T> property);

    void SetValue<T>(SettingProperty<T> property, T value);

    void SaveData(string filePath);

    void LoadData(string filePath);
}
