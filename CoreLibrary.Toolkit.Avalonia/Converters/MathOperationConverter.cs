using System.Globalization;
using Avalonia.Data.Converters;

namespace Zeng.CoreLibrary.Toolkit.Avalonia.Converters;

/// <summary>
/// 数学运算转换器
/// </summary>
/// <remarks>
/// 运算符：+ - * /
/// 参数里填"运算符 运算数字"即可
/// </remarks>
public sealed class MathOperationConverter : IValueConverter
{
    public static MathOperationConverter Instance { get; } = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is not string arg)
            return value;
        dynamic source = value;
        try
        {
            var args = arg.Split(' ');
            var operation = args[0];
            var operateNum = double.Parse(args[1]);
            return operation switch
            {
                "+" => source + operateNum,
                "-" => source - operateNum,
                "*" => source * operateNum,
                "/" => source / operateNum,
                _ => value
            };
        }
        catch (Exception) { }

        try
        {
            var args = arg.Split(' ');
            var operation = args[0];
            var operateNum = int.Parse(args[1]);
            return operation switch
            {
                "+" => source + operateNum,
                "-" => source - operateNum,
                "*" => source * operateNum,
                "/" => source / operateNum,
                _ => value
            };
        }
        catch (Exception e) { }

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        try { }
        catch (Exception) { }

        return value;
    }
}