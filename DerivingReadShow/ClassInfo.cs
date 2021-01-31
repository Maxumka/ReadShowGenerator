using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReadShowGenerator
{
    public sealed class ClassInfo
    {
        public string NameSpace { get; set; }

        public string Name { get; set; }

        public string FullName => $"{NameSpace}.{Name}";

        public IEnumerable<PropertyDeclarationSyntax> Properties { get; set; }

        public void Deconstruct(out string nameSpace, out string name, out string fullName, out IEnumerable<PropertyDeclarationSyntax> properties)
        {
            nameSpace = NameSpace;
            name = Name;
            fullName = FullName;
            properties = Properties;
        }
    }
}
