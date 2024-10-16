﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Services.Setting.Structs;

namespace CoreLibrary.Toolkit.Services.Setting.Contracts;

public interface ISettingValueCommand
{
    /// <summary>
    /// 能否执行值正在改变命令
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public bool CanExecuteChangingCommand(SettingValue sender, SettingValueChangeEventArgs args);

    /// <summary>
    /// 执行值正在改变命令
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void ExecuteSettingValueChangingCommand(SettingValue sender, SettingValueChangeEventArgs args);

    /// <summary>
    /// 能否执行值完成改变命令
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public bool CanExecuteChangedCommand(SettingValue sender, SettingValueChangeEventArgs args);

    /// <summary>
    /// 执行值完成改变命令
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void ExecuteSettingValueChangedCommand(SettingValue sender, SettingValueChangeEventArgs args);

    /// <summary>
    /// 能否修改设置值
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public bool CanModifySettingValue(SettingValue sender, SettingValueChangeEventArgs args);
}
