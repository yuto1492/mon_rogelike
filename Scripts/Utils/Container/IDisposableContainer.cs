using System;
using System.Collections.Generic;

namespace Container
{
    public class IDisposableContainer
    {
        private List<IDisposable> _disposables = new List<IDisposable>();
        
        public void Add(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        public void AllDispose()
        {
            _disposables.ForEach(x =>
            {
                x.Dispose();
            });
            _disposables.Clear();
        }
    }
}