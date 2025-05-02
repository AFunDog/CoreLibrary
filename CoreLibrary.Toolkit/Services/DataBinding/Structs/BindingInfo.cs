using Zeng.CoreLibrary.Toolkit.Services.DataBinding.Contracts;

namespace Zeng.CoreLibrary.Toolkit.Services.DataBinding.Structs;

internal sealed record BindingInfo(object Target, string TargetProperty, IValueConverter ValueConverter) { }
