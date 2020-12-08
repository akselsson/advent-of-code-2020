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
            var program = Program.Parse(Example);
            Assert.AreEqual(8,program.Fix().Value);
            
        }

        [Test]
        public void Part2()
        {
            var program = Program.Parse(Input);
            Assert.AreEqual(2096,program.Fix().Value);
        }


        private class Program
        {
            private readonly (string operation, int value)[] _operations;

            private Program((string operation, int value)[] operations)
            {
                this._operations = operations.ToArray();
            }

            public (bool Completed, int Value) Run()
            {
                var visitedLocations = new HashSet<int>();
                var currentLocation = 0;
                var value = 0;
                while (currentLocation < _operations.Length && !visitedLocations.Contains(currentLocation))
                {
                    visitedLocations.Add(currentLocation);
                    var operation = _operations[currentLocation];

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

                return (currentLocation == _operations.Length, value);
            }

            public static Program Parse(string example)
            {
                var parser = new Regex(@"(?<op>\w{3}) (?<value>[+-]\d+)");
                var operations = example
                    .Split(Environment.NewLine)
                    .Select(line => parser.Match(line))
                    .Where(x => x.Success)
                    .Select(match => (match.Groups["op"].Value, int.Parse(match.Groups["value"].Value)));
                return new Program(operations.ToArray());
            }

            public (bool Completed, int Value) Fix()
            {
                var operationsClone = _operations.ToArray(); 
                for (int i = 0; i < _operations.Length; i++)
                {
                    switch (_operations[i].operation)
                    {
                        case "jmp":
                            var newResult = TryRunWithOperation(operationsClone, i, "nop");
                            if (newResult.Completed)
                            {
                                return newResult;
                            }
                            break;
                        case "nop":
                            var newResult2 = TryRunWithOperation(operationsClone,i, "jmp");
                            if (newResult2.Completed)
                            {
                                return newResult2;
                            }
                            break;
                    }
                }

                return (false,0);
            }

            private (bool Completed, int Value) TryRunWithOperation((string operation, int value)[] operations, int i, string operation)
            {
                var oldOperation = operations[i];
                operations[i] = (operation, operations[i].value);
                var newProgram = new Program(operations);
                var result = newProgram.Run();
                operations[i] = oldOperation;
                return result;
            }
        }
    }
}