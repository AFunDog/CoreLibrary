using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Services.DataBinding.Contracts;

namespace CoreLibrary.Toolkit.Services.DataBinding.Structs;

internal sealed record BindingInfo(object Target, string TargetProperty, IValueConverter ValueConverter) { }
