using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day17
{
    public class Day17
    {
        private readonly string Example =
            @".#.
..#
###";

        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            var game = GameOfCubes.Parse(Example);
            game.Play(6);
            Assert.AreEqual(112, game.ActiveCount);
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

    public class GameOfCubes
    {
        private bool[][][] _cube;

        private GameOfCubes(bool[][][] cube)
        {
            _cube = cube;
        }

        public static GameOfCubes Parse(string input)
        {
            var z0 = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Select(chr => chr == '#').ToArray()).ToArray();
            return new GameOfCubes(new bool[][][] {z0});
        }

        public void Play(int iterations)
        {
            Console.WriteLine($"Start");
            Console.WriteLine(this);
            Console.WriteLine();
            
            for (int i = 0; i < iterations; i++)
            {
                Console.WriteLine(new string('-',_cube.Length));
                Console.WriteLine();
                
                Play();

                Console.WriteLine($"Iteration {i+1}");
                Console.WriteLine(this);
                Console.WriteLine();
            }
        }
        
        private void Play()
        {
            var length = _cube.Length;

            var newLength = length + 2;
            var newCube = new bool[_cube.Length + 2][][];

            for (int zNew = -1; zNew < length + 1; zNew++)
            {
                var plane = newCube[zNew + 1] = new bool[newLength][];
                for (int yNew = -1; yNew < length + 1; yNew++)
                {
                    var line = plane[yNew + 1] = new bool[newLength];
                    for (int xNew = -1; xNew < length + 1; xNew++)
                    {
                        line[yNew + 1] = GetNewState(zNew, yNew, xNew);
                    }
                }
            }

            _cube = newCube;

        }

        private bool GetNewState(in int zNew, in int yNew, in int xNew)
        {
            int count = 0;
            var currentState = zNew >= 0 &&
                               yNew >= 0 &&
                               xNew >= 0 &&
                               zNew < _cube.Length &&
                               xNew < _cube.Length &&
                               yNew < _cube.Length &&
                               _cube[zNew][yNew][xNew];
            

            for (int z = Math.Max(0, zNew-1); z <= Math.Min(_cube.Length-1, zNew+1); z++)
            {
                for (int y = Math.Max(0, yNew-1); y <= Math.Min(_cube[z].Length-1, yNew+1); y++)
                {
                    for (int x = Math.Max(0, xNew-1); x <= Math.Min(_cube[z][y].Length-1, xNew+1); x++)
                    {
                        if (_cube[z][y][x] && z != zNew && y != yNew && x != xNew)
                        {
                            count++;
                        }
                    }
                }
            }

            Console.WriteLine($"{zNew} {yNew} {xNew} {currentState} {count}");

            return currentState ? count == 2 || count == 3 : count == 3;
        }

        public int ActiveCount => _cube.Sum(plane => plane.Sum(line => line.Count(pos => pos)));

        public override string ToString()
        {
            return string.Join(Environment.NewLine + Environment.NewLine,
                _cube.Select(
                    plane => String.Join(Environment.NewLine, plane.Select(
                        line => new String(line.Select(
                            x => x ? '#' : '.').ToArray()))
                    )
                )
            );
        }
    }
}