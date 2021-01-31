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

            codeBuilder.AppendLine("using System;");
            codeBuilder.AppendLine("using System.Text;");
            codeBuilder.AppendLine("namespace TextShow");
            codeBuilder.AppendLine("{");

            #region BuildAttributeClass
            codeBuilder.AppendLine("[AttributeUsage(AttributeTargets.Class)]");
            codeBuilder.AppendLine("public class ShowAttribute : Attribute { }");
            #endregion BuildAttributeClass

            foreach (var (_, name, fullName, properties) in receiver.classesInfo)
            {
                codeBuilder.AppendLine($"public static class Show{name}");
                codeBuilder.AppendLine("{");
                codeBuilder.AppendLine($"public static string Show(this {fullName} obj)");
                codeBuilder.AppendLine("{");
                codeBuilder.AppendLine("var sb = new StringBuilder();");
                codeBuilder.AppendLine("var type = obj.GetType().ToString();");
                codeBuilder.AppendLine("sb.Append(type);");
                codeBuilder.AppendLine(@"sb.Append("" { "");");
                codeBuilder.AppendLine(@"var propName = "" "";");
                codeBuilder.AppendLine($"var instance = new {fullName}();");

                codeBuilder.AppendLine(BuildConvertPropertyToStringCode(properties));

                codeBuilder.AppendLine(@"sb.AppendLine(""} "");");
                codeBuilder.AppendLine("return sb.ToString();");
                codeBuilder.AppendLine("}");
                codeBuilder.AppendLine("}");
            }

            codeBuilder.AppendLine("}");

            context.AddSource("Show.cs", codeBuilder.ToString());
        }

        private string BuildConvertPropertyToStringCode(IEnumerable<PropertyDeclarationSyntax> properties)
        {
            var codeBuilder = new StringBuilder();

            foreach (var property in properties)
            {
                var nameProperty = property.Identifier
                                           .ToString();

                codeBuilder.AppendLine($@"sb.Append($""{nameProperty}"");");
                codeBuilder.AppendLine(@"sb.Append("" = "");");
                codeBuilder.AppendLine(@$"sb.Append(obj.{nameProperty});");
                if (property == properties.Last())
                {
                    codeBuilder.AppendLine(@"sb.Append("" "");");
                }
                else
                {
                    codeBuilder.AppendLine(@"sb.Append("", "");");
                }
            }

            return codeBuilder.ToString();
        }
    }
}