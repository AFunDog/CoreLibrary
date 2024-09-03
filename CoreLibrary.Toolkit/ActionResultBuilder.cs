using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreLibrary.Core.Structs;

namespace CoreLibrary.Toolkit
{
    public class ActionResultBuilder : IDisposable
    {
        private readonly Action<ActionResult, string>? _createAction;
        private readonly Action<ActionResult, string>? _finalAction;

        private ActionResult _lastResult;
        private string _extendMessage = "";

        public ActionResultBuilder(
            Action<ActionResult, string>? createAction = null,
            Action<ActionResult, string>? finalAction = null
        )
        {
            _createAction = createAction;
            _finalAction = finalAction;
        }

        public virtual ActionResult Success(string message = "", string extendMessage = "")
        {
            _lastResult = ActionResult.Success(message);
            _extendMessage = extendMessage;
            _createAction?.Invoke(_lastResult, extendMessage);
            return _lastResult;
        }

        public virtual ActionResult Warning(string message, string extendMessage = "")
        {
            _lastResult = ActionResult.Warning(message);
            _extendMessage = extendMessage;
            _createAction?.Invoke(_lastResult, extendMessage);
            return _lastResult;
        }

        public virtual ActionResult Error(string message, string extendMessage = "")
        {
            _lastResult = ActionResult.Error(message);
            _extendMessage = extendMessage;
            _createAction?.Invoke(_lastResult, extendMessage);
            return _lastResult;
        }

        public void Dispose()
        {
            _finalAction?.Invoke(_lastResult, _extendMessage);
        }
    }
}
