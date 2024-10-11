using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.ExecuteChain
{
    public interface IExecuteChain : IDisposable
    {
        int GroupCount { get; }

        void Run();
        Task RunAsync();
    }
}
