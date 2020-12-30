using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day23
{
    public class Day23
    {
        private readonly string Example =
            @"389125467";

        private readonly string Input = "157623984";

        [Test]
        public void Example1()
        {
            Assert.AreEqual("92658374",Simulate(Example,10));
            Assert.AreEqual("67384529",Simulate(Example,100));
        }
        
        [Test]
        public void Part1()
        {
            Assert.AreEqual("58427369",Simulate(Input,100));

        }

        private string Simulate(string input, int iterations)
        {
            LinkedList<byte> list = new LinkedList<byte>(input.Select(x => byte.Parse(new String(x,1))));
            var max = list.Max();
            var min = list.Min();
            var current = list.First;
            int iteration = 0;
            while (iteration++ < iterations)
            {
                var remove = Enumerable.Range(0, 3).Select(x =>
                {
                    var next = current.Next ?? list.First;
                    list.Remove(next);
                    return next.Value;
                }).ToArray();

                var destination = (byte)(current.Value - 1 < min ? max : current.Value - 1);
                while(remove.Contains(destination))
                {
                    destination = (byte)(destination - 1 < min ? max : destination - 1);
                }

                var destinationNode = list.Find(destination);

                foreach (var toAdd in remove.Reverse())
                {
                    list.AddAfter(destinationNode, toAdd);
                }

                current = current.Next ?? list.First;   
                //Console.WriteLine(current.Value + ": " + string.Join(" ",list));
            }

            var asList = list.ToList();
            var index = asList.IndexOf(1);
            return string.Join("",list.Skip(index+1).Concat(list.Take(index)).Select(x=>x.ToString()));

        }

        

        [Test]
        public void Example2()
        {
        }

        [Test]
        public void Part2()
        {
        }
    }
}