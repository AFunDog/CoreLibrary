using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.Structs;

public sealed class LocalizationChangedEventArgs : EventArgs
{
    public string Key { get; }

    public LocalizationChangedEventArgs(string key)
    {
        Key = key;
    }
}
