using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Services.DataBinding.Contracts;

namespace CoreLibrary.Toolkit.Services.DataBinding
{
    public interface IDataBindingService
    {
        DataBindingService Bind(
            INotifyPropertyChanged source,
            string sourceProperty,
            object target,
            string targetProperty,
            IValueConverter? valueConverter = null
        );

        DataBindingService UnBind(
            INotifyPropertyChanged source,
            string sourceProperty,
            object target,
            string targetProperty
        );

        DataBindingService BindCollection(
            INotifyCollectionChanged collection,
            Action<INotifyCollectionChanged, IList> itemsAdded,
            Action<INotifyCollectionChanged, IList> itemsRemoved
        );

        DataBindingService UnBindCollection(INotifyCollectionChanged collection);
        DataBindingService UnBindCollection(
            INotifyCollectionChanged collection,
            Action<INotifyCollectionChanged, IList> itemsAdded,
            Action<INotifyCollectionChanged, IList> itemsRemoved
        );
    }
}
