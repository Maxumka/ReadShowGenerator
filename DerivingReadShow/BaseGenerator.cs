using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DerivingReadShow
{
    [Generator]
    public abstract class BaseGenerator : ISourceGenerator
    {
        protected abstract string NameAttribute { get; set; }

        public abstract void Execute(GeneratorExecutionContext context);

        public void Initialize(GeneratorInitializationContext context) => context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(this));

        protected class SyntaxReceiver : ISyntaxReceiver
        {
            private readonly string nameAttribute;

            public SyntaxReceiver(BaseGenerator generator)
            {
                nameAttribute = generator.NameAttribute;
            }

            public List<(NamespaceDeclarationSyntax, IEnumerable<ClassDeclarationSyntax>)> nameSpaceClasses = new();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is NamespaceDeclarationSyntax nameSpace)
                {
                    var classes = nameSpace.Members
                                           .OfType<ClassDeclarationSyntax>();

                    var classesWithShowAttr = new List<ClassDeclarationSyntax>();

                    foreach (var itClass in classes)
                    {
                        var IsHaveShowAttr = itClass.DescendantNodes()
                                                    .OfType<AttributeSyntax>()
                                                    .Select(c => c.ToString())
                                                    .Any(c => c == nameAttribute);

                        if (IsHaveShowAttr)
                        {
                            classesWithShowAttr.Add(itClass);
                        }
                    }

                    nameSpaceClasses.Add((nameSpace, classesWithShowAttr));
                }
            }
        }
    }
}
