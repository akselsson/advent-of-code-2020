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
            var seats = Seats.Parse(Example,Seats.CountAdjacent);
            seats.RunUntilStable();
            Assert.AreEqual(37, seats.CountOccupied());
        }

        [Test]
        public void Part1()
        {
            var seats = Seats.Parse(Input,Seats.CountAdjacent);
            seats.RunUntilStable();
            Assert.AreEqual(2261, seats.CountOccupied());
        }

        [Test]
        public void Example2()
        {
            var seats = Seats.Parse(Example,Seats.CountNearestSeat);
            seats.RunUntilStable();
            Assert.AreEqual(26, seats.CountOccupied());
        }

        [Test]
        public void Part2()
        {
            var seats = Seats.Parse(Input,Seats.CountNearestSeat);
            seats.RunUntilStable();
            Assert.AreEqual(2039, seats.CountOccupied());
        }
    }

    public class Seats
    {
        private char[][] _state;
        private Func<char[][], int, int, bool> _shouldToggle;
        

        public static Seats Parse(
            string example, 
            Func<char[][],int,int,bool> counter)
        {
            return new()
            {
                _state = example.Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToCharArray()).ToArray(),
                _shouldToggle = counter
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
                            
                            if (_shouldToggle(_state, i, j)){
                                newState[i][j] = '#';
                            }
                            break;
                        case '#':
                            if (_shouldToggle(_state, i, j))
                            {
                                newState[i][j] = 'L';
                            }
                            break;
                    }   
                }
            }
            _state = newState;

        }

        public static bool CountAdjacent(char[][] state, int row, int column)
        {
            var count = 0;
            for (int i = Math.Max(0,row-1); i < Math.Min(state.Length,row+2); i++)
            {
                var candidateRow = state[i];
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

            return count == 0 && state[row][column] == 'L' || count >= 4 && state[row][column] == '#' ;
        }

        public int CountOccupied()
        {
            return _state.Sum(x => x.Count(y => y == '#'));
        }


        public static bool CountNearestSeat(char[][] state, int row, int column)
        {
            var directions = new (int row, int column)[]
            {
                (1, 1),
                (1, 0),
                (1, -1),
                (0, 1),
                (0, -1),
                (-1, 1),
                (-1, 0),
                (-1, -1),
            };
            /*var directions = Enumerable.Range(-1, 3)
                .SelectMany(x => Enumerable.Range(-1, 3).Select(y => (row: x, column: y)))
                .Where(x => x.row != 0 && x.column != 0);*/
            var count = directions.Select(x =>
            {
                var position = (row: row + x.row, column: column + x.column);
                while (position.row >= 0 && position.column >= 0 && position.row < state.Length &&
                       position.column < state[row].Length)
                {
                    switch (state[position.row][position.column])
                    {
                        case '#':
                            return 1;
                        case 'L':
                            return 0;

                    }
                    position = (row: position.row + x.row, column: position.column + x.column);
                }

                return 0;
            }).Sum();
            return count == 0 && state[row][column] == 'L' || count >= 5 && state[row][column] == '#' ;
        }
    }
}