using Newtonsoft.Json;
using PostSharp.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUTestDOM
{
    public static class TraceTest
    {
        [Serializable]
        public sealed class TraceAttribute : OnMethodBoundaryAspect
        {
            private readonly string category;
            InputPackage p = new InputPackage();

            public TraceAttribute(string category)
            {
                this.category = category;
            }

            public string Category { get { return category; } }

            public override void OnEntry(MethodExecutionArgs args)
            {
                p.NameSpace = args.Method.DeclaringType.Namespace;
                p.Name = args.Method.Name;
                p.Type = args.Method.DeclaringType;
                p.Parameters = new List<Param>();
                for (int x = 0; x < args.Arguments.Count; x++)
                {
                    p.Parameters.Add(new Param()
                    {
                        Nome = args.Method.GetParameters()[x].Name,
                        Type = args.Method.GetParameters()[x].ParameterType,
                        Value = JsonConvert.SerializeObject(args.Arguments.GetArgument(x))
                    });
                }
            }

            public override void OnExit(MethodExecutionArgs args)
            {
                p.Return = new Param() { Nome = args.Method.DeclaringType.Name, Type = args.Method.DeclaringType, Value = JsonConvert.SerializeObject(args.ReturnValue) };
                var codeBuilder = new CodeBuilder();
                string result = codeBuilder.BuildTest(p);
            }
        }
    }
}
