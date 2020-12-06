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
        public void Part1()
        {
            Assert.AreEqual(6551,CountDistinctAnswers(Input).Sum());
        }

        [Test]
        public void Example2()
        {
            var counts = CountSharedAnswers(Example);
            Assert.AreEqual(6,counts.Sum());
        }
        
        [Test]
        public void Part2()
        {
            Assert.AreEqual(3358,CountSharedAnswers(Input).Sum());
        }

        private IEnumerable<int> CountSharedAnswers(string example)
        {
            var groups = example.Split(Environment.NewLine + Environment.NewLine);
            return groups.Select(x =>
            {
                var answers = x.Replace(Environment.NewLine,"").Distinct().ToArray();
                var individuals = x.Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries).ToArray();
                return answers.Count(ch => individuals.All(y => y.Contains(ch)));
            });
        }

        private IEnumerable<int> CountDistinctAnswers(string example)
        {
            var groups = example.Split(Environment.NewLine + Environment.NewLine);
            return groups.Select(x => x.Replace(Environment.NewLine, "").Distinct().Count());
        }
    }
}