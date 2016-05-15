using System;
using System.Collections.Generic;
using System.Linq;

namespace ProxyDashboard
{
    public interface IProxySetter
    {
        void SetProxy(string ip);
    }
}
