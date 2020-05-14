using System;
using System.Collections.Generic;
using System.Text;

namespace MovieBuffLib
{
    public interface ILocalDataProvider
    {
        void Set(string key, string value);
        string Get(string key);
        void Reset();
    }
}
