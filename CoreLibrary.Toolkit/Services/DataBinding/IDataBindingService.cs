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
using CoreLibrary.Toolkit.Services.Setting;

namespace CoreLibrary.Toolkit.Services.DataBinding;

public interface IDataBindingService
{
    public static IDataBindingService Implement => new DataBindingService();

    IDataBindingService Bind(
        INotifyPropertyChanged source,
        string sourceProperty,
        object target,
        string targetProperty,
        IValueConverter? valueConverter = null
    );

    IDataBindingService UnBind(
        INotifyPropertyChanged source,
        string sourceProperty,
        object target,
        string targetProperty
    );

    IDataBindingService BindCollection(
        INotifyCollectionChanged collection,
        Action<INotifyCollectionChanged, IList> itemsAdded,
        Action<INotifyCollectionChanged, IList> itemsRemoved
    );

    IDataBindingService UnBindCollection(INotifyCollectionChanged collection);
    IDataBindingService UnBindCollection(
        INotifyCollectionChanged collection,
        Action<INotifyCollectionChanged, IList> itemsAdded,
        Action<INotifyCollectionChanged, IList> itemsRemoved
    );
}
