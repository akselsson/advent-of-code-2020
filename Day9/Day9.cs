using System;
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
            var exception = FindException(5, Example);
            Assert.AreEqual(127,exception);
        }

        private long? FindException(int preamble, string input)
        {
            var numbers = input.Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x)).ToArray();
            for (int i = preamble; i < numbers.Length; i++)
            {
                bool match = false;
                for (int j = i-preamble; j < i; j++)
                {
                    for (int k = j+1; k < i; k++)
                    {
                        if (numbers[j] + numbers[k] == numbers[i])
                        {
                            match = true;
                        }
                    }
                }

                if (!match)
                {
                    return numbers[i];
                }
            }

            return null;

        }

        [Test]
        public void Part1()
        {
            Assert.AreEqual(1492208709,FindException(25,Input));
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