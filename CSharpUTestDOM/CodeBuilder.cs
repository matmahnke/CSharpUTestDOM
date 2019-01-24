using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUTestDOM
{
    public class CodeBuilder
    {
        private string outputPath;
        private string outputNamespace;
        private string outputClass;
        private string outputMock;
        public CodeBuilder()
        {
            outputPath = ConfigurationManager.AppSettings["output-path"];
            outputNamespace = ConfigurationManager.AppSettings["output-namespace"];
            outputClass = ConfigurationManager.AppSettings["output-class"];
            outputMock = ConfigurationManager.AppSettings["output-mock"];

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);
            if (!Directory.Exists(outputMock))
                Directory.CreateDirectory(outputMock);
        }

        public string BuildTest(InputPackage obj)
        {
            var codeCompileUnit = BuildCodeCompileUnit(obj);
            CreateMock(obj);
            var result = GenerateCode(codeCompileUnit, obj);
            return result;
        }

        private string GenerateCode(CodeCompileUnit codeCompileUnit, InputPackage inputPackage)
        {
            CodeDomProvider provider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();
            string result = string.Empty;
            using (TextWriter w = File.CreateText(outputPath + "\\" + inputPackage.Name + ".cs"))
            {
                provider.GenerateCodeFromCompileUnit(codeCompileUnit, w, new CodeGeneratorOptions());
            }
            return result;
        }

        private CodeCompileUnit BuildCodeCompileUnit(InputPackage obj)
        {
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();

            CodeNamespace @namespace = new CodeNamespace(outputNamespace);
            CodeNamespace importsNameSpace = new CodeNamespace
            {
                Imports =
                {
                    new CodeNamespaceImport("System"),
                    new CodeNamespaceImport("Microsoft.VisualStudio.TestTools.UnitTesting"),
                    new CodeNamespaceImport("Newtonsoft.Json"),
                    new CodeNamespaceImport("System.IO"),
                    new CodeNamespaceImport(obj.NameSpace)
                }
            };
            codeCompileUnit.Namespaces.Add(@namespace);
            codeCompileUnit.Namespaces.Add(importsNameSpace);

            @namespace.Types.Add(BuildDeclarationType(obj));
            return codeCompileUnit;
        }

        private CodeTypeDeclaration BuildDeclarationType(InputPackage obj)
        {
            var codeTypeDeclaration = new CodeTypeDeclaration(nameof(obj.Type));
            codeTypeDeclaration.IsClass = true;
            codeTypeDeclaration.Name = outputClass + "Test";
            codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration("TestClass"));
            codeTypeDeclaration.Members.Add(BuildEntryPointMethod(obj));
            return codeTypeDeclaration;
        }

        private void CreateMock(InputPackage obj)
        {
            if (!File.Exists(outputMock + $"\\{obj.Name}Mock.json"))
                File.Create(outputMock + $"\\{obj.Name}Mock.json");
            File.WriteAllText(outputMock + $"\\{obj.Name}Mock.json", JsonConvert.SerializeObject(obj.Return.Value));
        }

        private CodeMemberMethod BuildEntryPointMethod(InputPackage obj)
        {
            CodeMemberMethod codeMemberMethod = new CodeMemberMethod();
            codeMemberMethod.Name = obj.Name + "Test";
            codeMemberMethod.Attributes = (codeMemberMethod.Attributes & ~MemberAttributes.AccessMask) | MemberAttributes.Public;
            codeMemberMethod.CustomAttributes.Add(new CodeAttributeDeclaration("TestMethod"));
            codeMemberMethod.Statements.AddRange(BuildMethodCall(obj));
            return codeMemberMethod;
        }
        private CodeStatementCollection BuildMethodCall(InputPackage obj)
        {
            var codeExpressionStatement = new CodeStatementCollection();
            //Create result variable
            var codeVariableDeclarationStatement = new CodeVariableDeclarationStatement(typeof(object), "result", new CodeSnippetExpression("null"));
            CodeAssignStatement resultCodeAssign = new CodeAssignStatement(new CodeVariableReferenceExpression("result"),
                 //Creates the call of the method to be tested
                 new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(obj.Type), obj.Name,
                            //Create parameters
                            obj.Parameters.Select(o =>
                                BuildParameter(o.Nome, o.Type.Name, o.Value)
                            ).ToArray()
                        )
                    );

            codeExpressionStatement.Add(codeVariableDeclarationStatement);
            codeExpressionStatement.Add(resultCodeAssign);


            var codeVariableJsonResult = new CodeVariableDeclarationStatement(typeof(object), "resultJson", new CodeSnippetExpression("null"));
            codeExpressionStatement.Add(codeVariableJsonResult);
            var codeVariableMock = new CodeVariableDeclarationStatement(typeof(string), "mock", new CodeSnippetExpression("string.Empty"));
            codeExpressionStatement.Add(codeVariableMock);

            //Convert result object to json
            CodeAssignStatement resultJsonCodeAssign = new CodeAssignStatement(new CodeVariableReferenceExpression("resultJson"),
            new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression("Newtonsoft.Json.JsonConvert"), "SerializeObject",
                new CodeVariableReferenceExpression("result")));

            codeExpressionStatement.Add(resultJsonCodeAssign);

            //Get json from file
            CodeAssignStatement mockCodeAssign = new CodeAssignStatement(new CodeVariableReferenceExpression("mock"),
                new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression("File"), "ReadAllText",
                new CodePrimitiveExpression(outputMock + $"\\{obj.Name}Mock.json")));

            codeExpressionStatement.Add(mockCodeAssign);

            var desserializeStructure = new CodeMethodInvokeExpression(
            new CodeTypeReferenceExpression("Newtonsoft.Json.JsonConvert")
            , "DeserializeObject",
            new CodeVariableReferenceExpression("mock"));
            desserializeStructure.Method.TypeArguments.Add(new CodeTypeReference(typeof(string)));

            var jsonToStringValue = new CodeVariableDeclarationStatement(typeof(string), "mockObj", desserializeStructure);

            codeExpressionStatement.Add(jsonToStringValue);

            //Create assert
            CodeMethodInvokeExpression assertExpression = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression("Assert"), "AreEqual", new CodeVariableReferenceExpression("resultJson"), new CodeVariableReferenceExpression("mockObj"));

            codeExpressionStatement.Add(assertExpression);


            return codeExpressionStatement;
        }
        private CodeMethodInvokeExpression BuildParameter(string name, string type, string value)
        {
            var codeMethodInvokeExpression = new CodeMethodInvokeExpression(
            new CodeTypeReferenceExpression("Newtonsoft.Json.JsonConvert")
            , "DeserializeObject",
            new CodePrimitiveExpression(value));
            codeMethodInvokeExpression.Method.TypeArguments.Add(new CodeTypeReference(type));

            return codeMethodInvokeExpression;
        }
    }
}
