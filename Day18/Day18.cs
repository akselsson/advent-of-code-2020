using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Day18
{
    public class Day18
    {
        private readonly string Example =
            @"";

        private readonly string Input = File.ReadAllText("input.txt");

        [TestCase("1 + 2 * 3 + 4 * 5 + 6", 71)]
        [TestCase("1 + (2 * 3) + (4 * (5 + 6))", 51)]
        [TestCase("2 * 3 + (4 * 5)", 26)]
        [TestCase("5 + (8 * 3 + 9 + 3 * 4 * 3)", 437)]
        [TestCase("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 12240)]
        [TestCase("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 13632)]
        public void Example1(string input, int value)
        {
            Assert.AreEqual(value, Evaluate(input));
        }
        
        [Test]
        public void Part1()
        {
            Assert.AreEqual(202553439706,Input.Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries).Select(Evaluate).Sum());
        }

        private long Evaluate(string input)
        {
            using var expressions = input
                .Select<char, Expression>(token =>
                {
                    switch (token)
                    {
                        case '+':
                            return new PlusExpression();
                        case '*':
                            return new MultiplyExpression();
                        case ' ':
                            return new EmptyExpression();
                        case '(':
                            return new StartParenthesisExpression();
                        case ')':
                            return new EndParenthesisExpression();
                        case { } a when a >= '0' && a <= '9':
                            return new LiteralExpression(int.Parse(new string(new[] {token})));
                    }

                    throw new InvalidOperationException("Unknown token " + token);
                }).GetEnumerator();
            var tree = (Expression)new InitExpression();
            while (expressions.MoveNext())
            {
                var next = expressions.Current;
                next.Add(tree);
                next.Take(expressions);
                tree = next;
                Console.WriteLine($"{tree}");
            }
            return tree.Value;
        }


        abstract class Expression
        {
            public abstract void Take(IEnumerator<Expression> next);
            public abstract void Add(Expression previous);
            public abstract long Value { get; }

            public override string ToString()
            {
                return $"{GetType()}: {Value}";
            }
        }

        abstract class BinaryExpressions : Expression
        {
            protected Expression lhs, rhs;
            public override void Take(IEnumerator<Expression> next)
            {
                next.MoveNext();
                
                rhs = next.Current;
                rhs.Take(next);
            }

            public override void Add(Expression previous)
            {
                lhs = previous;
            }
        }

        abstract class UnaryExpression : Expression
        {
            protected Expression Previous;
            public override void Add(Expression previous)
            {
                Previous = previous;
            }

            public override void Take(IEnumerator<Expression> next)
            {
                
            }
        }

        class InitExpression : UnaryExpression
        {
            public override long Value => 0;
            public override string ToString()
            {
                return "";
            }
        }

        class EmptyExpression : UnaryExpression
        {
            public override long Value => _next.Value;
            private Expression _next;
            public override void Take(IEnumerator<Expression> next)
            {
                next.MoveNext();
                _next = next.Current;
                _next.Add(Previous);
                _next.Take(next);
            }

            public override string ToString()
            {
                return _next.ToString();
            }
        }

        class LiteralExpression : UnaryExpression
        {
            public LiteralExpression(int value)
            {
                Value = value;
            }

            public override long Value { get; }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        class PlusExpression : BinaryExpressions
        {
            public override long Value => lhs.Value + rhs.Value;

            public override string ToString()
            {
                return lhs + "+" + rhs;
            }
        }

        class MultiplyExpression : BinaryExpressions
        {
            public override long Value => lhs.Value * rhs.Value;

            public override string ToString()
            {
                return lhs + "*" + rhs;
            }
        }

        class StartParenthesisExpression : Expression
        {
            public override void Take(IEnumerator<Expression> next)
            {
                while (next.MoveNext() && next.Current is not EndParenthesisExpression)
                {
                    var current = next.Current;
                    current.Add(_inner);
                    current.Take(next);
                    _inner = current;
                }
            }

            private Expression _inner = new InitExpression();
            public override long Value => _inner.Value;

            public override void Add(Expression previous)
            {
                
            }

            public override string ToString()
            {
                return "(" + _inner + ")";
            }
        }
        
        class EndParenthesisExpression : UnaryExpression
        {
            public override long Value =>
                throw new Exception($"{nameof(EndParenthesisExpression)} does not have a value");
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