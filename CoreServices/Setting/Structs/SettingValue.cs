using CoreServices.Setting.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.Setting.Structs;

public sealed class SettingValueChangeEvenArgs(object oldValue, object newValue) : EventArgs
{
    public object OldValue { get; } = oldValue;
    public object NewValue { get; } = newValue;

}

public class SettingValue : INotifyPropertyChanging,INotifyPropertyChanged
{
    protected object _value;

    protected readonly object _defValue;
    protected ISettingValueCommand _command;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    public ISettingValueCommand Command => _command;
    public object Value
    {
        get => _value;
        set
        {
            if (!CanModfiySettingValue(this, new(_value, value)))
                return;
            OnSettingValueChanging(this, new(_value, value));
            PropertyChanging?.Invoke(this, new(nameof(Value)));
            _value = value;
            OnSettingValueChanged(this, new(_value, value));
            PropertyChanged?.Invoke(this, new(nameof(Value)));
        }
    }


    public SettingValue(object defValue, ISettingValueCommand command)
    {
        _defValue = defValue;
        _value = defValue;
        _command = command;
    }

    ~SettingValue()
    {
        PropertyChanged = null;
        PropertyChanging = null;
    }

    /// <summary>
    /// 初始化设置的值，使用此函数改变设置的值且不会触发事件和指令
    /// </summary>
    /// <param name="value"></param>
    internal virtual void InitValue(object? value = null)
    {
        _value = value ?? _defValue;
    }

    /// <summary>
    /// 重置设置为默认值
    /// </summary>
    public virtual void ResetValue()
    {
        Value = _defValue;
    }

    /// <summary>
    /// 当前值是否为默认值
    /// </summary>
    /// <returns></returns>
    public virtual bool IsDefValue()
    {
        return _value.Equals(_defValue);
    }
    protected virtual bool CanModfiySettingValue(SettingValue sender, SettingValueChangeEvenArgs e)
    {
        return _command.CanModifySettingValue(sender, e);
    }
    protected virtual void OnSettingValueChanging(SettingValue sender, SettingValueChangeEvenArgs e)
    {
        if (_command.CanExecuteChangingCommand(sender, e))
        {
            _command.ExecuteSettingValueChangingCommand(sender, e);
        }
    }
    protected virtual void OnSettingValueChanged(SettingValue sender, SettingValueChangeEvenArgs e)
    {
        if (_command.CanExecuteChangedCommand(sender, e))
        {
            _command.ExecuteSettingValueChangedCommand(sender, e);
        }
    }
}