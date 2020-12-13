using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day13
{
    public class Day13
    {
        private readonly string Example =
            @"939
7,13,x,x,59,x,31,19";

        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            var (busId,timeUntilNextDeparture) = CalculateDepartureTime(Example);
            
            Assert.AreEqual(295,busId*timeUntilNextDeparture);
            
        }
        
        [Test]
        public void Part1()
        {
            var (busId,timeUntilNextDeparture) = CalculateDepartureTime(Input);
            
            Assert.AreEqual(4782,busId*timeUntilNextDeparture);
        }

        private (int busId, int timeUntilNextDeparture) CalculateDepartureTime(string input)
        {
            var lines = input.Split(Environment.NewLine);
            var time = int.Parse(lines[0]);
            return lines[1].Split(',')
                .Where(x=>x != "x")
                .Select(int.Parse)
                .Select(x => (busId: x, timeUntilNextDeparture: (time / x + 1) * x  - time))
                .OrderBy(x => x.timeUntilNextDeparture)
                .First();


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