using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DerivingReadShow
{
    [Generator]
    class ReadGenerator : BaseGenerator
    {
        protected override string NameAttribute { get; set; } = "Read";

        public override void Execute(GeneratorExecutionContext context)
        {
            var codeBuilder = new StringBuilder();

            codeBuilder.AppendLine("using System;");
            codeBuilder.AppendLine("using System.Text;");
            codeBuilder.AppendLine("using System.Collections.Generic;");
            codeBuilder.AppendLine("using System.Linq;");
            codeBuilder.AppendLine("namespace TextRead");
            codeBuilder.AppendLine("{");

            #region BuildAttributeClass
            codeBuilder.AppendLine("[AttributeUsage(AttributeTargets.Class)]");
            codeBuilder.AppendLine("public class ReadAttribute : Attribute { }");
            #endregion BuildAttributeClass

            if (context.SyntaxReceiver is not SyntaxReceiver receiver) return;

            foreach (var (nameSpace, classes) in receiver.nameSpaceClasses)
            {
                var namespaceName = nameSpace.Name
                                             .ToString();

                foreach (var itClass in classes)
                {
                    var className = itClass.Identifier
                                           .ToString();

                    var fullClassName = $"{namespaceName}.{className}";

                    codeBuilder.AppendLine($"public static class Read{className}");
                    codeBuilder.AppendLine("{");
                    codeBuilder.AppendLine($"public static {fullClassName} Read(string obj)");
                    codeBuilder.AppendLine("{");
                    codeBuilder.AppendLine($"var instance = new {fullClassName}();");

                    codeBuilder.AppendLine($"var className = obj[0..{fullClassName.Length}];");
                    codeBuilder.AppendLine($"var typeName = typeof({fullClassName}).FullName;");
                    codeBuilder.AppendLine("if (className != typeName) return null;");
                    codeBuilder.AppendLine(@"obj = obj.Replace("" "", """");");
                    codeBuilder.AppendLine("var beginProp = obj.IndexOf('{') + 1;");
                    codeBuilder.AppendLine("var endProp = obj.LastIndexOf('}');");
                    codeBuilder.AppendLine("if (beginProp == -1 || endProp == -1) return null;");
                    codeBuilder.AppendLine("var props = obj[beginProp..endProp].Split(',');");
                    codeBuilder.AppendLine("var dictProps = new Dictionary<string, string>();");

                    codeBuilder.AppendLine("foreach (var prop in props)");
                    codeBuilder.AppendLine("{");
                    codeBuilder.AppendLine(@"if (!prop.Contains(""="")) return null;");
                    codeBuilder.AppendLine("var nameAndValue = prop.Split('=');");
                    codeBuilder.AppendLine("if (nameAndValue.Length != 2) return null;");
                    codeBuilder.AppendLine(@"dictProps.Add(nameAndValue.First(), nameAndValue.Last());");
                    codeBuilder.AppendLine("}");

                    var countProps = 0;

                    foreach (var member in itClass.Members)
                    {
                        if (member is PropertyDeclarationSyntax prop)
                        {
                            var name = prop.Identifier
                                           .ToString();

                            var type = prop.Type
                                           .ToString();

                            codeBuilder.AppendLine($@"if(!dictProps.TryGetValue($""{name}"", out var prop{name})) return null;");

                            if (type is "string")
                            {
                                codeBuilder.AppendLine($"instance.{name} = prop{name};");
                            }
                            else
                            {
                                codeBuilder.AppendLine($"if(!{type}.TryParse(prop{name}, out var res{name})) return null;");
                                codeBuilder.AppendLine($"instance.{name} = res{name};");
                            }

                            countProps++;
                        }
                    }

                    codeBuilder.AppendLine($"if(dictProps.Count != {countProps}) return null;");

                    codeBuilder.AppendLine("return instance;");
                    codeBuilder.AppendLine("}");
                    codeBuilder.AppendLine("}");
                }

                codeBuilder.AppendLine("}");
            }

            context.AddSource("Read.cs", codeBuilder.ToString());
        }
    }
}
