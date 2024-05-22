using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CoreServices.Template;

public abstract class DisposableTemplate : IDisposable
{
    protected bool Disposed { get; set; }

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
            DestoryManagedResource();
        }
        DestoryUnmanagedResource();
        Disposed = true;
    }

    protected abstract void DestoryManagedResource();
    protected abstract void DestoryUnmanagedResource();

    ~DisposableTemplate() => Dispose(false);
}

