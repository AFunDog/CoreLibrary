using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Core.Contacts;

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

    /// <summary>
    /// 释放托管资源
    /// </summary>
    /// <remarks>
    /// 当调用 <see cref="DisposeAsync()"/> 即主动释放时调用
    /// </remarks>
    protected abstract ValueTask DisposeManagedResourceAsync();

    /// <summary>
    /// 释放非托管资源
    /// </summary>
    /// <remarks>
    /// 当对象被释放时调用
    /// 包括 <see cref="GC"/> 回收和主动释放
    /// </remarks>>
    protected abstract ValueTask DisposeUnmanagedResourceAsync();

    /// <summary>
    /// 当对象被释放后的触发函数
    /// </summary>
    /// <remarks>
    /// 当对象被回收时调用
    /// 包括 <see cref="GC"/> 回收和主动释放
    /// </remarks>>
    protected abstract ValueTask OnDisposedAsync();
}
