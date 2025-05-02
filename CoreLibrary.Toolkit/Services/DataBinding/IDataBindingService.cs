using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using Zeng.CoreLibrary.Toolkit.Services.DataBinding.Contracts;

namespace Zeng.CoreLibrary.Toolkit.Services.DataBinding;

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
