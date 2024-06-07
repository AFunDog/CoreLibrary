using CoreServices.Setting.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.Setting.Structs;

public class SettingValueCommand(
    Action<object, SettingValueChangeEvenArgs> executeSettingValueChangingCommand,
    Action<object, SettingValueChangeEvenArgs> executeSettingValueChangedCommand,
    Func<object,SettingValueChangeEvenArgs,bool>? canExecuteChangingCommand = null,
    Func<object, SettingValueChangeEvenArgs, bool>? canExecuteChangedCommand = null,
    Func<object,SettingValueChangeEvenArgs,bool>? canModifySettingValue = null) : ISettingValueCommand
{
    private static readonly Func<object, SettingValueChangeEvenArgs, bool> DefaultCanFunc = (s, e) => true;


    private readonly Func<object,SettingValueChangeEvenArgs,bool> _canExecuteChangedCommand = canExecuteChangedCommand ?? DefaultCanFunc;
    private readonly Func<object, SettingValueChangeEvenArgs, bool> _canExecuteChangingCommand = canExecuteChangingCommand ?? DefaultCanFunc;
    private readonly Func<object, SettingValueChangeEvenArgs, bool> _canModifySettingValue = canModifySettingValue ?? DefaultCanFunc;
    private readonly Action<object, SettingValueChangeEvenArgs> _executeSettingValueChangedCommand = executeSettingValueChangedCommand;
    private readonly Action<object, SettingValueChangeEvenArgs> _executeSettingValueChangingCommand = executeSettingValueChangingCommand;



    public bool CanExecuteChangedCommand(object sender, SettingValueChangeEvenArgs args) => _canExecuteChangedCommand(sender, args);
    public bool CanExecuteChangingCommand(object sender, SettingValueChangeEvenArgs args) => _canExecuteChangingCommand(sender, args);
    public bool CanModifySettingValue(object sender, SettingValueChangeEvenArgs args)=>_canModifySettingValue(sender, args);
    public void ExecuteSettingValueChangedCommand(object sender, SettingValueChangeEvenArgs args)=>_executeSettingValueChangedCommand(sender, args);
    public void ExecuteSettingValueChangingCommand(object sender, SettingValueChangeEvenArgs args)=>_executeSettingValueChangingCommand(sender, args);
}

