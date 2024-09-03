using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Formatters;

namespace CoreLibrary.Toolkit.Services.Setting.Structs;

[MessagePackObject]
public readonly record struct SettingRecord(
    [property: Key(0)] string Key,
    [property: Key(1), MessagePackFormatter(typeof(TypelessFormatter))] object Value
);
