using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Core.Contacts;

/// <summary>
/// 可释放对象
/// </summary>
public abstract class DisposableObject : IDisposable
{
    public bool Disposed { get; private set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Disposed)
            return;
        if (disposing)
        {
            DisposeManagedResource();
        }
        DisposeUnmanagedResource();
        OnDisposed();
        Disposed = true;
    }

    /// <summary>
    /// 释放托管资源
    /// </summary>
    /// <remarks>
    /// 当调用 <see cref="Dispose()"/> 即主动释放时调用
    /// </remarks>
    protected abstract void DisposeManagedResource();

    /// <summary>
    /// 释放非托管资源
    /// </summary>
    /// <remarks>
    /// 当对象被释放时调用
    /// 包括 <see cref="GC"/> 回收和主动释放
    /// </remarks>>
    protected abstract void DisposeUnmanagedResource();

    /// <summary>
    /// 当对象被释放后的触发函数
    /// </summary>
    /// <remarks>
    /// 当对象被回收时调用
    /// 包括 <see cref="GC"/> 回收和主动释放
    /// </remarks>>
    protected abstract void OnDisposed();

    ~DisposableObject() => Dispose(false);
}
