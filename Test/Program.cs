using System;
using Ext;
using System.Linq;
using System.Collections.Generic;
using TextRead;

namespace Test
{
    [Read]
    [Show]
    public abstract class Plant
    {
        public string Name { get; set; }

        public bool IsTree { get; set; }
    }

    [Read]
    [Show]
    public sealed class Tree : Plant
    {
        public int Age { get; set; }
    }

    [Show]
    public sealed class Shrub : Plant
    {
        public Month MonthFlower { get; set; }
    }

    public sealed class Mushroom : Plant
    {
        public bool IsEdible { get; set; }
    }

    public enum Month
    {
        Janurary,
        February,
        March,
        April,
        May,
        June,
        Jule,
        Augast,
        September,
        October,
        November,
        December
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var treeIn = "Test.Tree {Name = bereza, IsTree = true, Age = 12}";

            var tree = ReadTree.Read(treeIn);

            Console.WriteLine(tree.Show());
        }
    }
}
