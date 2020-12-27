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
            var (grammar, messages) = ParseInput(input);
            var matches = grammar.MatchAll(messages);
            Assert.AreEqual(expected, matches.Count(),string.Join(Environment.NewLine,matches));
        }
        
        [Test]
        public void Part1()
        {
            var (grammar, messages) = ParseInput(Input);
            Assert.AreEqual(182,grammar.MatchAll(messages).Length);
        }

        [TestCase(@"42: 9 14 | 10 1
9: 14 27 | 1 26
10: 23 14 | 28 1
1: ""a""
11: 42 31
5: 1 14 | 15 1
19: 14 1 | 14 14
12: 24 14 | 19 1
16: 15 1 | 14 14
31: 14 17 | 1 13
6: 14 14 | 1 14
2: 1 24 | 14 4
0: 8 11
13: 14 3 | 1 12
15: 1 | 14
17: 14 2 | 1 7
23: 25 1 | 22 14
28: 16 1
4: 1 1
20: 14 14 | 1 15
3: 5 14 | 16 1
27: 1 6 | 14 18
14: ""b""
21: 14 1 | 1 14
25: 1 1 | 1 14
22: 14 14
8: 42
26: 14 22 | 1 20
18: 15 15
7: 14 5 | 1 21
24: 14 1

abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa
bbabbbbaabaabba
babbbbaabbbbbabbbbbbaabaaabaaa
aaabbbbbbaaaabaababaabababbabaaabbababababaaa
bbbbbbbaaaabbbbaaabbabaaa
bbbababbbbaaaaaaaabbababaaababaabab
ababaaaaaabaaab
ababaaaaabbbaba
baabbaaaabbaaaababbaababb
abbbbabbbbaaaababbbbbbaaaababb
aaaaabbaabaaaaababaa
aaaabbaaaabbaaa
aaaabbaabbaaaaaaabbbabbbaaabbaabaaa
babaaabbbaaabaababbaabababaaab
aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba", 12)]
        public void Example2(string input, int expected)
        {
            var (grammar, messages) = ParseInput(input);
            ChangeGrammarForPart2(grammar);
            var matches = grammar.MatchAll(messages);
            Assert.AreEqual(expected, matches.Count(),string.Join(Environment.NewLine,matches));
        }

        [Test]
        public void Part2()
        {
            var (grammar, messages) = ParseInput(Input);
            ChangeGrammarForPart2(grammar);
            Assert.AreEqual(334, grammar.MatchAll(messages).Length);
        }
        
        private static void ChangeGrammarForPart2(Grammar grammar)
        {
            grammar.SetRule(8,new MatchAnyRule(new[]
            {
                new MatchRuleIndex(new[] {42}),
                new MatchRuleIndex(new[] {42, 8})
            }));
            grammar.SetRule(11,new MatchAnyRule(new[]
            {
                new MatchRuleIndex(new[] {42, 31}),
                new MatchRuleIndex(new[] {42, 11, 31})
            }));
        }

        private static (Grammar grammar, string[] messages) ParseInput(string example)
        {
            bool IsRule(string x) => x.Contains(':');

            var lines = example.Split(Environment.NewLine);
            var grammar = new Grammar(lines
                .TakeWhile(IsRule)
                .Select(ParseRule)
                .ToArray());

            var messages = lines.SkipWhile(x => IsRule(x) || string.IsNullOrEmpty(x)).ToArray();
            return (grammar, messages);
        }

        private static (int index,Rule rule) ParseRule(string line)
        {
            var parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var index = int.Parse(parts[0]);
            var body = parts[1].Trim();

            return (index,body.StartsWith('"')
                ? (Rule) new MatchString(body.Split("\"", StringSplitOptions.RemoveEmptyEntries).First())
                : new MatchAnyRule(body.Split(" | ", StringSplitOptions.RemoveEmptyEntries).Select(y =>
                    new MatchRuleIndex(y.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)))));
        }
        
    }

    abstract class Rule
    {
        public abstract string[] Matches(string message, IDictionary<int, Rule> rules);
    }

    class MatchString : Rule
    {
        private readonly string _string;

        public MatchString(string @string)
        {
            _string = @string;
        }

        public override string[] Matches(string message, IDictionary<int, Rule> rules)
        {

            return message.StartsWith(_string) ? new[] {message.Substring(1)} : new string[0];
        }
    }

    class MatchRuleIndex : Rule
    {
        private readonly int[] _indexes;

        public MatchRuleIndex(IEnumerable<int> indexes) 
        {
            _indexes = indexes.ToArray();
        }

        public override string[] Matches(string message, IDictionary<int, Rule> rules)
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

        public override string[] Matches(string message, IDictionary<int, Rule> rules)
        {
            return _rules.SelectMany(x => x.Matches(message, rules)).ToArray();
        }
    }

    class Grammar
    {
        private readonly IDictionary<int,Rule> _rules;

        public Grammar((int index, Rule rule)[] rules)
        {
            _rules = rules.ToDictionary(x=>x.index,x=>x.rule);
        }
        
        public string[] MatchAll(string[] messages)
        {
            return messages.Where(MatchMessage).ToArray();
        }

        private bool MatchMessage(string message)
        {
            return _rules[0].Matches(message, _rules).Any(x=>x.Length == 0);
        }

        public void SetRule(int index, Rule rule)
        {
            _rules[index] = rule;
        }

        
    }

    
}