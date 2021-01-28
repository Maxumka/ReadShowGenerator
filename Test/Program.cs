using System;
using Ext;
using System.Linq;
using System.Collections.Generic;
using TextRead;

namespace Test
{
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
       
        public Hobby PersonHobby { get; set; }
    }

    public enum Hobby 
    {
        Music,
        Games,
    };

    public class Program
    {
        static void Main(string[] args)
        {
            var personStr = "Test.Person { Name = Max, Age = 21, PersonHobby = Music }";

            var a = ReadPerson.Read(personStr);

            Console.WriteLine(a.Show());
        }
    }
}
