using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var game = GameOfCubes.Parse(Input);
            game.Play(6);
            Assert.AreEqual(207, game.ActiveCount);
        }

        [Test]
        public void Example2()
        {
            var game = GameOfCubes.Parse(Example);
            game.Play(6,true);
            Assert.AreEqual(848, game.ActiveCount);
        }

        [Test]
        public void Part2()
        {
            var game = GameOfCubes.Parse(Input);
            game.Play(6,true);
            Assert.AreEqual(2308, game.ActiveCount);
        }
    }

    public record Coordinate(int X, int Y, int Z, int W);

    public class GameOfCubes
    {
        private HashSet<Coordinate> _activeCells;

        public GameOfCubes(HashSet<Coordinate> activeCells)
        {
            _activeCells = activeCells;
        }


        public static GameOfCubes Parse(string input)
        {
            var activeCells = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .SelectMany((line, y) =>
                    line.Select((chr, x) => (chr == '#', x))
                        .Where(c => c.Item1)
                        .Select(c => new Coordinate(y, c.x, 0, 0))
                ).ToHashSet();
            return new GameOfCubes(activeCells);
        }

        public void Play(int iterations, bool _4D = false)
        {
            Console.WriteLine($"Start");
            Console.WriteLine(this);
            Console.WriteLine();

            for (int i = 0; i < iterations; i++)
            {
                Console.WriteLine(new string('-', _activeCells.Count));
                Console.WriteLine();

                if (_4D)
                {
                    Play4D();
                }
                else
                {
                    Play3D();
                }

                Console.WriteLine($"Iteration {i + 1}");
                Console.WriteLine(this);
                Console.WriteLine();
            }
        }

        private void Play3D()
        {
            IDictionary<Coordinate, (int count, bool isActive)> neighBors =
                _activeCells.ToDictionary(x => x, _ => (0, true));
            foreach (var cell in _activeCells)
            {
                for (int x = cell.X - 1; x < cell.X + 2; x++)
                {
                    for (int y = cell.Y - 1; y < cell.Y + 2; y++)
                    {
                        for (int z = cell.Z - 1; z < cell.Z + 2; z++)
                        {
                            var coordinate = new Coordinate(x, y, z, 0);
                            if (coordinate == cell)
                            {
                                continue;
                            }

                            neighBors[coordinate] =
                                neighBors.TryGetValue(coordinate, out var state)
                                    ? (state.count + 1, state.isActive)
                                    : (1, false);
                        }
                    }
                }
            }

            _activeCells = neighBors
                .Where(x => x.Value.isActive ? (x.Value.count == 2 || x.Value.count == 3) : x.Value.count == 3)
                .Select(x => x.Key)
                .ToHashSet();
        }

        private void Play4D()
        {
            IDictionary<Coordinate, (int count, bool isActive)> neighBors =
                _activeCells.ToDictionary(x => x, _ => (0, true));
            foreach (var cell in _activeCells)
            {
                for (int x = cell.X - 1; x < cell.X + 2; x++)
                {
                    for (int y = cell.Y - 1; y < cell.Y + 2; y++)
                    {
                        for (int z = cell.Z - 1; z < cell.Z + 2; z++)
                        {
                            for (int w = cell.W - 1; w < cell.W + 2; w++)
                            {
                                var coordinate = new Coordinate(x, y, z, w);
                                if (coordinate == cell)
                                {
                                    continue;
                                }

                                neighBors[coordinate] =
                                    neighBors.TryGetValue(coordinate, out var state)
                                        ? (state.count + 1, state.isActive)
                                        : (1, false);
                            }
                        }
                    }
                }
            }

            _activeCells = neighBors
                .Where(x => x.Value.isActive ? (x.Value.count == 2 || x.Value.count == 3) : x.Value.count == 3)
                .Select(x => x.Key)
                .ToHashSet();
        }

        public int ActiveCount => _activeCells.Count;

        public override string ToString()
        {
            var max = _activeCells.Aggregate(
                new Coordinate(0, 0, 0, 0),
                (curr, agg) =>
                    new Coordinate(Math.Max(curr.X, agg.X), Math.Max(curr.Y, agg.Y), Math.Max(curr.Z, agg.Z), Math.Max(curr.W, agg.W)));
            var min = _activeCells.Aggregate(
                max,
                (curr, agg) =>
                    new Coordinate(Math.Min(curr.X, agg.X), Math.Min(curr.Y, agg.Y), Math.Min(curr.Z, agg.Z),Math.Min(curr.W, agg.W)));
            StringBuilder sb = new StringBuilder();
            for (int w = min.W; w <= max.W; w++)
            {
                for (int z = min.Z; z <= max.Z; z++)
                {
                    sb.AppendLine($"z={z} w={w}");
                    for (int x = min.X; x <= max.X; x++)
                    {
                        for (int y = min.Y; y <= max.Y; y++)
                        {
                            sb.Append(_activeCells.Contains(new Coordinate(x, y, z, w)) ? '#' : '.');
                        }

                        sb.AppendLine();
                    }

                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}