using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

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

            foreach (var (nameSpace, classes, abstractClasses) in receiver.nameSpaceClasses)
            {
                var namespaceName = nameSpace.Name
                                             .ToString();

                foreach (var classDeclaration in classes)
                {
                    var className = classDeclaration.Identifier
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

                    var countAllProperty = 0;

                    var abstractClass = GetParentClassDeclaration(classDeclaration, abstractClasses);

                    if (abstractClass is not null)
                    {
                        var (parseCodeAbstractProperty, countAbstractProperty) = BuildPropertyParseCode(abstractClass, context, namespaceName);
                        codeBuilder.AppendLine(parseCodeAbstractProperty);
                        countAllProperty += countAbstractProperty;
                    }

                    var (parseCode, countProperty) = BuildPropertyParseCode(classDeclaration, context, namespaceName);
                    codeBuilder.AppendLine(parseCode);
                    countAllProperty += countProperty;

                    codeBuilder.AppendLine($"if(dictProps.Count != {countAllProperty}) return null;");

                    codeBuilder.AppendLine("return instance;");
                    codeBuilder.AppendLine("}");
                    codeBuilder.AppendLine("}");
                }

                codeBuilder.AppendLine("}");
            }

            context.AddSource("Read.cs", codeBuilder.ToString());
        }

        private (string parseCode, int countProperty) BuildPropertyParseCode(ClassDeclarationSyntax classDeclaration, 
            GeneratorExecutionContext context, string nameSpaceName)
        {
            var codeBuilder = new StringBuilder();
            var countProperty = 0;

            foreach (var member in classDeclaration.Members)
            {
                if (member is PropertyDeclarationSyntax property)
                {
                    var nameProperty = property.Identifier;
                    var typeProperty = property.Type;
                    var propertySymbol = (IPropertySymbol)context.Compilation
                                                                 .GetSemanticModel(classDeclaration.SyntaxTree)
                                                                 .GetDeclaredSymbol(property);

                    codeBuilder.AppendLine($@"if(!dictProps.TryGetValue($""{nameProperty}"", out var prop{nameProperty})) return null;");

                    if (propertySymbol.Type.TypeKind is TypeKind.Enum)
                    {
                        codeBuilder.AppendLine($@"if(!Enum.TryParse<{nameSpaceName}.{typeProperty}>
                                              (prop{nameProperty}, out var res{nameProperty})) return null;");

                        codeBuilder.AppendLine($"instance.{nameProperty} = res{nameProperty};");
                    }
                    else if (typeProperty.ToString() is "string")
                    {
                        codeBuilder.AppendLine($"instance.{nameProperty} = prop{nameProperty};");
                    }
                    else
                    {
                        codeBuilder.AppendLine($"if(!{typeProperty}.TryParse(prop{nameProperty}, out var res{nameProperty})) return null;");
                        codeBuilder.AppendLine($"instance.{nameProperty} = res{nameProperty};");
                    }

                    countProperty++;
                }
            }

            return (codeBuilder.ToString(), countProperty);
        }
    } 
}
