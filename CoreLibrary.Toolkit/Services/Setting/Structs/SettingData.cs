using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Formatters;

namespace CoreLibrary.Toolkit.Services.Setting.Structs;

internal readonly record struct SettingData(
    string Token,
    [property: MessagePackFormatter(typeof(TypelessFormatter))] object? Value
);
