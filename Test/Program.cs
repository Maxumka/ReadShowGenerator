using System;
using Ext;
using System.Linq;
using System.Collections.Generic;
using TextRead;

namespace Test
{
    [Read]
    [Show]
    public class Tree
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public bool IsNormal { get; set; }
    }

    [Read]
    [Show]
    public class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }

    public class Program
    {
        static void Main(string[] args)
        {         
            var treeStr = "Test.Tree { Name = березка, Age = 12, IsNormal = True}";

            var tree = ReadTree.Read(treeStr);

            Console.WriteLine(tree.Show());

            var personStr = "Test.Person { Name = Max, Age = 12 }";

            var person = ReadPerson.Read(personStr);

            Console.WriteLine(person.Show());
        }
    }
}
