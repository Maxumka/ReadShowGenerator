using Microsoft.CodeAnalysis;
using System.Text;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace DerivingReadShow
{
    [Generator]
    public class ShowGenerator : BaseGenerator
    {
        protected override string NameAttribute { get; set; } = "Show";

        public override void Execute(GeneratorExecutionContext context)
        {
            var codeBuilder = new StringBuilder();

            if (context.SyntaxReceiver is not SyntaxReceiver receiver) return;

            foreach (var (nameSpace, classes, abstractClasses) in receiver.nameSpaceClasses)
            {
                var namespaceName = nameSpace.Name
                                             .ToString();

                codeBuilder.AppendLine("using System;");
                codeBuilder.AppendLine("using System.Text;");
                codeBuilder.AppendLine("namespace Ext");
                codeBuilder.AppendLine("{");

                #region BuildAttributeClass
                codeBuilder.AppendLine("[AttributeUsage(AttributeTargets.Class)]");
                codeBuilder.AppendLine("public class ShowAttribute : Attribute { }");
                #endregion BuildAttributeClass

                foreach (var itClass in classes)
                {
                    var className = itClass.Identifier
                                           .ToString();

                    codeBuilder.AppendLine($"public static class Show{className}");
                    codeBuilder.AppendLine("{");
                    codeBuilder.AppendLine($"public static string Show(this {namespaceName}.{className} obj)");
                    codeBuilder.AppendLine("{");
                    codeBuilder.AppendLine("var sb = new StringBuilder();");
                    codeBuilder.AppendLine("var type = obj.GetType().ToString();");
                    codeBuilder.AppendLine("sb.Append(type);");
                    codeBuilder.AppendLine(@"sb.Append("" { "");");
                    codeBuilder.AppendLine(@"var propName = "" "";");
                    codeBuilder.AppendLine($"var instance = new {namespaceName}.{className}();");

                    ClassDeclarationSyntax abstractClass = null; 

                    foreach (var c in abstractClasses)
                    {
                        var node = itClass.Identifier
                                          .Parent
                                          .DescendantNodes()
                                          .Select(n => n.NormalizeWhitespace("", ""))
                                          .Where(n => n.ToString() == c.Identifier.ToString())
                                          .FirstOrDefault();

                        if (node is not null)
                        {
                            abstractClass = c;

                            break;
                        }
                    }

                    if (abstractClass is not null)
                    {
                        foreach (var member in abstractClass.Members)
                        {
                            if (member is PropertyDeclarationSyntax prop)
                            {
                                var name = prop.Identifier
                                               .ToString();

                                codeBuilder.AppendLine($@"sb.Append($""{name}"");");
                                codeBuilder.AppendLine(@"sb.Append("" = "");");
                                codeBuilder.AppendLine(@$"sb.Append(obj.{name});");
                                codeBuilder.AppendLine(@"sb.Append("", "");");
                            }
                        }
                    }

                    foreach (var member in itClass.Members)
                    {
                        if (member is PropertyDeclarationSyntax prop)
                        {
                            var name = prop.Identifier
                                           .ToString();
                            
                            codeBuilder.AppendLine($@"sb.Append($""{name}"");");
                            codeBuilder.AppendLine(@"sb.Append("" = "");");
                            codeBuilder.AppendLine(@$"sb.Append(obj.{name});");
                            if (member == itClass.Members.Last())
                            {
                                codeBuilder.AppendLine(@"sb.Append("" "");");
                            }
                            else
                            {
                                codeBuilder.AppendLine(@"sb.Append("", "");");
                            }
                        }
                    }
                    codeBuilder.AppendLine(@"sb.AppendLine(""} "");");
                    codeBuilder.AppendLine("return sb.ToString();");
                    codeBuilder.AppendLine("}");
                    codeBuilder.AppendLine("}");
                }

                codeBuilder.AppendLine("}");
            }

            context.AddSource("Show.cs", codeBuilder.ToString());
        }
    }
}