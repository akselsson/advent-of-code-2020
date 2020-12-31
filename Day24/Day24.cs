using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            return input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(line =>
                {
                    var actions = new List<(int deltaX, int deltaY)>();
                    for (int i = 0; i < line.Length; i++)
                    {
                        switch (line[i])
                        {
                            case 'e':
                                actions.Add((2, 0));
                                break;
                            case 'w':
                                actions.Add((-2, 0));
                                break;
                            case 'n':
                                actions.Add((line[++i] == 'e' ? 1 : -1, -1));
                                break;
                            case 's':
                                actions.Add((line[++i] == 'e' ? 1 : -1, 1));
                                break;
                        }
                    }

                    return actions;
                })
                .Select(line => line.Aggregate((deltaX: 0, deltaY: 0),
                    (agg, curr) => (agg.deltaX + curr.deltaX, agg.deltaY + curr.deltaY)))
                .Aggregate(new Dictionary<(int x, int y), bool>(), (agg, curr) =>
                {
                    agg[curr] = !agg.TryGetValue(curr, out var black) || !black;
                    return agg;
                })
                .Count(x => x.Value);
        }

        [Test]
        public void Part1()
        {
            Assert.AreEqual(377, CountBlack(Input));
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
}