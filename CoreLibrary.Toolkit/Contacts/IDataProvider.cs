using Zeng.CoreLibrary.Toolkit.Structs;

namespace Zeng.CoreLibrary.Toolkit.Contacts;

/// <summary>
/// 数据提供者接口
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public interface IDataProvider<T> where T : notnull
{
    /// <summary>
    /// 默认空实现
    /// </summary>
    public static IDataProvider<T> Empty { get; } = new EmptyProvider();
    /// <summary>
    /// 数据发生改变事件
    /// </summary>
    event Action<IDataProvider<T>, DataProviderDataChangedEventArgs<T>>? DataChanged;

    /// <summary>
    /// 数据
    /// </summary>
    T? Data { get; }

    /// <summary>
    /// 主动加载数据
    /// </summary>
    void LoadData();

    /// <summary>
    /// 主动加载数据（异步）
    /// </summary>
    /// <returns></returns>
    public virtual Task LoadDataAsync(CancellationToken cancellationToken = default) => Task.Run(LoadData, cancellationToken);

    private sealed class EmptyProvider : IDataProvider<T>
    {
        public T? Data => default;

        public event Action<IDataProvider<T>,DataProviderDataChangedEventArgs<T>>? DataChanged;

        public void LoadData()
        {
        }

        // public Task LoadDataAsync() => Task.CompletedTask;
    }
}