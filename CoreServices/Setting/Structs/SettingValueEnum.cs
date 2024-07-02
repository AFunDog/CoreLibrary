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
    public IList<EnumValue> Enum => _enum;
    public SettingValueEnum(int defvalue, EnumValue[] @enum, ISettingValueCommand command) : base(defvalue, command)
    {
        _enum = @enum;
        
    }
    // 重写事件参数,传递的 Value 改为 EnumValue 的 Parameter
    protected override bool CanModfiySettingValue(SettingValue sender, SettingValueChangeEvenArgs e)
    {
        return _command.CanModifySettingValue(sender, new(_enum[(int)e.OldValue].Parameter, _enum[(int)e.NewValue].Parameter));
    }
    protected override void OnSettingValueChanging(SettingValue sender, SettingValueChangeEvenArgs e)
    {
        base.OnSettingValueChanging(sender, new(_enum[(int)e.OldValue].Parameter, _enum[(int)e.NewValue].Parameter));
    }
    protected override void OnSettingValueChanged(SettingValue sender, SettingValueChangeEvenArgs e)
    {
        base.OnSettingValueChanged(sender, new(_enum[(int)e.OldValue].Parameter, _enum[(int)e.NewValue].Parameter));
    }
}

