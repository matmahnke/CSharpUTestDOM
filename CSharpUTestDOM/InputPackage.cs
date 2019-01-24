using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUTestDOM
{
    [Serializable]
    public class InputPackage
    {
        public Type Type { get; set; }
        public string NameSpace { get; set; }
        public string Name { get; set; }
        public List<Param> Parameters { get; set; }
        public Param Return { get; set; }
    }
}
