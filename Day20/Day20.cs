using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
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
            Assert.AreEqual(20899048083289, CalculatePart1Answer(image));
        }

        [Test]
        public void Part1()
        {
            var tiles = Input.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseTile);
            var image = new Image(tiles.ToArray()).Assemble();
            Assert.AreEqual(54755174472007, CalculatePart1Answer(image));
        }

        [Test]
        public void Example2()
        {
        }

        [Test]
        public void Part2()
        {
        }

        private static long CalculatePart1Answer(long[,] image)
        {
            return image[0, 0] * image[0, image.GetLength(1) - 1] * image[image.GetLength(0) - 1, 0] *
                   image[image.GetLength(0) - 1, image.GetLength(1) - 1];
        }


        class Image
        {
            private readonly Tile[] _tiles;

            public Image(Tile[] tiles)
            {
                _tiles = tiles;
            }

            public long[,] Assemble()
            {
                foreach (var variant in _tiles.SelectMany(tile => tile.Variants))
                {
                    variant.FillPossibleMatches(_tiles);
                }

                var length = (int) Math.Sqrt(_tiles.Length);
                return _tiles
                    .SelectMany(tile => tile.Variants)
                    .AsParallel()
                    .SelectMany(variant => variant.TryFit(new TileVariant[length, length], _tiles.ToHashSet(), 0, 0))
                    .Select(image =>
                    {
                        var returnValue = new long[length, length];
                        for (int i = 0; i < image.GetLength(0); i++)
                        {
                            for (int j = 0; j < image.GetLength(1); j++)
                            {
                                returnValue[i, j] = image[i, j].Tile.Id;
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

        class TileVariant
        {
            public TileVariant(Tile tile, string[] content)
            {
                Tile = tile;
                Borders = GetBorders(content);
            }

            public Tile Tile { get; }
            public string[] Borders { get; }

            private IDictionary<int, List<TileVariant>> Matches { get; } = new Dictionary<int, List<TileVariant>>();

            static class Border
            {
                public const int Top = 0;
                public const int Right = 1;
                public const int Bottom = 2;
                public const int Left = 3;
            }

            private static string[] GetBorders(string[] content)
            {
                return new[]
                {
                    content[0],
                    new String(content.Select(x => x.Last()).ToArray()),
                    content[^1],
                    new String(content.Select(x => x.First()).ToArray())
                };
            }

            public void FillPossibleMatches(Tile[] tiles)
            {
                foreach (var tile in tiles.Where(t => t.Id != Tile.Id))
                {
                    foreach (var variant in tile.Variants)
                    {
                        for (int i = 0; i < Borders.Length; i++)
                        {
                            if (Borders[i] == variant.Borders[(i + 2) % Borders.Length])
                            {
                                if (!Matches.TryGetValue(i, out var matches))
                                {
                                    matches = new List<TileVariant>();
                                    Matches[i] = matches;
                                }

                                matches.Add(variant);
                            }
                        }
                    }
                }
            }

            public IEnumerable<TileVariant[,]> TryFit(TileVariant[,] image, HashSet<Tile> remainingTiles, int x, int y)
            {
                if (x > 0 && Borders[Border.Left] != image[x - 1, y].Borders[Border.Right])
                {
                    yield break;
                }

                if (y > 0 && Borders[Border.Top] != image[x, y - 1].Borders[Border.Bottom])
                {
                    yield break;
                }

                var newImage = (TileVariant[,]) image.Clone();
                newImage[x, y] = this;
                var newRemainingTiles = new HashSet<Tile>(remainingTiles);
                newRemainingTiles.Remove(Tile);

                if (x < image.GetLength(0) - 1)
                {
                    if (!Matches.TryGetValue(Border.Right, out var matches))
                    {
                        yield break;
                        ;
                    }

                    foreach (var match in matches)
                    {
                        if (!newRemainingTiles.Contains(match.Tile))
                        {
                            continue;
                        }

                        foreach (var result in match.TryFit(newImage, newRemainingTiles, x + 1, y))
                        {
                            yield return result;
                        }
                    }

                    yield break;
                }

                if (y < image.GetLength(1) - 1)
                {
                    if (!image[0, y].Matches.TryGetValue(Border.Bottom, out var matches))
                    {
                        yield break;
                    }

                    foreach (var match in matches)
                    {
                        if (!newRemainingTiles.Contains(match.Tile))
                        {
                            continue;
                        }

                        foreach (var result in match.TryFit(newImage, newRemainingTiles, 0, y + 1))
                        {
                            yield return result;
                        }
                    }

                    yield break;
                }

                yield return newImage;
            }
        }

        class Tile
        {
            public int Id { get; }
            private string[] _content;

            public TileVariant[] Variants { get; }

            public Tile(in int id, string[] content)
            {
                Id = id;
                _content = content;
                Variants = GenerateVariants(content).ToArray();
            }


            private IEnumerable<TileVariant> GenerateVariants(string[] content)
            {
                var flips = new List<string[]> {content};
                flips.Add(content.Select(b => new String(b.Reverse().ToArray())).ToArray());

                foreach (var flip in flips)
                {
                    yield return new TileVariant(this, flip);
                    var rotatedContent = flip;
                    for (int rotation = 0; rotation < 3; rotation++)
                    {
                        rotatedContent = rotatedContent
                            .Select((line, i) =>
                                line.Select((_, j) => rotatedContent[rotatedContent.Length - j - 1][i]))
                            .Select(x => new String(x.ToArray())).ToArray();
                        yield return new TileVariant(this, rotatedContent);
                    }
                }
            }
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

        [Test]
        public void ImageTestWithRotation()
        {
            var first = new Tile(1, new[]
            {
                "...",
                "#..",
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

        [Test]
        public void VariantsTest()
        {
            var tile = new Tile(1, new[]
            {
                "12",
                "34"
            });


            var expected = new[]
            {
                new[] {"12", "24", "34", "13"},
                new[] {"31", "12", "42", "34"},
                new[] {"43", "31", "21", "42"},
                new[] {"24", "43", "13", "21"},
                // Flip 
                new[] {"21", "13", "43", "24"},
                new[] {"42", "21", "31", "43"},
                new[] {"34", "42", "12", "31"},
                new[] {"13", "34", "24", "12"},
            };
            var variants = tile.Variants;
            CollectionAssert.AreEquivalent(expected, variants.Select(v => v.Borders));
        }

        [Test]
        public void VariantsTest2()
        {
            var input = new[]
            {
                "#....####.",
                "#..#.##...",
                "#.##..#...",
                "######.#.#",
                ".#...#.#.#",
                ".#########",
                ".###.#..#.",
                "########.#",
                "##...##.#.",
                "..###.#.#.",
            };
            var expected = "..#.###...,.####....#,.##...####,.#.#.###..";

            Console.WriteLine(expected);
            var tile = new Tile(1, input);
            foreach (var variant in tile.Variants.Where(x => x.Borders[0] == "..#.###..."))
            {
                Console.WriteLine(string.Join(",", variant.Borders));
            }

            CollectionAssert.Contains(tile.Variants.Select(x => string.Join(",", x.Borders)), expected);
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