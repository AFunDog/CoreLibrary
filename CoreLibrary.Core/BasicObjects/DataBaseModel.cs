using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CoreLibrary.Core.BasicObjects
{
    /// <summary>
    /// 数据基础模型，基于 <see cref="ObservableObject"/>
    /// </summary>
    /// <remarks>
    /// 附加功能
    /// <list type="bullet">
    /// <item>基于 <see cref="ObservableObject"/> 的基本功能</item>
    /// </list>
    /// </remarks>
    public abstract class DataBaseModel : ObservableObject { }
}
