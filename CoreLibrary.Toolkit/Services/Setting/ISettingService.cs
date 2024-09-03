using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Services.Setting.Structs;

namespace CoreLibrary.Toolkit.Services.Setting
{
    /// <summary>
    /// 设置服务
    /// </summary>
    public interface ISettingService
    {
        /// <summary>
        /// 设置字典
        /// </summary>
        IReadOnlyDictionary<string, SettingConfiguration> Settings { get; }

        /// <summary>
        /// 设置分组信息
        /// </summary>
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyCollection<string>>> GroupInfos { get; }

        /// <summary>
        /// 设置信息配置器
        /// </summary>
        interface ISettingServiceBuilder
        {
            ISettingServiceBuilder ConfigureGroup(string groupKey);
            ISettingServiceBuilder ConfigureSetting(
                SettingConfiguration settingConfiguration,
                string? groupKey = null,
                string? ownerKey = null
            );
        }

        /// <summary>
        /// 配置设置信息
        /// </summary>
        /// <param name="builder">配置器</param>
        void BuildSettings(Action<ISettingServiceBuilder> builder);

        /// <summary>
        /// 快速获取某个设置键的值
        /// </summary>
        /// <param name="key">设置键</param>
        /// <returns>值</returns>
        object? GetSettingValue(string key);

        /// <summary>
        /// 快速设置某个设置键的值，会触发更改命令
        /// </summary>
        /// <param name="key">设置键</param>
        /// <param name="value">值</param>
        void SetSettingValue(string key, object value);

        /// <summary>
        /// 保存设置到本地文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        void SaveSettings(string filePath);

        /// <summary>
        /// 保存设置到本地文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveSettingsAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// 读取文件夹中所有设置文件的数据
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        void ReadSettings(string dirPath);

        /// <summary>
        /// 读取文件夹中所有设置文件的数据
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ReadSettingsAsync(string dirPath, CancellationToken cancellationToken = default);

        /// <summary>
        /// 尝试执行所有更改命令，并不会实际改变设置的值，只是触发命令。
        /// </summary>
        void TryExecuteAllCommands();
    }
}
