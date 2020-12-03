using System.IO;
using NUnit.Framework;

namespace Day3
{
    public class Day3Tests
    {
        [Test]
        public void Example1()
        {
            var input = new[]
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

            var trees = CountTrees(input, deltaX: 3, deltaY: 1);
            Assert.AreEqual(7, trees);
        }

        [Test]
        public void Answer1()
        {
            var input = File.ReadAllLines("input.txt");
            Assert.AreEqual(207, CountTrees(input, 3, 1));
        }

        private int CountTrees(string[] input, int deltaX, int deltaY)
        {
            int count = 0;
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