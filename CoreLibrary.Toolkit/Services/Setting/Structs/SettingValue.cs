using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Services.Setting.Contracts;

namespace CoreLibrary.Toolkit.Services.Setting.Structs;

public sealed class SettingValueChangeEvenArgs(object oldValue, object newValue) : EventArgs
{
    public object OldValue { get; } = oldValue;
    public object NewValue { get; } = newValue;
}

public class SettingValue : INotifyPropertyChanging, INotifyPropertyChanged
{
    protected bool _isChanged;
    protected object _internalValue;
    protected object _value;

    protected readonly object _defValue;
    protected ISettingValueCommand _command;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event PropertyChangingEventHandler? PropertyChanging;

    public ISettingValueCommand Command => _command;

    /// <summary>
    /// 当前值是否为默认值
    /// </summary>
    public virtual bool IsDefValue => _value.Equals(_defValue);

    /// <summary>
    /// 在上一次应用或取消更改前设置值是否被改变过
    /// </summary>
    public bool IsChanged
    {
        get => _isChanged;
        protected set
        {
            PropertyChanging?.Invoke(this, new(nameof(IsChanged)));
            _isChanged = value;
            PropertyChanged?.Invoke(this, new(nameof(IsChanged)));
        }
    }

    /// <summary>
    /// 设置的内部实际值
    /// </summary>
    public object InternalValue
    {
        get => _internalValue;
        protected set
        {
            if (!CanModfiyInternalValue(this, new(_internalValue, value)))
                return;
            OnInternalValueChanging(this, new(_internalValue, value));
            _internalValue = value;
            Value = value;
            IsChanged = true;
            OnInternalValueChanged(this, new(_internalValue, value));
        }
    }

    /// <summary>
    /// 设置值。更改此属性并不会产生实际作用，则有应用设置时才会有用。
    /// </summary>
    public object Value
    {
        get => _value;
        set
        {
            PropertyChanging?.Invoke(this, new(nameof(Value)));
            _value = value;
            if (!_value.Equals(_internalValue))
                IsChanged = true;
            PropertyChanged?.Invoke(this, new(nameof(Value)));
        }
    }

    public SettingValue(object defValue, ISettingValueCommand command)
    {
        _defValue = defValue;
        _internalValue = defValue;
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
    /// 应用设置的更改，触发更改命令
    /// </summary>
    public virtual void ApplyChange() => InternalValue = Value;

    /// <summary>
    /// 取消设置的更改，不触发更改命令
    /// </summary>
    public virtual void CancelChange() => Value = InternalValue;

    /// <summary>
    /// 重置设置为默认值
    /// </summary>
    public virtual void ResetValue()
    {
        InternalValue = _defValue;
    }

    protected virtual bool CanModfiyInternalValue(SettingValue sender, SettingValueChangeEvenArgs e) =>
        _command.CanModifySettingValue(sender, e);

    protected virtual void OnInternalValueChanging(SettingValue sender, SettingValueChangeEvenArgs e)
    {
        //Debug.WriteLine($"{e.NewValue} {e.OldValue}");
        if (_command.CanExecuteChangingCommand(sender, e))
            _command.ExecuteSettingValueChangingCommand(sender, e);
    }

    protected virtual void OnInternalValueChanged(SettingValue sender, SettingValueChangeEvenArgs e)
    {
        if (_command.CanExecuteChangedCommand(sender, e))
            _command.ExecuteSettingValueChangedCommand(sender, e);
    }
}
