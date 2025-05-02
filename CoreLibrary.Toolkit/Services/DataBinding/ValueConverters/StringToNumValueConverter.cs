using Zeng.CoreLibrary.Toolkit.Services.DataBinding.Contracts;

namespace Zeng.CoreLibrary.Toolkit.Services.DataBinding.ValueConverters;

public sealed class StringToNumValueConverter : IValueConverter
{
    public object Convert(object sourceValue, Type targetType, object? parameter)
    {
        return int.Parse((string)sourceValue);
    }
}
