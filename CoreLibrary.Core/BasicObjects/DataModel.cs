using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Mapster;
using MessagePack;

namespace CoreLibrary.Core.BasicObjects
{
    // TODO : 性能方面还有问题，以后遇到瓶颈需要修改

    /// <summary>
    /// 数据模型，基于 <see cref="DataBaseModel"/>
    /// </summary>
    /// <remarks>
    /// 附加功能
    /// <list type="bullet" >
    ///     <item><see cref="IsChanged"/>  属性 - 获取数据是否已经发生改变</item>
    ///     <item><see cref="ApplyChange"/> 和 <see cref="CancelChange"/> 方法 - 应用或取消数据的更改 </item>
    /// </list>
    /// </remarks>
    /// <typeparam name="DataType">目标数据类型，即派生类类型</typeparam>
    public abstract class DataModel<DataType> : DataBaseModel
        where DataType : DataModel<DataType>
    {
        /// <summary>
        /// 数据是否已经发生改变
        /// </summary>
        /// <remarks>
        /// 调用 <see cref="ApplyChange"/> 和 <see cref="CancelChange"/> 会重置这个属性值为 false
        /// </remarks>
        public bool IsChanged { get; protected set; }

        [IgnoreMember]
        [AdaptIgnore]
        private byte[] _internalData = [];

        protected DataModel()
        {
            Console.WriteLine($"DataModel Constructor");
            _internalData = MessagePackSerializer.Serialize((DataType)this);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            IsChanged = true;
        }

        /// <summary>
        /// 当应用更改发生后执行，由派生类实现
        /// </summary>
        protected virtual void OnApplyChanged() { }

        /// <summary>
        /// 当取消更改发生后执行，由派生类实现
        /// </summary>
        protected virtual void OnCancelChanged() { }

        /// <summary>
        /// 应用更改
        /// </summary>
        /// <remarks>
        /// 此操作会重置 <see cref="IsChanged"/> 为 false
        /// </remarks>
        public void ApplyChange()
        {
            if (IsChanged)
            {
                IsChanged = false;
                _internalData = MessagePackSerializer.Serialize((DataType)this);
                OnApplyChanged();
            }
        }

        /// <summary>
        /// 取消更改
        /// </summary>
        /// <remarks>
        /// 此操作会重置 <see cref="IsChanged"/> 为 false
        /// </remarks>
        public void CancelChange()
        {
            if (IsChanged)
            {
                IsChanged = false;
                MessagePackSerializer.Deserialize<DataType>(_internalData).Adapt((DataType)this);
                OnCancelChanged();
            }
        }
    }
}
