using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Day14
{
    public class Day14
    {
        private readonly string Example =
            @"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0";

        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            var enumerable = Run(Example);
            Console.Write(string.Join(Environment.NewLine,enumerable.Select(x=>Convert.ToString(x,2))));
            Assert.AreEqual(165,enumerable.Sum());
        }
        
        [Test]
        public void Example1_2()
        {
            var enumerable = Run(@"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11");
            Console.Write(string.Join(Environment.NewLine,enumerable.Select(x=>Convert.ToString(x,2))));
            Assert.AreEqual(73,enumerable.Sum());
        }
        
        [Test]
        public void Part1()
        {
            Assert.AreEqual(15172047086292,Run(Input).Sum());
        }

        private IEnumerable<long> Run(string input)
        {
            return input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(
                    (mask: (or: 0L, and: 0L), memory: new List<long>()),
                    (agg, curr) =>
                    {
                        if (curr.StartsWith("mask ="))
                        {
                            return (ParseMask(curr), agg.memory);
                        }

                        var memory = ParseMemory(curr);
                        if (memory.index >= agg.memory.Count)
                        {
                            agg.memory.AddRange(new long[memory.index + 1 - agg.memory.Count ]);
                        }

                        agg.memory[memory.index] = (memory.value | agg.mask.or) & agg.mask.and;
                        return agg;
                    }
                ).memory;

        }

        private (int index, long value) ParseMemory(string input)
        {
            var pattern = new Regex(@"mem\[(\d+)\] = (\d+)");
            var match = pattern.Match(input);
            return (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
        }

        private (long or, long and) ParseMask(string input)
        {
            var pattern = new Regex(@"mask = ([X01]+)$");
            var match = pattern.Match(input);
            var mask = match.Groups[1].Value;
            var or = mask.Replace("X", "0");
            var and = mask.Replace("X", "1");
            return (Convert.ToInt64(or,2),Convert.ToInt64(and,2));
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