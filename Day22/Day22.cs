using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Assert.AreEqual(306,Play(Example));
        }

        private int Play(string input)
        {
            var decks = input
                .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(player =>
                {
                    var lines = player.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                    var cards = new Queue<int>(lines.Skip(1).Select(int.Parse).ToArray());
                    return (Name: lines[0], Cards: cards);
                }).ToList();
            while (decks.All(x => x.Cards.Count > 0))
            {
                var cards = decks.Select(x=>x.Cards.Dequeue()).ToList();
                var winner = cards.IndexOf(cards.Max());
                foreach (var card in cards.OrderByDescending(x=>x))
                {
                    decks[winner].Cards.Enqueue(card);
                }
            }

            foreach (var card in decks.SelectMany(x=>x.Cards))
            {
                Console.WriteLine(card);
            }
            return decks.Where(x => x.Cards.Any()).SelectMany(x=>x.Cards).Reverse().Select((x, i) => x * (i + 1)).Sum();

        }

        [Test]
        public void Part1()
        {
            Assert.AreEqual(33631,Play(Input));
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