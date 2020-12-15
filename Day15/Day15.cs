using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day15
{
    public class Day15
    {
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
        public void Example1_2()
        {
            CollectionAssert.AreEqual(new[]{0,3,6,0,3,3,1,0,4,0},Play("0,3,6").Take(10));
        }
        
        [Test]
        public void Part1()
        {
            Assert.AreEqual(639,Play("11,18,0,20,1,7,16").Skip(2020-1).First());
        }
        
        [Test]
        public void Example2()
        {
            Assert.AreEqual(175594,Play("0,3,6").Skip(30_000_000-1).First());
        }

        [Test]
        public void Part2()
        {
            Assert.AreEqual(266,Play("11,18,0,20,1,7,16").Skip(30_000_000-1).First());
        }

        private IEnumerable<int> Play(string input)
        {
            var inputList = input.Split(",").Select(int.Parse).ToList();
            
            var spokenNumbers = new Dictionary<int, Queue<int>>();
            var lastNumber =0;
            var count = 0;
            
            foreach (var number in inputList)
            {
                yield return number;
                lastNumber = number;
                spokenNumbers[lastNumber] = new Queue<int>(new[] {count});
                count++;
            }
            
            while (true)
            {
                int currentNumber;
                if (spokenNumbers.TryGetValue(lastNumber, out var speaks))
                {
                    if (speaks.Count == 1)
                    {
                        currentNumber = 0;
                    }
                    else
                    {
                        currentNumber = count - speaks.Dequeue() - 1;
                    }
                }
                else
                {
                    currentNumber = 0;
                }
                
                yield return currentNumber;

                AddToHistory(currentNumber,count);

                lastNumber = currentNumber;
                count++;

            }
            
            void AddToHistory(int currentNumber, int turns)
            {
                if (spokenNumbers.TryGetValue(currentNumber, out var prev))
                {
                    prev.Enqueue(turns);
                }
                else
                {
                    spokenNumbers[currentNumber] = new Queue<int>(new[] {turns});
                }
            }
        }
    }
}