using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CoreServices.DataBinding.Contracts;

namespace CoreServices.DataBinding.Structs
{
    internal sealed record BindingInfo(object Target, string TargetProperty, IValueConverter ValueConverter) { }
}
