using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day11
{
    public class Day11
    {
        private readonly string Example =
            @"L.LL.LL.LL
LLLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLLL
L.LLLLLL.L
L.LLLLL.LL";

        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            var seats = Seats.Parse(Example);
            seats.RunUntilStable();
            Assert.AreEqual(37, seats.CountOccupied());
        }

        [Test]
        public void Part1()
        {
            var seats = Seats.Parse(Input);
            seats.RunUntilStable();
            Assert.AreEqual(2261, seats.CountOccupied());
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

    public class Seats
    {
        private char[][] _state;
        public static Seats Parse(string example)
        {
            return new()
            {
                _state = example.Split(Environment.NewLine).Select(x => x.ToCharArray()).ToArray()
            };
        }

        public override string ToString()
        {
            return String.Join(Environment.NewLine, _state.Select(x => new string(x)));
        }


        public void RunUntilStable()
        {
            var current = CountOccupied();
            int previous;
            do
            {
                previous = current;

                Run();
                
                Console.WriteLine(this);
                Console.WriteLine();
                
                current = CountOccupied();
            } while (CountOccupied() != previous);
        }

        private void Run()
        {
            var newState = new char[_state.Length][];
            for (int i = 0; i < _state.Length; i++)
            {
                var row = _state[i];
                newState[i] = row.ToArray();
                for (int j = 0; j < row.Length; j++)
                {
                    switch (row[j])
                    {

                        case 'L':
                            if (CountAdjacentOccupied(i, j) == 0){
                                newState[i][j] = '#';
                            }
                            break;
                        case '#':
                            if (CountAdjacentOccupied(i, j) >= 4)
                            {
                                newState[i][j] = 'L';
                            }
                            break;
                    }   
                }
            }
            _state = newState;

        }

        private int CountAdjacentOccupied(in int row, in int column)
        {
            var count = 0;
            for (int i = Math.Max(0,row-1); i < Math.Min(_state.Length,row+2); i++)
            {
                var candidateRow = _state[i];
                for (int j = Math.Max(0,column-1); j < Math.Min(candidateRow.Length,column+2); j++)
                {
                    if (i == row && j == column)
                    {
                        continue;
                    }

                    if (candidateRow[j] == '#')
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        public int CountOccupied()
        {
            return _state.Sum(x => x.Count(y => y == '#'));
        }
    }
}