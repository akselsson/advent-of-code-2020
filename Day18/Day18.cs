using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using NUnit.Framework;

namespace Day18
{
    public class Day18
    {
        private readonly string Input = File.ReadAllText("input.txt");

        [TestCase("1 + 2 * 3 + 4 * 5 + 6", 71)]
        [TestCase("1 + (2 * 3) + (4 * (5 + 6))", 51)]
        [TestCase("2 * 3 + (4 * 5)", 26)]
        [TestCase("5 + (8 * 3 + 9 + 3 * 4 * 3)", 437)]
        [TestCase("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 12240)]
        [TestCase("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 13632)]
        public void Example1(string input, int value)
        {
            var tree = Parse(input);
            Assert.AreEqual(value, tree.Value);
        }
        
        [Test]
        public void Part1()
        {
            Assert.AreEqual(202553439706,Input.Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries).Select(input => Parse(input).Value).Sum());
        }
        
        
        [TestCase("1 + 2 * 3", 9)]
        [TestCase("3 * 1 + 2", 9)]
        [TestCase("1 + 2 * 3 + 4 * 5 + 6", 231)]
        [TestCase("1 + (2 * 3) + (4 * (5 + 6))",51)]
        [TestCase("2 * 3 + (4 * 5)", 46)]
        [TestCase("5 + (8 * 3 + 9 + 3 * 4 * 3)", 1445)]
        [TestCase("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 669060)]
        [TestCase("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 23340)]
        public void Example2(string input, int value)
        {
            var tree = Parse(input);
            Console.WriteLine("Before:" + tree);

            tree = ApplyOperatorPrecedence(tree);
            Console.WriteLine("After:" + tree);
            Assert.AreEqual(value,tree.Value);
        }
        
        [Test]
        public void Part2()
        {
            Assert.AreEqual(88534268715686,
                Input.Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries)
                    .Select(Parse)
                    .Select(ApplyOperatorPrecedence)
                    .Select(x=>x.Value)
                    .Sum());

        }

        private Expression ApplyOperatorPrecedence(Expression tree)
        {
            return tree.Accept(new ApplyOperatorPrecedenceVisitor());
        }
        
        class RemoveEmptyExpressions : VisitorBase
        {
            public override Expression Visit(EmptyExpression expression)
            {
                return expression.Next.Accept(this);
            }
        }
        
        class ApplyOperatorPrecedenceVisitor : VisitorBase
        {
            public override Expression Visit(PlusExpression expression)
            {
                if (expression.lhs is MultiplyExpression lhs)
                {
                    return new MultiplyExpression(lhs.lhs, new PlusExpression(lhs.rhs, expression.rhs));
                }

                return expression;
            }
        }

        private class VisitorBase : IVisitor
        {
            public virtual Expression Visit(InitExpression expression)
            {
                return expression;
            }

            public virtual Expression Visit(EmptyExpression expression)
            {
                return expression;
            }

            public virtual Expression Visit(LiteralExpression expression)
            {
                return expression;
            }

            public virtual Expression Visit(PlusExpression expression)
            {
                return expression;
            }

            public virtual Expression Visit(MultiplyExpression expression)
            {
                return expression;
            }

            public virtual Expression Visit(StartParenthesisExpression expression)
            {
                return expression;
            }

            public virtual Expression Visit(EndParenthesisExpression expression)
            {
                return expression;
            }
        }

        
        
        interface IVisitor
        {
            Expression Visit(InitExpression expression);
            Expression Visit(EmptyExpression expression);
            Expression Visit(LiteralExpression expression);
            Expression Visit(PlusExpression expression);
            Expression Visit(MultiplyExpression expression);
            Expression Visit(StartParenthesisExpression expression);
            Expression Visit(EndParenthesisExpression expression);
        }

        

        private static Expression Parse(string input)
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

                    throw new InvalidOperationException($"Unknown token {token}({(byte) token})");
                }).GetEnumerator();
            var tree = (Expression) new InitExpression();
            while (expressions.MoveNext())
            {
                var next = expressions.Current;
                next.Add(tree);
                next.Take(expressions);
                tree = next;
                Console.WriteLine($"{tree}");
            }

            return tree.Accept(new RemoveEmptyExpressions());
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

            public abstract Expression Accept(IVisitor visitor);
        }

        abstract class BinaryExpressions : Expression
        {
            public Expression lhs;
            public Expression rhs;

            protected BinaryExpressions(Expression lhs, Expression rhs)
            {
                this.lhs = lhs;
                this.rhs = rhs;
            }

            public BinaryExpressions()
            {
                
            }

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

            public override Expression Accept(IVisitor visitor)
            {
                return visitor.Visit(this);
            }
        }

        class EmptyExpression : UnaryExpression
        {
            public override long Value => _next.Value;
            public Expression Next => _next;

            private Expression _next;

            public EmptyExpression(Expression next)
            {
                _next = next;
            }

            public EmptyExpression()
            {
                
            }

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

            public override Expression Accept(IVisitor visitor)
            {
                return visitor.Visit(new EmptyExpression(_next.Accept(visitor)));
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

            public override Expression Accept(IVisitor visitor)
            {
                return visitor.Visit(this);
            }
        }

        class PlusExpression : BinaryExpressions
        {
            public PlusExpression(Expression lhs, Expression rhs) : base(lhs, rhs)
            {
                
            }

            public PlusExpression()
            {
                
            }

            public override long Value => lhs.Value + rhs.Value;

            public override string ToString()
            {
                return $"[{lhs}+{rhs}]";
            }

            public override Expression Accept(IVisitor visitor)
            {
                return visitor.Visit(new PlusExpression(lhs.Accept(visitor),rhs.Accept(visitor)));
            }
        }

        class MultiplyExpression : BinaryExpressions
        {
            public MultiplyExpression(Expression lhs, Expression rhs): base(lhs,rhs)
            {
            }

            public MultiplyExpression()
            {
                
            }

            public override long Value => lhs.Value * rhs.Value;

            public override string ToString()
            {
                return $"[{lhs}*{rhs}]";
            }

            public override Expression Accept(IVisitor visitor)
            {
                return visitor.Visit(new MultiplyExpression(lhs.Accept(visitor),rhs.Accept(visitor)));
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

            private StartParenthesisExpression(Expression inner)
            {
                _inner = inner;
            }

            public StartParenthesisExpression()
            {
                
            }

            public override long Value => _inner.Value;

            public override void Add(Expression previous)
            {
                
            }

            public override string ToString()
            {
                return "(" + _inner + ")";
            }

            public override Expression Accept(IVisitor visitor)
            {
                return visitor.Visit(new StartParenthesisExpression(_inner.Accept(visitor)));
            }
        }
        
        class EndParenthesisExpression : UnaryExpression
        {
            public override long Value =>
                throw new Exception($"{nameof(EndParenthesisExpression)} does not have a value");

            public override Expression Accept(IVisitor visitor)
            {
                return visitor.Visit(this);
            }
        }


       

    }

    
}