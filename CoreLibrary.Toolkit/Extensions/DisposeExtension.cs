using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.Extensions;

public static class DisposeExtension
{
    /// <summary>
    /// 尝试调用该对象的 <see cref="IDisposable.Dispose"/> 方法
    /// </summary>
    /// <param name="target">目标对象</param>
    /// <returns>如果调用成功返回 true，反之返回 false</returns>
    public static bool TryDispose(this object target)
    {
        if (target is IDisposable disposable)
        {
            disposable.Dispose();
            return true;
        }
        return false;
    }
}
