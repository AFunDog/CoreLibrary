using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.ExecuteChain;

// TODO MCCN.Godot 有 ProgressChain 不知道能不能合在一起

public interface IExecuteChain : IDisposable
{
    int GroupCount { get; }

    void Run();
    Task RunAsync();
}
