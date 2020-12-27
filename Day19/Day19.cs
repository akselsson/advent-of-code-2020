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

        [TestCase(Example, 2)]
        [TestCase(@"0: ""a""

a
b
aa", 1)]
        [TestCase(@"0: 1
1: ""a""

a
b", 1)]
        public void Example1(string input, int expected)
        {
            var matches = CountMatchingRules(input);
            Assert.AreEqual(expected, matches.Count(),string.Join(Environment.NewLine,matches));
        }
        
        [Test]
        public void Part1()
        {
            Assert.AreEqual(0,CountMatchingRules(Input).Length);
        }

        [Test]
        public void Example2()
        {
        }

        [Test]
        public void Part2()
        {
        }

        private string[] CountMatchingRules(string example)
        {
            bool IsRule(string x) => x.Contains(':');

            var lines = example.Split(Environment.NewLine);
            var grammar = new Grammar(lines
                .TakeWhile(IsRule)
                .Select(ParseLine));

            var messages = lines.SkipWhile(x => IsRule(x) || string.IsNullOrEmpty(x)).ToArray();

            return messages.Where(x => grammar.Matches(x)).ToArray();
        }

        private Rule ParseLine(string line)
        {
            Console.WriteLine("Line: " + line);
            var x = line.Substring(line.IndexOf(':') + 2);
            Console.WriteLine($"\"{x}\"");
            if (x.StartsWith('"'))
            {
                return new StringRule(x.Split("\"", StringSplitOptions.RemoveEmptyEntries).First());
            }

            return new MatchAnyRule(x.Split(" | ",StringSplitOptions.RemoveEmptyEntries).Select(y =>
                new MatchRuleIndex(y.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse))));
        }
        
    }

    abstract class Rule
    {
        public abstract string[] Matches(string message, Rule[] rules);
    }

    class StringRule : Rule
    {
        private readonly string _s;

        public StringRule(string s)
        {
            _s = s;
        }

        public override string[] Matches(string message, Rule[] rules)
        {

            return message.StartsWith(_s) ? new[] {message.Substring(1)} : new string[0];
        }
    }

    class MatchRuleIndex : Rule
    {
        private readonly int[] _indexes;

        public MatchRuleIndex(IEnumerable<int> indexes)
        {
            _indexes = indexes.ToArray();
        }

        public override string[] Matches(string message, Rule[] rules)
        {
            var result = _indexes.Aggregate(
                (IEnumerable<string>)new[]{message},
                (agg, curr) => agg.SelectMany(a=>rules[curr].Matches(a,rules)));
            return result.ToArray();
        }
    }
    
    class MatchAnyRule : Rule
    {
        private readonly IEnumerable<Rule> _rules;

        public MatchAnyRule(IEnumerable<Rule> rules)
        {
            _rules = rules.ToArray();
        }

        public override string[] Matches(string message, Rule[] rules)
        {
            return _rules.SelectMany(x => x.Matches(message, rules)).ToArray();
        }
    }

    class Grammar
    {
        private readonly Rule[] _rules;

        public Grammar(IEnumerable<Rule> rules)
        {
            _rules = rules.ToArray();
        }

        public bool Matches(string message)
        {
            //Console.WriteLine($"{message} {String.Join(",", _rules.Select(x => x?.ToString()))}");
            var matches = _rules[0].Matches(message, _rules);
            return matches.Any(x=>x.Length == 0);
        }
    }

    
}