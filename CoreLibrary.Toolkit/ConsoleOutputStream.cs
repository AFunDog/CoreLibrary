using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Core.Contacts;

namespace CoreLibrary.Toolkit;

[Obsolete]
public sealed class ConsoleOutputStream : DisposableObject
{
    public TextWriter Output { get; } = Console.Out;

    public ConsoleOutputStream() { }

    public static ConsoleOutputStream Create() => new();

    public ConsoleOutputStream Write(string format, params object?[] args)
    {
        Output.Write(format, args);
        return this;
    }

    public ConsoleOutputStream Write(string format, ConsoleColor color, params object?[] args)
    {
        Console.ForegroundColor = color;
        Output.Write(format, args);
        Console.ResetColor();
        return this;
    }

    public ConsoleOutputStream Write(object value, ConsoleColor? color = null)
    {
        if (color is not null)
            Console.ForegroundColor = color.Value;
        Output.Write(value.ToString());
        Console.ResetColor();
        return this;
    }

    public ConsoleOutputStream Contain(
        string leftBorder,
        Action<ConsoleOutputStream> content,
        string rightBorder,
        ConsoleColor? borderColor = null
    )
    {
        if (borderColor is not null)
            Console.ForegroundColor = borderColor.Value;
        Output.Write(leftBorder);
        Console.ResetColor();
        content(this);
        if (borderColor is not null)
            Console.ForegroundColor = borderColor.Value;
        Output.Write(rightBorder);
        Console.ResetColor();
        return this;
    }

    public ConsoleOutputStream NewLine()
    {
        Output.WriteLine();
        return this;
    }

    public void Close()
    {
        Console.ResetColor();
    }

    protected override void DisposeManagedResource()
    {
        Close();
    }

    protected override void DisposeUnmanagedResource() { }

    protected override void OnDisposed() { }
}
