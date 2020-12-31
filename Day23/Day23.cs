using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Assert.AreEqual("92658374",CalculatePart1Answer(Example,10));
            Assert.AreEqual("67384529",CalculatePart1Answer(Example,100));
        }
        
        [Test]
        public void Part1()
        {
            Assert.AreEqual("58427369",CalculatePart1Answer(Input,100));
        }
        
        [Test]
        public void Example2()
        {
            Assert.AreEqual(149245887792,CalculatePart2Answer(Example,10_000_000,1_000_000));
        }
        
        [Test]
        public void Part2()
        {
            Assert.AreEqual(111057672960,CalculatePart2Answer(Input,10_000_000,1_000_000));
        }

        private string CalculatePart1Answer(string input, int iterations)
        {
            var list = DoSimulate(input, iterations, 0);

            var asList = list.ToList();
            var index = asList.IndexOf(1);
            return string.Join("",list.Skip(index+1).Concat(list.Take(index)).Select(x=>x.ToString()));

        }
        
        private long CalculatePart2Answer(string input, int iterations, int ensureLength = 0)
        {
            var list = DoSimulate(input, iterations, ensureLength);

            var one = list.Find(1);
            var next = one.Next;
            var nextNext = next.Next;
            var answer = (long)next.Value * nextNext.Value;

            Console.WriteLine($"{next.Value} * {nextNext.Value} = {answer}");
            return answer;
        }

        private static LinkedList<int> DoSimulate(string input, int iterations, int ensureLength)
        {
            LinkedList<int> list = new LinkedList<int>();
            var nodes = new Dictionary<int, LinkedListNode<int>>(ensureLength);

            foreach (var character in input)
            {
                var node = list.AddLast(int.Parse(new String(character, 1)));
                nodes[node.Value] = node;
            }
            var max = list.Max();
            var min = list.Min();
            foreach (var extra in Enumerable.Range(max+1, Math.Max(ensureLength - list.Count, 0)))
            {
                var node = list.AddLast(extra);
                nodes[node.Value] = node;
                max = extra;
            }
            
            var current = list.First;
            int iteration = 0;
            while (iteration++ < iterations)
            {
                var remove = Enumerable.Range(0, 3).Select(x =>
                {
                    var next = current.Next ?? list.First;
                    list.Remove(next);
                    nodes.Remove(next.Value);
                    return next.Value;
                }).ToArray();

                var destination = current.Value - 1 < min ? max : current.Value - 1;
                while (remove.Contains(destination))
                {
                    destination = (destination - 1 < min ? max : destination - 1);
                }

                var destinationNode = nodes[destination];

                foreach (var toAdd in remove.Reverse())
                {
                    var node = list.AddAfter(destinationNode, toAdd);
                    nodes[node.Value] = node;
                }

                current = current.Next ?? list.First;
                //Console.WriteLine(current.Value + ": " + string.Join(" ",list));
            }

            return list;
        }

    }
}