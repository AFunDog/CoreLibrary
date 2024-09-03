using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary.Core.Structs
{
    public enum ResultType
    {
        Success,
        Warning,
        Error
    }

    public readonly record struct ActionResult(ResultType ResultType, string Message)
    {
        public bool IsSucceed => ResultType == ResultType.Success;

        public static ActionResult Success(string message = "") => new(ResultType.Success, message);

        public static ActionResult Warning(string message) => new(ResultType.Warning, message);

        public static ActionResult Error(string message) => new(ResultType.Error, message);
    }
}
