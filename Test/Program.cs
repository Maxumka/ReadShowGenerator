using System;
using TextShow;
using TextRead;

namespace Test
{
    public abstract class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }

    [Read]
    [Show]
    public sealed class Student : Person
    {
        public int Course { get; set; }
    }

    public abstract class Plant
    {
        public string Name { get; set; }

        public int Age { get; set; }
    }

    [Read]
    [Show]
    public sealed class Tree : Plant
    {
        public bool IsTree { get; set; }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var treeText = "Test.Tree { Name = berezka, Age = 12, IsTree = true }";

            Console.WriteLine(ReadTree.Read(treeText).Show());
        }
    }
}
