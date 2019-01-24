using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSharpUTestDOM.TraceTest;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            App.sum(20, 50);
            Console.ReadKey();
        }
    }
}
