using CoreServices.DataBinding.Contracts;
using CoreServices.DataBinding.Structs;
using CoreServices.DataBinding.ValueConverters;
using CoreServices.Template;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.DataBinding
{
    public class DataBindingService : DisposableTemplate
    {
        // source sourceProperty bindingInfo
        private readonly Dictionary<INotifyPropertyChanged, Dictionary<PropertyInfo, List<BindingInfo>>> _bindings = [];
        private readonly Dictionary<INotifyCollectionChanged, List<(Action<INotifyCollectionChanged, IList> itemsAdded, Action<INotifyCollectionChanged, IList> itemsRemoved)>> _bindCollections = [];

        public DataBindingService Bind(INotifyPropertyChanged source, PropertyInfo sourceProperty, object target, PropertyInfo targetProperty, IValueConverter? valueConverter = null)
        {
            if (valueConverter is null)
                valueConverter = new EmptyValueConverter();

            BindingInfo targetInfo = new(target, targetProperty, valueConverter);
            if (_bindings.TryGetValue(source, out var values))
            {
                if (values.TryGetValue(sourceProperty, out var list))
                {
                    if (list.Find(s => s.Target == target && s.TargetProperty == targetProperty) is null)
                    {
                        list.Add(targetInfo);
                    }
                }
                else
                {
                    values.Add(sourceProperty, [targetInfo]);
                }
            }
            else
            {
                _bindings.Add(source, new() { [sourceProperty] = [targetInfo] });
                source.PropertyChanged += OnSourcePropertyChanged;
            }
            targetProperty.SetValue(target, valueConverter.Convert(sourceProperty.GetValue(source)!, target.GetType(), null));
            return this;
        }
        public DataBindingService UnBind(INotifyPropertyChanged source, PropertyInfo sourceProperty, object target, PropertyInfo targetProperty)
        {
            if (_bindings.TryGetValue(source, out var values))
            {
                if (values.TryGetValue(sourceProperty, out var list))
                {
                    if (list.Find(s => s.Target == target && s.TargetProperty == targetProperty) is BindingInfo info)
                    {
                        list.Remove(info);
                    }
                    if (list.Count == 0)
                    {
                        values.Remove(sourceProperty);
                    }
                }
                if (values.Count == 0)
                {
                    _bindings.Remove(source);
                }
            }
            return this;
        }
        public DataBindingService BindCollection(
            INotifyCollectionChanged collection,
            Action<INotifyCollectionChanged, IList> itemsAdded,
            Action<INotifyCollectionChanged, IList> itemsRemoved)
        {
            var actions = (itemsAdded, itemsRemoved);
            if (_bindCollections.TryGetValue(collection, out var list))
            {
                list.Add(actions);
            }
            else
            {
                _bindCollections[collection] = [actions];
                collection.CollectionChanged += OnCollectionChanged;
            }
            return this;
        }
        public DataBindingService UnBindCollection(
            INotifyCollectionChanged collection)
        {
            if (_bindCollections.TryGetValue(collection, out var list))
            {
                collection.CollectionChanged -= OnCollectionChanged;
                _bindCollections.Remove(collection);
            }
            return this;
        }
        public DataBindingService UnBindCollection(
            INotifyCollectionChanged collection,
            Action<INotifyCollectionChanged, IList> itemsAdded,
            Action<INotifyCollectionChanged, IList> itemsRemoved)
        {
            if (_bindCollections.TryGetValue(collection, out var list))
            {
                if (list.Find(s => s.itemsAdded == itemsAdded && s.itemsRemoved == itemsRemoved) is var actions)
                {
                    list.Remove(actions);
                    if (list.Count == 0)
                    {
                        collection.CollectionChanged -= OnCollectionChanged;
                        _bindCollections.Remove(collection);
                    }
                }
            }
            return this;
        }

        private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is null || sender is null || sender.GetType().GetProperty(e.PropertyName) is null)
                return;


            if (_bindings.TryGetValue((sender as INotifyPropertyChanged)!, out var binds))
            {
                if (binds.TryGetValue(sender.GetType().GetProperty(e.PropertyName)!, out var list))
                {
                    foreach ((var target, var targetProperty, var valueConverter) in list)
                    {
                        targetProperty.SetValue(target, valueConverter.Convert(sender.GetType().GetProperty(e.PropertyName)!.GetValue(sender)!, target.GetType(), null));
                    }
                }
            }
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is null)
                return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is null)
                        break;
                    foreach(var action in _bindCollections[(sender as INotifyCollectionChanged)!])
                    {
                        action.itemsAdded((sender as INotifyCollectionChanged)!, e.NewItems);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is null)
                        break;
                    foreach (var action in _bindCollections[(sender as INotifyCollectionChanged)!])
                    {
                        action.itemsRemoved((sender as INotifyCollectionChanged)!, e.OldItems);
                    }

                    break;
                default:
                    break;
            }
        }


        protected override void DestoryManagedResource()
        {
            _bindings.Clear();
            _bindCollections.Clear();
        }

        protected override void DestoryUnmanagedResource()
        {

        }
    }
}
