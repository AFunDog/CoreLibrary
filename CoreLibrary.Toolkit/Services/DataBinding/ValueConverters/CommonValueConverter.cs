using Zeng.CoreLibrary.Toolkit.Services.DataBinding.Contracts;

namespace Zeng.CoreLibrary.Toolkit.Services.DataBinding.ValueConverters;

public sealed class CommonValueConverter(Func<object, Type, object?, object> converter) : IValueConverter
{
    private Func<object, Type, object?, object> _converter = converter;

    public object Convert(object sourceValue, Type targetType, object? parameter)
    {
        return _converter(sourceValue, targetType, parameter);
    }
}
