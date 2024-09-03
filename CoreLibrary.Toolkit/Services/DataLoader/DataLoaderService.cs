using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using CoreLibrary.Toolkit.Services.DataLoader.Structs;
using Microsoft.VisualStudio.Threading;

namespace CoreLibrary.Toolkit.Services.DataLoader
{
    public sealed class DataLoaderService : IDataLoaderService
    {
        private readonly AsyncAutoResetEvent _signal = new();

        private readonly Dictionary<string, bool> _loaded = [];
        private readonly Dictionary<string, bool> _asyncLoaded = [];

        private readonly Dictionary<string, List<LoaderConfiguration>> _loaders = [];
        private readonly Dictionary<string, List<AsyncLoaderConfiguration>> _asyncLoaders = [];

        public DataLoaderService()
        {
            _signal.Set();
        }

        public void SetupLoader(string loadKey, Action loader, bool destoryAfterLoad = true)
        {
            if (_loaders.TryGetValue(loadKey, out var loaders))
            {
                loaders.Add(new(loader, destoryAfterLoad));
            }
            else
            {
                _loaders[loadKey] = [new(loader, destoryAfterLoad)];
            }
        }

        public void SetupAsyncLoader(
            string loadKey,
            Func<CancellationToken, Task> asyncLoader,
            bool destoryAfterLoad = true
        )
        {
            if (_asyncLoaders.TryGetValue(loadKey, out var loaders))
            {
                loaders.Add(new(asyncLoader, destoryAfterLoad));
            }
            else
            {
                _asyncLoaders[loadKey] = [new(asyncLoader, destoryAfterLoad)];
            }
        }

        public void StartLoad(string loadKey)
        {
            List<Task> tasks = [];

            if (_loaders.TryGetValue(loadKey, out var loaderConfigs))
            {
                foreach (var config in loaderConfigs)
                {
                    config.Loader();
                }
                loaderConfigs.RemoveAll(x => x.DestoryAfterLoad);
                _loaded[loadKey] = true;
            }
        }

        public async Task StartAsyncLoadAsync(string loadKey, CancellationToken cancellationToken = default)
        {
            try
            {
                await _signal.WaitAsync(cancellationToken);
                List<Task> tasks = [];

                if (_asyncLoaders.TryGetValue(loadKey, out var loaderConfigs))
                {
                    foreach (var config in loaderConfigs)
                    {
                        tasks.Add(config.AsyncLoader(cancellationToken));
                    }

                    await Task.WhenAll(tasks);

                    loaderConfigs.RemoveAll(x => x.DestoryAfterLoad);
                    _asyncLoaded[loadKey] = true;
                }
            }
            finally
            {
                _signal.Set();
            }
        }

        public bool IsLoaded(string loadKey) =>
            _loaded.TryGetValue(loadKey, out var loaded) && loaded
            || _asyncLoaded.TryGetValue(loadKey, out var asyncLoaded) && asyncLoaded;
    }
}
