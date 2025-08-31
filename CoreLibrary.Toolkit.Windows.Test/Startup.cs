using Microsoft.Extensions.DependencyInjection;
using Zeng.CoreLibrary.Toolkit.Windows.Extensions;

namespace CoreLibrary.Toolkit.Windows.Test;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.UseSystemMonitor();
    }
}