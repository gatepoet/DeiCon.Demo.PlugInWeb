using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.theGecko.Utilities
{
    public interface IService
    {
        void Start();
        void Stop();
    }

    public interface ICloneable<T>
    {
        T Clone();
    }
}
