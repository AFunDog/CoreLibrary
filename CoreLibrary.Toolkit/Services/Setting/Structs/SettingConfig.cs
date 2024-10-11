using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.Services.Setting.Structs
{
    public sealed record SettingConfig(string Key, SettingValue SettingValue, AttachedArgs? AttachedArgs = null)
    {
        //public string SelfKey => Key.Split('.').Last();
    }
}
