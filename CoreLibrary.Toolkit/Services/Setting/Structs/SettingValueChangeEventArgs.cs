using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.Services.Setting.Structs
{
    public sealed class SettingValueChangeEventArgs(SettingValue settingValue, object oldValue, object newValue)
        : EventArgs
    {
        public SettingValue SettingValue { get; } = settingValue;

        public object OldValue { get; } = oldValue;

        public object NewValue { get; } = newValue;
    }
}
