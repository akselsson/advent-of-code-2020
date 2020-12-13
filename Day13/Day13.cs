using System;
using System.Diagnostics;
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
            var lines = Example.Split(Environment.NewLine);
            var timestamp = EarliestTimestampWithMatchingOffsets(lines[1]);
            Assert.AreEqual(1068781,timestamp);
        }
        
        [Test]
        public void Example2_2()
        {
            Assert.AreEqual(1261476,EarliestTimestampWithMatchingOffsets("67,7,x,59,61"));
            Assert.AreEqual(1202161486,EarliestTimestampWithMatchingOffsets("1789,37,47,1889"));
        }
        [Test]
        public void Part2()
        {
            // This finishes in about 10 hours on a 2018 MBP wihtout startValue
            // startValue 149491899999098 finishes after 8 hours
            var lines = Input.Split(Environment.NewLine);
            var timestamp = EarliestTimestampWithMatchingOffsets(lines[1],1118684865113056);
            Assert.AreEqual(1118684865113056,timestamp);
        }

        private static long EarliestTimestampWithMatchingOffsets(string input, long? startValue = null)
        {
            var buses = input.Split(',')
                .Select((x, i) =>
                    x != "x" ? (int.Parse(x), i) : ((int?) null, i))
                .Where(x => x.Item1.HasValue)
                .Select(x => (busId: x.Item1.Value, offset: x.Item2)).OrderByDescending(x=>x.busId).ToArray();
            var largestBusId = buses.OrderByDescending(x => x.busId).First();
            var busesToTest = buses.Skip(1).ToArray();
            File.Delete("out.txt");
            long a= 1;
            Stopwatch timer = Stopwatch.StartNew();
            long start = startValue ?? largestBusId.busId-largestBusId.offset; 
            for (long i = start; i < long.MaxValue - largestBusId.busId; i += (long)largestBusId.busId)
            {
                if (a++ % 100000000 == 0)
                {
                    File.AppendAllText("out.txt", $"{i} {long.MaxValue-i} {(a)/ timer.ElapsedMilliseconds } iterations/s {Environment.NewLine}");
                }

                if (NewMethod(busesToTest, i))
                {
                    return i;
                }
            }

            throw new Exception("No matching times");
        }

        private static bool NewMethod((int busId, int offset)[] buses, long i)
        {
            for (int j = 0; j < buses.Length; j++)
            {
                if ((i + buses[j].offset) % buses[j].busId != 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}