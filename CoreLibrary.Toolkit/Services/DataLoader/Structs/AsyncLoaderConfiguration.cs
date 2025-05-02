namespace Zeng.CoreLibrary.Toolkit.Services.DataLoader.Structs;

internal sealed record AsyncLoaderConfiguration(Func<CancellationToken, Task> AsyncLoader, bool DestoryAfterLoad);
