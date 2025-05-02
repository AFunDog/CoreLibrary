using Zeng.CoreLibrary.Toolkit.Services.DataBinding.Contracts;

namespace Zeng.CoreLibrary.Toolkit.Services.DataBinding.ValueConverters;

public sealed class EmptyValueConverter : IValueConverter
{
    public object Convert(object sourceValue, Type targetType, object? parameter)
    {
        return sourceValue;
    }
}
