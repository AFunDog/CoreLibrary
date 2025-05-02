namespace Zeng.CoreLibrary.Core.Structs;

[Obsolete("用 Result 替代这种不强制性的结果")]
public enum ResultType
{
    Success,
    Warning,
    Error,
}

/// <summary>
/// 执行结果 用于函数返回值
/// </summary>
/// <param name="ResultType"></param>
/// <param name="Message"></param>
[Obsolete("用 Result 替代这种不强制性的结果")]
public readonly record struct ActionResult(ResultType ResultType, string Message)
{
    public bool IsSucceed => ResultType == ResultType.Success;

    public static ActionResult Success(string message = "") => new(ResultType.Success, message);

    public static ActionResult Warning(string message) => new(ResultType.Warning, message);

    public static ActionResult Error(string message) => new(ResultType.Error, message);
}
