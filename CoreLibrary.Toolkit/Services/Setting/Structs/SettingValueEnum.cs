using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Services.Setting.Contracts;

namespace CoreLibrary.Toolkit.Services.Setting.Structs;

public sealed class SettingValueEnum : SettingValue
{
    private readonly EnumValue[] _enum;
    public IList<EnumValue> Enum => _enum;

    public SettingValueEnum(int defvalue, EnumValue[] @enum, ISettingValueCommand command)
        : base(defvalue, command)
    {
        _enum = @enum;
    }

    // 重写事件参数,传递的 Value 改为 EnumValue 的 Parameter
    protected override bool CanModfiyInternalValue(SettingValue sender, SettingValueChangeEvenArgs e)
    {
        return _command.CanModifySettingValue(
            sender,
            new(_enum[(int)e.OldValue].Parameter, _enum[(int)e.NewValue].Parameter)
        );
    }

    protected override void OnInternalValueChanging(SettingValue sender, SettingValueChangeEvenArgs e)
    {
        base.OnInternalValueChanging(sender, new(_enum[(int)e.OldValue].Parameter, _enum[(int)e.NewValue].Parameter));
    }

    protected override void OnInternalValueChanged(SettingValue sender, SettingValueChangeEvenArgs e)
    {
        base.OnInternalValueChanged(sender, new(_enum[(int)e.OldValue].Parameter, _enum[(int)e.NewValue].Parameter));
    }
}
