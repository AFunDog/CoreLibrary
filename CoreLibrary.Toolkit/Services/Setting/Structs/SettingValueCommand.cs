using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Services.Setting.Contracts;

namespace CoreLibrary.Toolkit.Services.Setting.Structs;

public sealed class SettingValueCommand(
    Action<SettingValue, SettingValueChangeEventArgs>? executeSettingValueChangedCommand = null,
    Action<SettingValue, SettingValueChangeEventArgs>? executeSettingValueChangingCommand = null,
    Func<SettingValue, SettingValueChangeEventArgs, bool>? canExecuteChangedCommand = null,
    Func<SettingValue, SettingValueChangeEventArgs, bool>? canExecuteChangingCommand = null,
    Func<SettingValue, SettingValueChangeEventArgs, bool>? canModifySettingValue = null
) : ISettingValueCommand
{
    private static readonly Func<SettingValue, SettingValueChangeEventArgs, bool> DefaultCanFunc = (s, e) => true;

    private readonly Func<SettingValue, SettingValueChangeEventArgs, bool> _canExecuteChangedCommand =
        canExecuteChangedCommand ?? DefaultCanFunc;
    private readonly Func<SettingValue, SettingValueChangeEventArgs, bool> _canExecuteChangingCommand =
        canExecuteChangingCommand ?? DefaultCanFunc;
    private readonly Func<SettingValue, SettingValueChangeEventArgs, bool> _canModifySettingValue =
        canModifySettingValue ?? DefaultCanFunc;
    private readonly Action<SettingValue, SettingValueChangeEventArgs>? _executeSettingValueChangedCommand =
        executeSettingValueChangedCommand;
    private readonly Action<SettingValue, SettingValueChangeEventArgs>? _executeSettingValueChangingCommand =
        executeSettingValueChangingCommand;

    public bool CanExecuteChangedCommand(SettingValue sender, SettingValueChangeEventArgs args) =>
        _canExecuteChangedCommand(sender, args);

    public bool CanExecuteChangingCommand(SettingValue sender, SettingValueChangeEventArgs args) =>
        _canExecuteChangingCommand(sender, args);

    public bool CanModifySettingValue(SettingValue sender, SettingValueChangeEventArgs args) =>
        _canModifySettingValue(sender, args);

    public void ExecuteSettingValueChangedCommand(SettingValue sender, SettingValueChangeEventArgs args) =>
        _executeSettingValueChangedCommand?.Invoke(sender, args);

    public void ExecuteSettingValueChangingCommand(SettingValue sender, SettingValueChangeEventArgs args) =>
        _executeSettingValueChangingCommand?.Invoke(sender, args);
}
