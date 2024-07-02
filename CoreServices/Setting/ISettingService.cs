using CoreServices.Setting.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.Setting
{
    public interface ISettingService
    {
        IDictionary<string,SettingValue> Settings { get; }

        interface ISettingServiceBuilder
        {
            ISettingServiceBuilder Configure(string key,SettingValue setting);
        }

        void BuildSettings(Action<ISettingServiceBuilder> builder);

        SettingValue? GetSetting(string key);
        object GetSettingValue(string key, object defaultValue);
        void SetSettingValue(string key, object value);

        void SaveSettings(string path);
        void ReadSettings(string path);
        void TryExecuteAllCommands();
    }
}
