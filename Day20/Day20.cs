using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Day20
{
    public class Day20
    {
        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            var tiles = Example.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseTile);
            var image = new Image(tiles.ToArray()).Assemble();
            Assert.AreEqual(1951, image[0, 0]);
            CollectionAssert.AreEqual(new int[3, 3]
            {
                {1951, 2311, 3079},
                {2729, 1427, 2473},
                {2971, 1489, 1171}
            }, image);
        }

        [Test]
        public void ImageTest()
        {
            var first = new Tile(1, new[]
            {
                "...",
                "..#",
                "..."
            });
            var second = new Tile(2, new[]
            {
                "...",
                "#..",
                "..."
            });
            var third = new Tile(3, new[]
            {
                "...",
                "..#",
                "..."
            });
            var fourth = new Tile(4, new[]
            {
                "...",
                "#..",
                "..."
            });
            var image = new Image(new[] {first, second, third, fourth});
            CollectionAssert.AreEquivalent(new[,] {{1, 2}, {3, 4}}, image.Assemble());
        }

        class Image
        {
            private readonly Tile[] _tiles;

            public Image(Tile[] tiles)
            {
                _tiles = tiles;
            }

            public int[,] Assemble()
            {
                foreach (var tile in _tiles)
                {
                    tile.FindPossibleMatches(_tiles);
                }

                var length = (int) Math.Sqrt(_tiles.Length);
                return _tiles
                    .SelectMany(tile => tile.TryFit(new Tile[length, length], _tiles, 0, 0))
                    .Select(image =>
                {
                    var returnValue = new int[length, length];
                    for (int i = 0; i < image.GetLength(0); i++)
                    {
                        for (int j = 0; j < image.GetLength(1); j++)
                        {
                            returnValue[i, j] = image[i, j].Id;
                        }
                    }

                    return returnValue;
                }).First();
            }
        }


        private Tile ParseTile(string tile)
        {
            Regex tileNo = new Regex("Tile (?<tile>\\d+):");
            var index = int.Parse(tileNo.Match(tile).Groups["tile"].Value);
            return new Tile(index,
                tile.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray());
        }

        class Tile
        {
            public int Id { get; }
            private IDictionary<int, List<Tile>> _matches = new Dictionary<int, List<Tile>>();

            //Top, Right, Bottom, Left
            private string[] _borders;

            public Tile(in int id, string[] content)
            {
                Id = id;
                _borders = new[]
                {
                    content[0],
                    new String(content.Select(x => x.Last()).ToArray()),
                    content[^1],
                    new String(content.Select(x => x.First()).ToArray())
                };
            }

            public void FindPossibleMatches(Tile[] tiles)
            {
                foreach (var tile in tiles.Where(t => t != this))
                {
                    for (int i = 0; i < _borders.Length; i++)
                    {
                        if (_borders[i] == tile._borders[(i + 2) % _borders.Length])
                        {
                            if (!_matches.TryGetValue(i, out var matches))
                            {
                                matches = new List<Tile>();
                                _matches[i] = matches;
                            }

                            matches.Add(tile);
                        }
                    }
                }
            }

            public IEnumerable<Tile[,]> TryFit(Tile[,] image, Tile[] remainingTiles, int x, int y)
            {
                if (x > 0 && image[x - 1, y]._borders[1] != _borders[3])
                {
                    return Enumerable.Empty<Tile[,]>();
                }

                if (y > 0 && image[x, y - 1]._borders[2] != _borders[0])
                {
                    return Enumerable.Empty<Tile[,]>();
                }

                var newImage = (Tile[,]) image.Clone();
                newImage[x, y] = this;
                var newRemainingTiles = remainingTiles.Except(new[] {this}).ToArray();

                if (x < image.GetLength(0)-1)
                {
                    if (!_matches.TryGetValue(1, out var matches))
                    {
                        return Enumerable.Empty<Tile[,]>();
                    }

                    return matches
                        .Where(newRemainingTiles.Contains)
                        .Where(tile => x == 0 || image[x - 1, y]._borders[1] == tile._borders[3])
                        .SelectMany(match => match.TryFit(newImage, newRemainingTiles, x + 1, y));
                }

                if (y < image.GetLength(1)-1)
                {
                    if (!image[0, y]._matches.TryGetValue(1, out var matches))
                    {
                        return Enumerable.Empty<Tile[,]>();
                    }

                    return matches
                        .Where(newRemainingTiles.Contains)
                        .SelectMany(match => match.TryFit(newImage, newRemainingTiles, 0, y + 1));
                }

                return new[] {newImage};
            }
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

        private readonly string Example =
            @"Tile 2311:
..##.#..#.
##..#.....
#...##..#.
####.#...#
##.##.###.
##...#.###
.#.#.#..##
..#....#..
###...#.#.
..###..###

Tile 1951:
#.##...##.
#.####...#
.....#..##
#...######
.##.#....#
.###.#####
###.##.##.
.###....#.
..#.#..#.#
#...##.#..

Tile 1171:
####...##.
#..##.#..#
##.#..#.#.
.###.####.
..###.####
.##....##.
.#...####.
#.##.####.
####..#...
.....##...

Tile 1427:
###.##.#..
.#..#.##..
.#.##.#..#
#.#.#.##.#
....#...##
...##..##.
...#.#####
.#.####.#.
..#..###.#
..##.#..#.

Tile 1489:
##.#.#....
..##...#..
.##..##...
..#...#...
#####...#.
#..#.#.#.#
...#.#.#..
##.#...##.
..##.##.##
###.##.#..

Tile 2473:
#....####.
#..#.##...
#.##..#...
######.#.#
.#...#.#.#
.#########
.###.#..#.
########.#
##...##.#.
..###.#.#.

Tile 2971:
..#.#....#
#...###...
#.#.###...
##.##..#..
.#####..##
.#..####.#
#..#.#..#.
..####.###
..#.#.###.
...#.#.#.#

Tile 2729:
...#.#.#.#
####.#....
..#.#.....
....#..#.#
.##..##.#.
.#.####...
####.#.#..
##.####...
##..#.##..
#.##...##.

Tile 3079:
#.#.#####.
.#..######
..#.......
######....
####.#..#.
.#...#.##.
#.#####.##
..#.###...
..#.......
..#.###...";
    }
}