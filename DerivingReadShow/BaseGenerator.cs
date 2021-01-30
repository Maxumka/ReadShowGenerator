using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

            public List<(NamespaceDeclarationSyntax, IEnumerable<ClassDeclarationSyntax>, IEnumerable<ClassDeclarationSyntax>)> 
                nameSpaceClasses = new();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is NamespaceDeclarationSyntax nameSpace)
                {
                    var classes = nameSpace.Members
                                           .OfType<ClassDeclarationSyntax>();

                    var classesWithAttr = new List<ClassDeclarationSyntax>();

                    var abstractClasses = new List<ClassDeclarationSyntax>();

                    foreach (var itClass in classes)
                    {
                        var IsHaveShowAttr = itClass.DescendantNodes()
                                                    .OfType<AttributeSyntax>()
                                                    .Select(c => c.ToString())
                                                    .Any(c => c == nameAttribute);

                        if (IsHaveShowAttr)
                        {
                            var isAbstract = itClass.Modifiers
                                                    .Any(x => x.IsKind(SyntaxKind.AbstractKeyword));

                            if (isAbstract)
                            {
                                abstractClasses.Add(itClass);
                            }
                            else
                            {
                                classesWithAttr.Add(itClass);
                            }
                        }
                    }

                    nameSpaceClasses.Add((nameSpace, classesWithAttr, abstractClasses));
                }
            }
        }

        protected ClassDeclarationSyntax GetParentClassDeclaration(ClassDeclarationSyntax classDeclaration, 
            IEnumerable<ClassDeclarationSyntax> abstractClassesDeclaration)
        {
            foreach (var abstractClassDeclaration in abstractClassesDeclaration)
            {
                var node = classDeclaration.Identifier
                                           .Parent
                                           .DescendantNodes()
                                           .Select(n => n.NormalizeWhitespace("", ""))
                                           .Where(n => n.ToString() == abstractClassDeclaration.Identifier.ToString())
                                           .FirstOrDefault();

                if (node is not null)
                {
                    return abstractClassDeclaration;
                }
            }

            return null;
        }
    }
}
