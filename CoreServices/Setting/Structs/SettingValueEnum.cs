using CoreServices.Setting.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.Setting.Structs;

public class SettingValueEnum : SettingValue
{
    private readonly EnumValue[] _enum;
    public EnumValue[] Enum => _enum;
    public SettingValueEnum(int defvalue, EnumValue[] @enum, ISettingValueCommand command) : base(defvalue, command)
    {
        _enum = @enum;
    }
}

