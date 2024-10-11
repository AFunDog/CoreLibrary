using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Toolkit.ExecuteChain.EventArgs
{
    public enum ExecuteStepType
    {
        Start,
        End,
        EndWithError
    }

    public sealed record ExecuteChainEventArgs(
        int GroupIndex,
        ExecuteStepType ExecuteStepType,
        string? GroupName = null,
        string? StepName = null
    );
}
