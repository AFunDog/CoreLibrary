using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.Services.Setting.Structs
{
    public sealed record SettingConfiguration(
        string OnlyKey,
        SettingValue SettingValue,
        AttachedArgs? AttachedArgs = null
    );
}
