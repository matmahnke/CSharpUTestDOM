using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSharpUTestDOM.TraceTest;

namespace ConsoleTest
{
    public static class App
    {
        [Trace("Debug")]
        public static decimal sum(decimal x, decimal y)
        {
            return x + y;
        }
    }

}
