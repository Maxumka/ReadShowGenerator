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

                foreach (var classDeclaration in classes)
                {
                    var className = classDeclaration.Identifier
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

                    ClassDeclarationSyntax abstractClass = GetParentClassDeclaration(classDeclaration, abstractClasses); 

                    if (abstractClass is not null)
                    {
                        var convertCodeAbstractPropertyToString = BuildConvertPropertyToStringCode(abstractClass);
                        codeBuilder.AppendLine(convertCodeAbstractPropertyToString);
                    }

                    var convertCodePropertyToString = BuildConvertPropertyToStringCode(classDeclaration);
                    codeBuilder.AppendLine(convertCodePropertyToString);

                    codeBuilder.AppendLine(@"sb.AppendLine(""} "");");
                    codeBuilder.AppendLine("return sb.ToString();");
                    codeBuilder.AppendLine("}");
                    codeBuilder.AppendLine("}");
                }

                codeBuilder.AppendLine("}");
            }

            context.AddSource("Show.cs", codeBuilder.ToString());
        }

        private string BuildConvertPropertyToStringCode(ClassDeclarationSyntax classDeclaration)
        {
            var codeBuilder = new StringBuilder();

            foreach (var member in classDeclaration.Members)
            {
                if (member is PropertyDeclarationSyntax property)
                {
                    var nameProperty = property.Identifier
                                   .ToString();

                    codeBuilder.AppendLine($@"sb.Append($""{nameProperty}"");");
                    codeBuilder.AppendLine(@"sb.Append("" = "");");
                    codeBuilder.AppendLine(@$"sb.Append(obj.{nameProperty});");
                    if (member == classDeclaration.Members.Last())
                    {
                        codeBuilder.AppendLine(@"sb.Append("" "");");
                    }
                    else
                    {
                        codeBuilder.AppendLine(@"sb.Append("", "");");
                    }
                }
            }

            return codeBuilder.ToString();
        }
    }
}