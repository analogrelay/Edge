using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VibrantUtils;

namespace Edge
{
    internal class DisposableAction : IDisposable
    {
        private Action _act;

        public DisposableAction(Action act)
        {
            Requires.NotNull(act, "act");

            _act = act;
        }

        public void Dispose()
        {
            _act();
        }
    }
}
