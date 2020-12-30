using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Day22
{
    public class Day22
    {
        private readonly string Example =
            @"Player 1:
9
2
6
3
1

Player 2:
5
8
4
7
10";

        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            Assert.AreEqual(306, Play(Example));
        }

        [Test]
        public void Part1()
        {
            Assert.AreEqual(33631, Play(Input));
        }

        [Test]
        public void Example2()
        {
            Assert.AreEqual(291, PlayRecursive(Example));
        }

        [Test]
        public void Part2()
        {
            Assert.AreEqual(33469, PlayRecursive(Input));
        }
        
        private int Play(string input)
        {
            var decks = ParseInput(input);
            while (decks.All(x => x.Cards.Count > 0))
            {
                var cards = decks.Select(x => x.Cards.Dequeue()).ToList();
                CalculateWinnerOfRoundAndAddToDeck(cards, decks);
            }

            return CalculateScore(decks.Select(x => (x.Name, x.Cards, x.Cards.Any())).ToList());
        }

        private static void CalculateWinnerOfRoundAndAddToDeck(List<int> cards,
            List<(string Name, Queue<int> Cards)> decks)
        {
            var winner = cards.IndexOf(cards.Max());
            foreach (var card in cards.OrderByDescending(x => x))
            {
                decks[winner].Cards.Enqueue(card);
            }
        }

        private static int CalculateScore(List<(string Name, Queue<int> Cards, bool winner)> decks)
        {
            foreach (var card in decks.SelectMany(x => x.Cards))
            {
                Console.WriteLine(card);
            }

            return decks.Where(x => x.winner).SelectMany(x => x.Cards).Reverse().Select((x, i) => x * (i + 1))
                .Sum();
        }

        private static List<(string Name, Queue<int> Cards)> ParseInput(string input)
        {
            var decks = input
                .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(player =>
                {
                    var lines = player.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                    var cards = new Queue<int>(lines.Skip(1).Select(int.Parse).ToArray());
                    return (Name: lines[0], Cards: cards);
                }).ToList();
            return decks;
        }

        private int PlayRecursive(string input)
        {
            int score = 0;
            var maxStackSize = 16777216; //16 MB chosen at random
            var thread = new Thread(() =>
            {
                var decks = ParseInput(input);

                int games = 1;
                var result  = PlayRecursive(decks, new HashSet<string>(), 1, 1, ref games);

                score = CalculateScore(result);
            }, maxStackSize);
            thread.Start();
            thread.Join();
            return score;
        }

        private List<(string Name, Queue<int> Cards, bool Winner)> PlayRecursive(
            List<(string Name, Queue<int> Cards)> decks,
            HashSet<string> history, int round, int game, ref int gameCounter)
        {
            while (true)
            {
                if (decks.Any(x => !x.Cards.Any()))
                {
                    var result = decks.Select(x => (x.Name, x.Cards, winner: x.Cards.Any())).ToList();
                    //Console.WriteLine("Player deck is empty. Winner: " + result.First(x=>x.winner).Name);
                    return result;
                }

                if (history.Contains(CalculateHistory(decks)))
                {
                    //Console.WriteLine("Player 1 wins due to duplicate history");
                    return decks.Select((x, i) => (x.Name, x.Cards, winner: i == 0)).ToList();
                }

                history.Add(CalculateHistory(decks));

                //Console.WriteLine();
                //Console.WriteLine($"Start round: {round} game {game}{Environment.NewLine}{CalculateHistory(decks)}");

                var drawnCards = decks.Select(x => x.Cards.Dequeue()).ToList();
                var valueTuples = decks.Zip(drawnCards);
                if (valueTuples.All(x => x.First.Cards.Count() >= x.Second))
                {
                    //Console.WriteLine("Enter Recursive play");
                    gameCounter++;
                    var subgameResult = PlayRecursive(
                        valueTuples
                            .Select(x => (
                                x.First.Name,
                                new Queue<int>(x.First.Cards.Take(x.Second))
                            ))
                            .ToList(),
                        new HashSet<string>(),
                        1,
                        gameCounter,
                        ref gameCounter);
                    var winner = subgameResult.IndexOf(subgameResult.Find(x => x.Winner));
                    decks[winner].Cards.Enqueue(drawnCards[winner]);
                    decks[winner].Cards.Enqueue(drawnCards[(winner + 1) % 2]);
                    return PlayRecursive(decks, history, round + 1, game, ref gameCounter);
                }

                CalculateWinnerOfRoundAndAddToDeck(drawnCards, decks);
                round++;
            }
        }

        private string CalculateHistory(List<(string Name, Queue<int> Cards)> decks)
        {
            return string.Join(Environment.NewLine, decks.Select(x => $"{x.Name} {string.Join(",", x.Cards)}"));
        }
    }
}