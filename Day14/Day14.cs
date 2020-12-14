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
            Console.Write(string.Join(Environment.NewLine, enumerable.Select(x => Convert.ToString(x, 2))));
            Assert.AreEqual(165, enumerable.Sum());
        }

        [Test]
        public void Example1_2()
        {
            var enumerable = Run(@"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11");
            Console.Write(string.Join(Environment.NewLine, enumerable.Select(x => Convert.ToString(x, 2))));
            Assert.AreEqual(73, enumerable.Sum());
        }

        [Test]
        public void Part1()
        {
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
                            agg.memory.AddRange(new long[memory.index + 1 - agg.memory.Count]);
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
            return (Convert.ToInt64(or, 2), Convert.ToInt64(and, 2));
        }


        [Test]
        public void Example2()
        {
            var enumerable = Run2(@"mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1");
            Console.Write(string.Join(Environment.NewLine, enumerable.Select(x =>
                $"{x.address}{Convert.ToString(x.value, 2)}")));
            Assert.AreEqual(208, enumerable.Sum(x=>x.value));
        }
        
        [Test]
        public void Example2_2()
        {
            var enumerable = Run2(@"mask = 000000000000000000000000000000X1001X
mem[42] = 100");
            Console.Write(string.Join(Environment.NewLine, enumerable.Select(x =>
                $"{x.address}{Convert.ToString(x.value, 2)}")));
            Assert.AreEqual(400, enumerable.Sum(x=>x.value));
            CollectionAssert.AreEquivalent(new[]{26,27,58,59},enumerable.Select(x=>x.address));
        }

        [Test]
        public void Test_Combinations()
        {
            Assert.AreEqual(8, Combinations("XXX").Count());
            CollectionAssert.AreEquivalent(new[]{0,1,2,3,4,5,6,7}, Combinations("XXX"));
            CollectionAssert.AreEquivalent(new[]{0,1,4,5}, Combinations("X0X"));
        }

        private IEnumerable<long> Combinations(string input)
        {
            return Combinations(input.ToCharArray());
        }
        private IEnumerable<long> Combinations(char[] input)
        {

            var x = Array.IndexOf(input,'X');
            if (x == -1)
            {
                yield return Convert.ToInt64(new string(input), 2);
                yield break;
            }
            foreach(var candidate in new[]{'1','0'})
            {
                var oldValue = input[x];
                input[x] = candidate;
                foreach (var combination in Combinations(input))
                {
                    yield return combination;
                }

                input[x] = oldValue;
            }
        }

        [Test]
        public void Part2()
        {
            var enumerable = Run2(Input);
            Console.Write(string.Join(Environment.NewLine, enumerable.Select(x =>
                $"{x.address}{Convert.ToString(x.value, 2)}")));
            Assert.AreEqual(4197941339968, enumerable.Sum(x=>x.value));
        }

        private IEnumerable<(long address,long value)> Run2(string input)
        {
            var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            var memory = new Dictionary<long, long>();
            (long and,long or,long[] floats) masks = (0,0,new long[0]);
            foreach (var line in lines)
            {
                if (line.StartsWith("mask ="))
                {
                    masks = ParseMask2(line);
                    continue;
                }

                var currentMemory = ParseMemory(line);
                var index = (currentMemory.index | masks.or) & masks.and;
                    
                foreach (var mask in masks.floats)
                {
                    memory[index | mask] = currentMemory.value;
                }
            }

            return memory.Select(x=>(x.Key,x.Value));
        }

        private (long and,long or,long[] floats) ParseMask2(string input)
        {
            var pattern = new Regex(@"mask = ([X01]+)$");
            var match = pattern.Match(input);
            var mask = match.Groups[1].Value;
            var and = Convert.ToInt64(mask.Replace("0","1").Replace("X", "0"),2);
            var or = Convert.ToInt64(mask.Replace("X", "1"),2);
            return (and,or,Combinations(mask.Replace("1","0")).ToArray());
        }
    }
}