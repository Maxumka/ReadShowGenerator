using System;
using Ext;
using System.Linq;
using System.Collections.Generic;
using TextRead;

namespace Test
{
    [Show]
    public abstract class Person
    {
        public string SecondName { get; set; }
    }

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
            var treeIn = "Test.Tree {Name = bereza, Age = 12}";

            var tree = ReadTree.Read(treeIn);

            var treeInst = new Tree { Name = "berezka", Age = 12, IsTree = true };

            var shrunInst = new Shrub { Name = "kust", IsTree = false, MonthFlower = Month.April };

            Console.WriteLine(shrunInst.Show());
        }
    }
}
