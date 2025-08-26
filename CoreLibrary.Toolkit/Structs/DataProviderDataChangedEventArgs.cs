namespace Zeng.CoreLibrary.Toolkit.Structs;

public sealed class DataProviderDataChangedEventArgs<T> : EventArgs where T : notnull
{
    public T? OldData { get; init; }
    public T? NewData { get; init; }
}