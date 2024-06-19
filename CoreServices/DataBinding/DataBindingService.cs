using CoreServices.DataBinding.Contracts;
using CoreServices.DataBinding.Structs;
using CoreServices.DataBinding.ValueConverters;
using CoreServices.Template;
using System;
using System.Collections.Generic;
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

        protected override void DestoryManagedResource()
        {
            _bindings.Clear();
        }

        protected override void DestoryUnmanagedResource()
        {

        }
    }
}
