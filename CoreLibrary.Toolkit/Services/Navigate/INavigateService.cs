namespace Zeng.CoreLibrary.Toolkit.Services.Navigate;

/// <summary>
/// 导航功能服务
/// </summary>
public interface INavigateService
{
    /// <summary>
    /// 当导航发生时触发
    /// </summary>
    event Action<INavigateService, object?>? OnNavigated;

    /// <summary>
    /// 取消绑定所有的导航事件
    /// </summary>
    void Unbind();

    /// <summary>
    /// 注册导航路由
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="route"></param>
    /// <returns></returns>
    INavigateService RegisterViewRoute<T>(string route) where T : new();

    /// <summary>
    /// 注册导航路由
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="route"></param>
    /// <param name="viewFactory"></param>
    /// <returns></returns>
    INavigateService RegisterViewRoute<T>(string route, Func<T> viewFactory) where T : class;

    /// <summary>
    /// 导航到指定路由
    /// </summary>
    /// <remarks>
    /// 若路由发生变化则触发 <see cref="OnNavigated"/> 事件
    /// </remarks>
    /// <param name="route"></param>
    void Navigate(string route);

    /// <summary>
    /// 回到上一个路由
    /// </summary>
    void Back();

    /// <summary>
    /// 能否回退
    /// </summary>
    /// <returns></returns>
    bool CanBack();

    /// <summary>
    /// 强制刷新
    /// </summary>
    /// <remarks>
    /// 会触发 <see cref="OnNavigated"/> 事件
    /// </remarks>
    void ForceRefresh();

    /// <summary>
    /// 当前路由
    /// </summary>
    string CurrentRoute { get; }

    /// <summary>
    /// 默认空服务
    /// </summary>
    public static INavigateService Empty { get; } = new EmptyService();

    private sealed class EmptyService : INavigateService
    {
        public string CurrentRoute { get; } = "";

        public event Action<INavigateService, object>? OnNavigated;

        public INavigateService RegisterViewRoute<T>(string route) where T : new() => this;

        public INavigateService RegisterViewRoute<T>(string route, Func<T> viewFactory) where T : class => this;

        public void Navigate(string route) { }
        public void Back() { }
        public bool CanBack() => false;

        public void ForceRefresh() { }

        public void Unbind() { }
    }
}