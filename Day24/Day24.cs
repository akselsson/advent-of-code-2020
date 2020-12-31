using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using NUnit.Framework;

namespace Day24
{
    public class Day24
    {
        private readonly string Example =
            @"sesenwnenenewseeswwswswwnenewsewsw
neeenesenwnwwswnenewnwwsewnenwseswesw
seswneswswsenwwnwse
nwnwneseeswswnenewneswwnewseswneseene
swweswneswnenwsewnwneneseenw
eesenwseswswnenwswnwnwsewwnwsene
sewnenenenesenwsewnenwwwse
wenwwweseeeweswwwnwwe
wsweesenenewnwwnwsenewsenwwsesesenwne
neeswseenwwswnwswswnw
nenwswwsewswnenenewsenwsenwnesesenew
enewnwewneswsewnwswenweswnenwsenwsw
sweneswneswneneenwnewenewwneswswnese
swwesenesewenwneswnwwneseswwne
enesenwswwswneneswsenwnewswseenwsese
wnwnesenesenenwwnenwsewesewsesesew
nenewswnwewswnenesenwnesewesw
eneswnwswnwsenenwnwnwwseeswneewsenese
neswnwewnwnwseenwseesewsenwsweewe
wseweeenwnesenwwwswnew";

        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            Assert.AreEqual(10, CountBlack(Example));
        }

        private int CountBlack(string input)
        {
            return GetInitialLayout(input)
                .Count(x => x.Value);
        }

        private static Dictionary<Vector, bool> GetInitialLayout(string input)
        {
            return input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(line =>
                {
                    var actions = new List<Vector>();
                    for (int i = 0; i < line.Length; i++)
                    {
                        switch (line[i])
                        {
                            case 'e':
                                actions.Add(new Vector(2, 0));
                                break;
                            case 'w':
                                actions.Add(new Vector(-2, 0));
                                break;
                            case 'n':
                                actions.Add(new Vector(line[++i] == 'e' ? 1 : -1, -1));
                                break;
                            case 's':
                                actions.Add(new Vector(line[++i] == 'e' ? 1 : -1, 1));
                                break;
                        }
                    }

                    return actions;
                })
                .Select(line => line.Aggregate(new Vector(X: 0, Y: 0),
                    (agg, curr) => agg + curr))
                .Aggregate(new Dictionary<Vector, bool>(), (agg, curr) =>
                {
                    agg[curr] = !agg.TryGetValue(curr, out var black) || !black;
                    return agg;
                });
        }

        record Vector(int X, int Y)
        {
            public static Vector operator +(Vector first, Vector second)
            {
                return new(first.X + second.X, first.Y + second.Y);
            }
        }

        [Test]
        public void Part1()
        {
            Assert.AreEqual(377, CountBlack(Input));
        }

        [Test]
        public void Example2()
        {
            //Assert.AreEqual(15, Play(Example, 1));
            Assert.AreEqual(12, Play(Example, 2));
            Assert.AreEqual(2208, Play(Example, 100));
        }
        
        [Test]
        public void Part2()
        {
            Assert.AreEqual(4231, Play(Input, 100));
        }

        private int Play(string input, int days)
        {
            var directions = new[]
            {
                new Vector(2, 0),
                new Vector(-2, 0),
                new Vector(1, -1),
                new Vector(-1, -1),
                new Vector(1, 1),
                new Vector(-1, 1)
            };
            var layout = GetInitialLayout(input);
            //PrintTiles(layout);

            while (days-- > 0)
            {
                var neighbours = layout
                    .Where(x => x.Value)
                    .SelectMany(kvp => directions.Select(x => x + kvp.Key))
                    .Where(x => !layout.ContainsKey(x))
                    .Distinct()
                    .ToDictionary(x => x, _ => false);
                layout = new Dictionary<Vector, bool>(
                    layout
                        .Concat(neighbours)
                        .OrderBy(x=>x.Key.Y).ThenBy(x=>x.Key.X)
                        .Select(kvp =>
                {
                    var countAdjacent = directions.Aggregate(0,
                        (agg, curr) => layout.TryGetValue(kvp.Key + curr, out var black) && black ? agg + 1 : agg);
                    return new KeyValuePair<Vector, bool>(kvp.Key,
                        kvp.Value ? (countAdjacent == 1 || countAdjacent == 2) : countAdjacent == 2);
                }).Where(x => x.Value));

                //Console.WriteLine();
                //PrintTiles(layout);
            }

            return layout.Count();
        }

        private static void PrintTiles(Dictionary<Vector, bool> layout)
        {
            Console.WriteLine($"{layout.Min(kvp => kvp.Key.X),4:N0}");
            for (int y = layout.Min(kvp => kvp.Key.Y); y < layout.Max(kvp => kvp.Key.Y); y++)
            {
                Console.Write($"{y,2:N0} ");
                for (int x = layout.Min(kvp => kvp.Key.X); x < layout.Max(kvp => kvp.Key.X); x++)
                {
                    Console.Write( x % 2 == 0 && y % 2 == 0 || Math.Abs(x % 2) == 1 && Math.Abs(y % 2) == 1
                        ? layout.TryGetValue(new Vector(x, y), out var black) && black ? '#' : '.'
                        : ' ');
                }

                Console.WriteLine();
            }
        }

        
    }
}