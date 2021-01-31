using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReadShowGenerator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DerivingReadShow
{
    [Generator]
    public abstract class BaseGenerator : ISourceGenerator
    {
        protected abstract string NameAttribute { get; set; }

        public abstract void Execute(GeneratorExecutionContext context);

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(this));
        }

        protected class SyntaxReceiver : ISyntaxReceiver
        {
            private readonly string nameAttribute;

            public SyntaxReceiver(BaseGenerator generator)
            {
                nameAttribute = generator.NameAttribute;
            }

            public List<ClassInfo> classesInfo = new();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is NamespaceDeclarationSyntax nameSpace)
                {
                    var classesDeclaration = nameSpace.Members
                                                      .OfType<ClassDeclarationSyntax>();

                    foreach (var classDeclaration in classesDeclaration)
                    {
                        var IsHaveAttribute = classDeclaration.DescendantNodes()
                                                              .OfType<AttributeSyntax>()
                                                              .Any(c => c.ToString() == nameAttribute);

                        if (!IsHaveAttribute) continue;

                        var propertiesDeclaration = new List<PropertyDeclarationSyntax>();

                        var maybeParentClass = GetParentClass(nameSpace, classDeclaration);

                        if (maybeParentClass is not null)
                        {
                            propertiesDeclaration.AddRange(GetProperties(maybeParentClass));
                        }

                        propertiesDeclaration.AddRange(GetProperties(classDeclaration));

                        var classInfo = new ClassInfo
                        {
                            NameSpace = nameSpace.Name.ToString(),
                            Name = classDeclaration.Identifier.ValueText,
                            Properties = propertiesDeclaration
                        };

                        classesInfo.Add(classInfo);
                    }                    
                }
            }

            private ClassDeclarationSyntax GetParentClass(NamespaceDeclarationSyntax namespaceDeclaration, ClassDeclarationSyntax classDeclaration)
            {
                var maybeBaseType = classDeclaration.DescendantNodes()
                                                    .Where(c => c.Kind() == SyntaxKind.SimpleBaseType)
                                                    .FirstOrDefault();

                if (maybeBaseType is not SimpleBaseTypeSyntax simpleBaseType) return null;

                var maybeParentClass = namespaceDeclaration.DescendantNodes()
                                                           .OfType<ClassDeclarationSyntax>()
                                                           .Where(n => n.Identifier.ValueText == maybeBaseType.ToString())
                                                           .FirstOrDefault();

                return maybeParentClass;
            }

            private IEnumerable<PropertyDeclarationSyntax> GetProperties(ClassDeclarationSyntax classDeclaration)
                => classDeclaration.Members.OfType<PropertyDeclarationSyntax>(); 
        }
    }
}
