using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day15
{
    public class Day15
    {
        private readonly string Example =
            @"";

        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            Assert.AreEqual(436,Play("0,3,6").Skip(2020-1).First());
        }
        [Test]
        public void Example1_1()
        {
            Assert.AreEqual(1,Play("1,3,2").Skip(2020-1).First());
            Assert.AreEqual(10,Play("2,1,3").Skip(2020-1).First());
            Assert.AreEqual(27,Play("1,2,3").Skip(2020-1).First());
            Assert.AreEqual(78,Play("2,3,1").Skip(2020-1).First());
            Assert.AreEqual(438,Play("3,2,1").Skip(2020-1).First());
            Assert.AreEqual(1836,Play("3,1,2").Skip(2020-1).First());
        }
        
        [Test]
        public void Part1()
        {
            Assert.AreEqual(639,Play("11,18,0,20,1,7,16").Skip(2020-1).First());
        }

        private IEnumerable<int> Play(string input)
        {
            var spokenNumbers = input.Split(",").Select(int.Parse).ToList();
            foreach (var number in spokenNumbers)
            {
                yield return number;
            }
            int turn = 0;
            while (true)
            {
                
                var lastNumber = spokenNumbers.Last();
                var previousSpeakOfNumber = spokenNumbers.LastIndexOf(lastNumber, spokenNumbers.Count-2);
                var currentNumber = previousSpeakOfNumber == -1 ? 0 : spokenNumbers.Count - previousSpeakOfNumber - 1;
                spokenNumbers.Add(currentNumber);
                
                yield return currentNumber;

            }
        }

        

        [Test]
        public void Example2()
        {
            //Assert.AreEqual(175594,Play("0,3,6").Skip(30_000_000-1).First());
        }

        [Test]
        public void Part2()
        {
        }
    }
}