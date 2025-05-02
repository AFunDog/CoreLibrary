namespace Zeng.CoreLibrary.Core.Structs;

/// <summary>
/// 执行结果 用于函数返回值
/// </summary>
/// <typeparam name="TR">正确时返回值类型</typeparam>
/// <typeparam name="TE">错误时返回值类型</typeparam>
public abstract class Result<TR, TE> { }

public sealed class Success<TR, TE> : Result<TR, TE>
{
    public TR Value { get; }

    public Success(TR value)
    {
        Value = value;
    }
}

public sealed class Error<TR, TE> : Result<TR, TE>
{
    public TE Value { get; }

    public Error(TE value)
    {
        Value = value;
    }
}
