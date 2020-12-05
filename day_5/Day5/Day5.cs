using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day5
{
    public class Day5
    {
        private static readonly string[] ExampleInput = {
            "BFFFBBFRRR",
            "FFFBBBFRRR",
            "BBFFBBFRLL"
        };

        private static readonly string[] Input = File.ReadAllLines("input.txt");

        [Test]
        public void Example1()
        {
            CollectionAssert.AreEquivalent(new[]
            {
                (70, 7, 567),
                (14, 7, 119),
                (102, 4, 820)
            }, ExampleInput.Select(ParseSeat));
        }

        [Test]
        public void Assignment1()
        {
            Assert.AreEqual(922,Input.Select(ParseSeat).Select(x=>x.Item3).Max());
        }
        
        

        [Test]
        public void Assignment2()
        {
            var seats = Input.Select(ParseSeat).OrderBy(x=>x.Item1).ThenBy(x=>x.Item2).ToArray();
            var first = seats.First().Item3-1;
            var lastBeforeGap = seats.TakeWhile(x => ++first == x.Item3).Last();
            
            Assert.AreEqual(747,lastBeforeGap.Item3+1);
        }

        [Test]
        public void Example1_version2()
        {
            Assert.AreEqual(567,ParseSeat2(ExampleInput[0]));
        }
        
        [Test]
        public void Assignment1_version2()
        {
            Assert.AreEqual(922,Input.Select(ParseSeat2).Max());
        }
        
        [Test]
        public void Assignment2_version2()
        {
            var seats = Input.Select(ParseSeat2).OrderBy(x=>x);
            var gap = seats.Aggregate(
                (previous: 0, gap: 0),
                (agg, curr) => curr == agg.previous + 1 ? (curr, agg.gap) : (curr, curr-1)
                );
            Assert.AreEqual(747,gap.gap);
        }

        private int ParseSeat2(string input)
        {
            int val = 0;
            int mask = 1;
            foreach (var character in input.Reverse())
            {

                switch (character)
                {
                    case 'F':
                    case 'L':
                        break;
                    case 'B':
                    case 'R':
                        val |= mask;

                        break;
                }
                mask <<= 1;


            }

            return val;
        }

        private (int,int,int) ParseSeat(string input)
        {
            var row = ParseBSP(input.Substring(0, 7),'F','B');
            var column = ParseBSP(input.Substring(7, 3),'L','R');
            return (row, column, row*8+column);

        }

        private int ParseBSP(string input, char lower, char upper)
        {
            int rangeUpper = (int)Math.Pow(2,input.Length);
            int rangeLower = 0;
            foreach (var character in input)
            {
                var midpoint = (rangeUpper + rangeLower) / 2;
                if (character == lower)
                {
                    rangeUpper = midpoint;
                }
                else if (character == upper)
                {
                    rangeLower = midpoint;
                }
                else
                {
                    throw new Exception($"Unknown character {character}");
                }
            }

            return rangeLower;
        }
    }
}