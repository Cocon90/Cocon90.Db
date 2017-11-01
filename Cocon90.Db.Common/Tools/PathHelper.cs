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
            return AppContext.BaseDirectory;
        }
    }
}
