using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day9
{
    public class Day9
    {
        private readonly string Example =
            @"35
20
15
25
47
40
62
55
65
95
102
117
150
182
127
219
299
277
309
576";

        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            var input = ParseInput(Example);
            var exception = FindException(5, input);
            Assert.AreEqual(127,exception);
        }

        private long? FindException(int preamble, long[] input)
        {
            for (int i = preamble; i < input.Length; i++)
            {
                bool match = false;
                for (int j = i-preamble; j < i; j++)
                {
                    for (int k = j+1; k < i; k++)
                    {
                        if (input[j] + input[k] == input[i])
                        {
                            match = true;
                        }
                    }
                }

                if (!match)
                {
                    return input[i];
                }
            }

            return null;

        }

        private static long[] ParseInput(string input)
        {
            var numbers = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x))
                .ToArray();
            return numbers;
        }

        [Test]
        public void Part1()
        {
            Assert.AreEqual(1492208709,FindException(25, ParseInput(Input)));
        }

        [Test]
        public void Example2()
        {
            Assert.AreEqual(62,FindWeakness(5, Example));
        }
        
        [Test]
        public void Part2()
        {
            Assert.AreEqual(238243506,FindWeakness(25, Input));
        }

        private long? FindWeakness(int preamble, string input)
        {
            var numbers = ParseInput(input);
            var exception = FindException(preamble, numbers);
            
            for (int i = 0; i < numbers.Length; i++)
            {
                var weakness = FindWeaknessRecursive(new[] {numbers[i]}, numbers.Skip(i + 1).ToArray(), exception.Value);
                if (weakness.HasValue)
                {
                    return weakness;
                }
            }

            return null;
        }

        private long? FindWeaknessRecursive(long[] candidates, IEnumerable<long> input, in long exception)
        {
            var sum = candidates.Sum();
            if (sum == exception)
            {
                var sorted = candidates.OrderBy(x => x).ToArray();
                return sorted.First() + sorted.Last();
            }

            if (sum > exception)
            {
                return null;
            }

            if (!input.Any())
            {
                return null;
            }

            var next = input.First();
            return FindWeaknessRecursive(candidates.Concat(new[] {next}).ToArray(), input.Skip(1), exception);
        }
    }
}