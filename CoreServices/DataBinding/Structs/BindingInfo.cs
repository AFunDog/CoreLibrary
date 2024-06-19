using CoreServices.DataBinding.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.DataBinding.Structs
{
    internal record BindingInfo(object Target, PropertyInfo TargetProperty, IValueConverter ValueConverter)
    {
    }
}
