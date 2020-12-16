using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Day16
{
    public class Day16
    {
        private readonly string Example =
            @"class: 1-3 or 5-7
row: 6-11 or 33-44
seat: 13-40 or 45-50

your ticket:
7,1,14

nearby tickets:
7,3,47
40,4,50
55,2,20
38,6,12";

        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            var nearbyScanningErrors = NearbyScanningErrors(Example);
            CollectionAssert.AreEquivalent(new[]{4,55,12},nearbyScanningErrors);
            Assert.AreEqual(71, nearbyScanningErrors.Sum());
        }

        [Test]
        public void Example1_Parse()
        {
            var note = Note.Parse(Example);
            Assert.AreEqual(3,note.Rules.Length);
            Assert.AreEqual("class",note.Rules[0].Name);
            Assert.AreEqual((1,3),note.Rules[0].Conditions[0]);
            
            CollectionAssert.AreEqual(new[]{7,1,14},note.Ticket);
            
            CollectionAssert.AreEqual(new[]{7,3,47},note.NearbyTickets[0]);
        }

        private IEnumerable<int> NearbyScanningErrors(string input)
        {
            var note = Note.Parse(input);
            return note.NearbyTickets.SelectMany(x =>
                x.Where(y => !note.Rules.Any(rule => rule.Conditions.Any(c => c.Min <= y && c.Max >= y))));
        }

        record Note(
            (string Name, (int Min, int Max)[] Conditions)[] Rules, 
            int[] Ticket,
            int[][] NearbyTickets)
        {
            public static Note Parse(string input)
            {
                var sections = input.Split(Environment.NewLine + Environment.NewLine,
                    StringSplitOptions.RemoveEmptyEntries);
                Regex rule = new Regex(@"(?<field>\w+): (?<min1>\d+)-(?<max1>\d+) or (?<min2>\d+)-(?<max2>\d+)");
                var rules = sections[0].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => rule.Match(line))
                    .Select(match => (
                        name: match.Groups["field"].Value,
                        conditions: new[]
                        {
                            (min: int.Parse(match.Groups["min1"].Value), max: int.Parse(match.Groups["max1"].Value)),
                            (min: int.Parse(match.Groups["min2"].Value), max: int.Parse(match.Groups["max2"].Value))
                        })).ToArray();

                var ticket = sections[1].Split(Environment.NewLine)[1].Split(",").Select(int.Parse).ToArray();

                var nearbyTickets = sections[2].Split(Environment.NewLine).Skip(1).Select(line =>
                        line.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
                    .ToArray();

                return new Note(Rules: rules, Ticket: ticket, NearbyTickets: nearbyTickets);
            }
        }

        [Test]
        public void Part1()
        {
            var nearbyScanningErrors = NearbyScanningErrors(Input);
            Assert.AreEqual(29878, nearbyScanningErrors.Sum());
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