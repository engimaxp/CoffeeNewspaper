using System;
using System.Collections.Generic;

namespace CN_Core.Utilities
{
    public class ComparerBuilder<T>:IComparer<T> where T :new()
    {
        public static ComparerBuilder<T> Builder(Func<T, T, int> func)
        {
            return new ComparerBuilder<T>(func);
        }

        private readonly Func<T, T, int> _func;

        public ComparerBuilder(Func<T,T,int> func)
        {
            _func = func;
        }

        public int Compare(T x, T y)
        {
            if (_func == null)
                throw new System.NotImplementedException();
            else return _func(x, y);
        }
    }
}