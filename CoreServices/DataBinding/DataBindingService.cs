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
        // source sourceProperty target targetProperty
        private readonly Dictionary<INotifyPropertyChanged, Dictionary<PropertyInfo, List<(object Target, PropertyInfo TargetProperty)>>> _bindings = [];

        public void Bind(INotifyPropertyChanged source, PropertyInfo sourceProperty, object target, PropertyInfo targetProperty)
        {
            var targetInfo = (target, targetProperty);
            if (_bindings.TryGetValue(source, out var values))
            {
                if (values.TryGetValue(sourceProperty, out var list))
                {
                    if (!list.Contains(targetInfo))
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
            targetProperty.SetValue(target, sourceProperty.GetValue(source));
        }

        public void UnBind(INotifyPropertyChanged source, PropertyInfo sourceProperty, object target, PropertyInfo targetProperty)
        {
            if (_bindings.TryGetValue(source, out var values))
            {
                if (values.TryGetValue(sourceProperty, out var list))
                {
                    if (list.Contains((target, targetProperty)))
                    {
                        list.Remove((target, targetProperty));
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

        }

        private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is null || sender is null || sender.GetType().GetProperty(e.PropertyName) is null)
                return;


            if (_bindings.TryGetValue((sender as INotifyPropertyChanged)!, out var binds))
            {
                if (binds.TryGetValue(sender.GetType().GetProperty(e.PropertyName)!, out var list))
                {
                    foreach ((var target, var targetProperty) in list)
                    {
                        targetProperty.SetValue(target, sender.GetType().GetProperty(e.PropertyName)!.GetValue(sender));
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
