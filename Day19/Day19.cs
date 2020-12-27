using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day19
{
    public class Day19
    {
        private const string Example =
            @"0: 4 1 5
1: 2 3 | 3 2
2: 4 4 | 5 5
3: 4 5 | 5 4
4: ""a""
5: ""b""

ababbb
bababa
abbbab
aaabbb
aaaabbb";

        private readonly string Input = File.ReadAllText("input.txt");

        [TestCase(Example,2)]
        [TestCase(@"0: ""a""

a
b",1)]
        [TestCase(@"0: 1
1: ""a""

a
b",1)]
        public void Example1(string input, int expected)
        {
            Assert.AreEqual(expected,CountMatchingRules(input));
        }

        private int CountMatchingRules(string example)
        {
            bool IsRule(string x) => x.Contains(':');

            var lines = example.Split(Environment.NewLine);
            var grammar = new Grammar(lines
                .TakeWhile(IsRule)
                .Select(ParseLine));

            var messages = lines.SkipWhile(x => IsRule(x) || string.IsNullOrEmpty(x));

            return messages.Count(x => grammar.Matches(x));

        }

        private (int position,Rule r) ParseLine(string line, int position)
        {
            Console.WriteLine("Line: " + line);
            var rules = line.Substring(line.IndexOf(':') + 1)
                .Split(' ',StringSplitOptions.RemoveEmptyEntries)
                .Select<string,Rule>(x =>
                {
                    Console.WriteLine($"\"{x}\"");
                    if (x.StartsWith('"'))
                    {
                        return new StringRule(x.Split("\"", StringSplitOptions.RemoveEmptyEntries).First());
                    }

                    return new MatchOtherRules(x.Split(' ').Select(int.Parse));

                    return null;
                });
            return (position,rules.First());
        }

        abstract class Rule
        {
            public abstract (bool match, string rest) Matches(string message, Rule[] rules);

        }

        class StringRule : Rule
        {
            private readonly string _s;

            public StringRule(string s)
            {
                _s = s;
            }

            public override (bool match, string rest) Matches(string message, Rule[] rules)
            {
                return message.StartsWith(_s) ? 
                    (true, message.Substring(1)) : 
                    (false, message);
            }
        }
        
        class MatchOtherRules : Rule
        {
            private readonly int[] _indexes;

            public MatchOtherRules(IEnumerable<int> indexes)
            {
                _indexes = indexes.ToArray();
            }

            public override (bool match, string rest) Matches(string message, Rule[] rules)
            {
                return _indexes.Aggregate(
                    (match: true, rest: message),
                    (agg, curr) => agg.match ? rules[curr].Matches(agg.rest, rules) : agg);

            }
        }
        
        class Grammar
        {
            private readonly Rule[] _rules;

            public Grammar(IEnumerable<(int position, Rule r)> rules)
            {
                _rules = rules.Select(x=>x.r).ToArray();
            }

            public bool Matches(string message)
            {
                    Console.WriteLine($"{message} {String.Join(",", _rules.Select(x=>x?.ToString()))}");
                    return _rules[0].Matches(message, _rules).match;
            }
        }

        [Test]
        public void Part1()
        {
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