using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Day8
{
    public class Day8
    {
        private readonly string Example =
            @"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6";

        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            var program = Program.Parse(Example);
            Assert.AreEqual(5, program.Run().Value);
        }

        [Test]
        public void Part1()
        {
            var program = Program.Parse(Input);
            Assert.AreEqual(1941, program.Run().Value);
        }

        [Test]
        public void Example2()
        {
            var program = Program.Parse(Example).Fix();
            Assert.AreEqual(8,program.Run().Value);
            
        }

        [Test]
        public void Part2()
        {
            var program = Program.Parse(Input).Fix();
            Assert.AreEqual(2096,program.Run().Value);
        }


        private class Program
        {
            private readonly (string operation, int value)[] _stack;

            private Program(IEnumerable<(string operation, int value)> operations)
            {
                _stack = operations.ToArray();
            }

            public (bool Completed, int Value) Run()
            {
                var visitedLocations = new HashSet<int>();
                var currentLocation = 0;
                var value = 0;
                while (currentLocation < _stack.Length && !visitedLocations.Contains(currentLocation))
                {
                    visitedLocations.Add(currentLocation);
                    var operation = _stack[currentLocation];

                    switch (operation.operation)
                    {
                        case "acc":
                            value += operation.value;
                            currentLocation++;
                            break;
                        case "jmp":
                            currentLocation += operation.value;
                            break;
                        case "nop":
                            currentLocation++;
                            break;
                    }

                    Console.WriteLine($"{currentLocation}: {operation} {value}");
                }

                return (currentLocation == _stack.Length, value);
            }

            public static Program Parse(string example)
            {
                var parser = new Regex(@"(?<op>\w{3}) (?<value>[+-]\d+)");
                var operations = example
                    .Split(Environment.NewLine)
                    .Select(line => parser.Match(line))
                    .Where(x => x.Success)
                    .Select(match => (match.Groups["op"].Value, int.Parse(match.Groups["value"].Value)));
                return new Program(operations);
            }

            public Program Fix()
            {
                
                for (int i = 0; i < _stack.Length; i++)
                {
                    switch (_stack[i].operation)
                    {
                        case "jmp":
                            var newProgram = SwitchOperation(i, "nop");
                            if (newProgram.Run().Completed)
                            {
                                return newProgram;
                            }
                            break;
                        case "nop":
                            var newProgram2 = SwitchOperation(i, "nop");
                            if (newProgram2.Run().Completed)
                            {
                                return newProgram2;
                            }
                            break;
                    }
                }

                return this;
            }

            private Program SwitchOperation(int i, string operation)
            {
                var newStack = _stack.ToList();
                newStack[i] = (operation, newStack[i].value);
                var newProgram = new Program(newStack);
                return newProgram;
            }
        }
    }
}