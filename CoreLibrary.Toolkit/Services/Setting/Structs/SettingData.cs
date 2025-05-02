using MessagePack;
using MessagePack.Formatters;

namespace Zeng.CoreLibrary.Toolkit.Services.Setting.Structs;

internal readonly record struct SettingData(
    string Token,
    [property: MessagePackFormatter(typeof(TypelessFormatter))] object? Value
);
