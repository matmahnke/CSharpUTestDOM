using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUTestDOM
{
    [Serializable]
    public class Param
    {
        public string Nome { get; set; }
        public string Value { get; set; }
        public Type Type { get; set; }
    }
}
