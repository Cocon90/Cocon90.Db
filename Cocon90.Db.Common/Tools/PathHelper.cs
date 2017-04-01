using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Db.Common.Tools
{
    public class PathHelper
    {
        public static string GetBaseDirectory()
        {
#if NETSTANDARD
            return AppContext.BaseDirectory;
#elif NETFRAMEWORK
            return AppDomain.CurrentDomain.BaseDirectory;
#endif
        }
    }
}
