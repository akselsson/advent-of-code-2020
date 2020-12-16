using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
            return note.GetScanningErrors();
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

                var nearbyTickets = sections[2].Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(line =>
                        line.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray())
                    .ToArray();

                return new Note(Rules: rules, Ticket: ticket, NearbyTickets: nearbyTickets);
            }

            public IEnumerable<(int value,string field)> GetTicket()
            {
                var tickets = NearbyTickets.Where(x => x.All(y => Rules.Any(rule => Match(rule.Conditions, y)))).ToArray();
                Console.WriteLine($"Candidates: {string.Join(Environment.NewLine,tickets.Select(c=>string.Join(",",c)))}");
                var combinations = GetRuleCombination(Rules,tickets,0);
                return combinations.Select(x => (Ticket[x.Index], x.Name));
            }

            public (string Name, (int Min, int Max)[] Conditions, int Index)[] GetRuleCombination(
                (string Name, (int Min, int Max)[] Conditions)[] rules, int[][] tickets, int index)
            {
                if (rules.Length == 0)
                {
                    return Array.Empty<(string, (int, int)[], int)>();
                }
                foreach (var rule in rules)
                {
                    if (!tickets.Select(x => x[index]).All(ticket => Match(rule.Conditions, ticket)))
                    {
                        continue;
                    }

                    var combo = GetRuleCombination(
                        rules.Where(x => x.Name != rule.Name).ToArray(),
                        tickets,
                        index + 1);
                    if (combo != null)
                    {
                        return new[] {(rule.Name, rule.Conditions, index)}.Concat(combo).ToArray();
                    }
                }
                return null;
            }

            
            
            private static bool Match((int Min, int Max)[] conditions, int ticket)
            {
                return conditions.Any(c => c.Min <= ticket && c.Max >= ticket);
            }

            public IEnumerable<int> GetScanningErrors()
            {
                return NearbyTickets.SelectMany(x =>
                    x.Where(y => !Rules.Any(rule => Match(rule.Conditions,y))));
            }
        }

        [Test]
        public void Part1()
        {
            var nearbyScanningErrors = NearbyScanningErrors(Input);
            Assert.AreEqual(29878, nearbyScanningErrors.Sum());
        }

        private const string Example_2 = @"class: 0-1 or 4-19
row: 0-5 or 8-19
seat: 0-13 or 16-19

your ticket:
11,12,13

nearby tickets:
3,9,18
15,1,5
5,14,9";
        [Test]
        public void Example2()
        {
            var note = Note.Parse(Example_2);
            CollectionAssert.AreEquivalent(new[]{(11,"row"),(12,"class"),(13,"seat")},note.GetTicket());
        }


        [Test]
        public void Part2()
        {
            var note = Note.Parse(Input);
            var ticket = note.GetTicket();
            Assert.AreEqual(0,ticket.Where(x=>x.field.StartsWith("departure")).Aggregate(1,(agg,curr) => agg*curr.value));
        }
    }
}