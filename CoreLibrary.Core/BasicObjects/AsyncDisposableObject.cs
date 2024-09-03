using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Core.BasicObjects
{
    public abstract class AsyncDisposableObject : IAsyncDisposable
    {
        public bool Disposed { get; protected set; }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (Disposed)
                return;
            if (disposing)
            {
                await DisposeManagedResourceAsync();
            }
            await DisposeUnmanagedResourceAsync();
            await OnDisposedAsync();
            Disposed = true;
        }

        protected abstract ValueTask DisposeManagedResourceAsync();
        protected abstract ValueTask DisposeUnmanagedResourceAsync();
        protected abstract ValueTask OnDisposedAsync();
    }
}
