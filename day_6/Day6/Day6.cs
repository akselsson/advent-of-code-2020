using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day6
{
    public class Day6
    {
        private readonly string Example =
            @"abc

a
b
c

ab
ac

a
a
a
a

b";

        private string Input = File.ReadAllText("input.txt");
        
        [Test]
        public void Example1()
        {
            var counts = CountDistinctAnswers(Example);
            Assert.AreEqual(11, counts.Sum());
        }

        [Test]
        public void Assignment1()
        {
            Assert.AreEqual(6551,CountDistinctAnswers(Input).Sum());
        }

        private IEnumerable<int> CountDistinctAnswers(string example)
        {
            var groups = example.Split(Environment.NewLine + Environment.NewLine);
            var counts = groups.Select(x => x.Replace(Environment.NewLine, "").Distinct().Count());
            return counts;
        }
    }
}