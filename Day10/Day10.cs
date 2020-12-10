using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day10
{
    public class Day10
    {
        private readonly string Example =
            @"28
33
18
42
31
14
46
20
48
47
24
23
49
45
19
38
39
11
1
32
25
35
8
17
7
9
4
2
34
10
3";

        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            var differences = CalculateDifferences(Example);
            Assert.AreEqual(22, differences.one);
            Assert.AreEqual(10, differences.three );
        }

        private (int one, int three) CalculateDifferences(string input)
        {
            var differences = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .OrderBy(x => x)
                .Aggregate((one: 0, three: 0, voltage: 0), (agg, curr) =>
                {
                    Console.WriteLine($"{curr} {agg}");
                    return (
                        agg.one + (curr - agg.voltage == 1 ? 1 : 0),
                        agg.three + (curr - agg.voltage == 3 ? 1 : 0),
                        curr);
                });
            return (differences.one,differences.three+1);
        }

        [Test]
        public void Part1()
        {
            var differences = CalculateDifferences(Input);
            Assert.AreEqual(2112,differences.one * differences.three);
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