namespace Zeng.CoreLibrary.Toolkit.Services.DataBinding.Contracts;

public interface IValueConverter
{
    object Convert(object sourceValue, Type targetType, object? parameter);
}
