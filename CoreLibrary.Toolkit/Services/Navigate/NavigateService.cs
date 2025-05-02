using Serilog;

namespace Zeng.CoreLibrary.Toolkit.Services.Navigate;

internal sealed class NavigateService : INavigateService
{
    private ILogger Logger { get; }

    public event Action<INavigateService, object?>? OnNavigated;

    private Dictionary<string, Func<object>> RouteTable { get; } = [];

    public string CurrentRoute { get; private set; } = "";

    public NavigateService(ILogger logger)
    {
        Logger = logger;
    }

    public void Unbind()
    {
        OnNavigated = null;
    }

    public INavigateService RegisterViewRoute<T>(string route)
        where T : new()
    {
        RouteTable[route] = () => new T();
        return this;
    }

    public INavigateService RegisterViewRoute<T>(string route, Func<T> viewFactory)
        where T : class
    {
        RouteTable[route] = viewFactory;
        return this;
    }

    public void Navigate(string route)
    {
        // 如果 CurrentRoute 不变则不触发导航事件
        if (CurrentRoute == route)
            return;
        CurrentRoute = route;
        var view = GetRouteView();
        OnNavigated?.Invoke(this, view);
    }

    public void ForceRefresh()
    {
        var view = GetRouteView();
        OnNavigated?.Invoke(this, view);
    }

    private object? GetRouteView()
    {
        if (RouteTable.TryGetValue(CurrentRoute, out var viewBuilder))
        {
            try
            {
                var newView = viewBuilder();
                return newView;
            }
            catch (Exception e)
            {
                Logger.Error(e, "尝试生成路由对应的视图错误");
            }
        }
        return default;
    }
}
