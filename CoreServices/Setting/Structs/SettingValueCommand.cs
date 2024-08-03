using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreServices.Setting.Contracts;

namespace CoreServices.Setting.Structs;

public sealed class SettingValueCommand(
    Action<SettingValue, SettingValueChangeEvenArgs>? executeSettingValueChangedCommand = null,
    Action<SettingValue, SettingValueChangeEvenArgs>? executeSettingValueChangingCommand = null,
    Func<SettingValue, SettingValueChangeEvenArgs, bool>? canExecuteChangedCommand = null,
    Func<SettingValue, SettingValueChangeEvenArgs, bool>? canExecuteChangingCommand = null,
    Func<SettingValue, SettingValueChangeEvenArgs, bool>? canModifySettingValue = null
) : ISettingValueCommand
{
    private static readonly Func<SettingValue, SettingValueChangeEvenArgs, bool> DefaultCanFunc = (s, e) => true;

    private readonly Func<SettingValue, SettingValueChangeEvenArgs, bool> _canExecuteChangedCommand =
        canExecuteChangedCommand ?? DefaultCanFunc;
    private readonly Func<SettingValue, SettingValueChangeEvenArgs, bool> _canExecuteChangingCommand =
        canExecuteChangingCommand ?? DefaultCanFunc;
    private readonly Func<SettingValue, SettingValueChangeEvenArgs, bool> _canModifySettingValue =
        canModifySettingValue ?? DefaultCanFunc;
    private readonly Action<SettingValue, SettingValueChangeEvenArgs>? _executeSettingValueChangedCommand =
        executeSettingValueChangedCommand;
    private readonly Action<SettingValue, SettingValueChangeEvenArgs>? _executeSettingValueChangingCommand =
        executeSettingValueChangingCommand;

    public bool CanExecuteChangedCommand(SettingValue sender, SettingValueChangeEvenArgs args) =>
        _canExecuteChangedCommand(sender, args);

    public bool CanExecuteChangingCommand(SettingValue sender, SettingValueChangeEvenArgs args) =>
        _canExecuteChangingCommand(sender, args);

    public bool CanModifySettingValue(SettingValue sender, SettingValueChangeEvenArgs args) =>
        _canModifySettingValue(sender, args);

    public void ExecuteSettingValueChangedCommand(SettingValue sender, SettingValueChangeEvenArgs args) =>
        _executeSettingValueChangedCommand?.Invoke(sender, args);

    public void ExecuteSettingValueChangingCommand(SettingValue sender, SettingValueChangeEvenArgs args) =>
        _executeSettingValueChangingCommand?.Invoke(sender, args);
}
