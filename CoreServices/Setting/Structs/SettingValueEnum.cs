using CoreServices.Setting.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.Setting.Structs;

internal class SettingValueEnum : SettingValue
{
    private readonly EnumValue[] _enum;
    public EnumValue[] Enum => _enum;
    public SettingValueEnum(int defvalue, EnumValue[] @enum, ISettingValueCommand command) : base(defvalue, command)
    {
        _enum = @enum;
    }

    /// <summary>
    /// 初始化设置的值，只允许设置一次，之后都将无效
    /// </summary>
    /// <param name="value"></param>
    public override void InitValue(object? value = null)
    {
        if (value is null)
        {
            base._isValueInited = true;
        }
        if (base._isValueInited || value is not int)
            return;
        Value = value;

        base._isValueInited = true;
    }
}

