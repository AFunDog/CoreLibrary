using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Core.Structs;

/// <summary>
/// 执行结果 用于函数返回值
/// </summary>
/// <typeparam name="T">正确时返回值类型</typeparam>
/// <typeparam name="E">错误时返回值类型</typeparam>
public abstract class Result<T, E> { }

public sealed class Success<T, E> : Result<T, E>
{
    public T Value { get; }

    public Success(T value)
    {
        Value = value;
    }
}

public sealed class Error<T, E> : Result<T, E>
{
    public E Value { get; }

    public Error(E value)
    {
        Value = value;
    }
}
