using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day3
{
    public class Day3Tests
    {
        private static readonly string[] ExampleInput =
        {
            "..##.......",
            "#...#...#..",
            ".#....#..#.",
            "..#.#...#.#",
            ".#...##..#.",
            "..#.##.....",
            ".#.#.#....#",
            ".#........#",
            "#.##...#...",
            "#...##....#",
            ".#..#...#.#"
        };

        private static readonly (int, int)[] PartTwoPaths = new[]
        {
            (1, 1),
            (3, 1),
            (5, 1),
            (7, 1),
            (1, 2)
        };

        private static readonly string[] Input = File.ReadAllLines("input.txt");

        [Test]
        public void Example1()
        {
            var trees = CountTrees(ExampleInput, deltaX: 3, deltaY: 1);
            Assert.AreEqual(7, trees);
        }

        [Test]
        public void Answer1()
        {
            Assert.AreEqual(207, CountTrees(Input, 3, 1));
        }

        [Test]
        public void Test2()
        {
            var answer = PartTwo(ExampleInput);
            Assert.AreEqual(336, answer);
        }

        [Test]
        public void Answer2()
        {
            Assert.AreEqual(2575411200, PartTwo(Input));
        }

        private long PartTwo(string[] input)
        {
            return PartTwoPaths.Select(x => CountTrees(input, x.Item1, x.Item2)).Aggregate((long) 1, (x, y) =>
            {
                Console.WriteLine($"{x},{y},{x * y}");
                return x * y;
            });
        }

        private long CountTrees(string[] input, int deltaX, int deltaY)
        {
            long count = 0;
            int x = 0, y = 0;
            while (y < input.Length)
            {
                var width = input[y].Length;
                if (input[y][x % width] == '#')
                {
                    count++;
                }

                x += deltaX;
                y += deltaY;
            }

            return count;
        }
    }
}