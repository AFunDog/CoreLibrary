using System.Diagnostics;
using Serilog;
using Zeng.CoreLibrary.Toolkit.Extensions;
using Zeng.CoreLibrary.Toolkit.Windows.Services.SystemMonitor;

namespace CoreLibrary.Toolkit.Windows.Test;

public class SystemMonitorTest
{
    private IPerformanceMonitor PerformanceMonitor { get; }
    private ITestOutputHelper TestOutputHelper { get; }
    
    public SystemMonitorTest(ITestOutputHelper testOutputHelper, IPerformanceMonitor performanceMonitor)
    {
        PerformanceMonitor = performanceMonitor;
        TestOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public void DisposeTest()
    {
        TestOutputHelper.Write("{0}",PerformanceMonitor.MemoryUsage);
        // Debug.WriteLine("{0} Usage",PerformanceMonitor.MemoryUsage);
        TestOutputHelper.Write("Over");
    }
}