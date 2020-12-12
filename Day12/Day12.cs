using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day12
{
    public class Day12
    {
        private readonly string Example =
            @"F10
N3
F7
R90
F11";

        private readonly string Input = File.ReadAllText("input.txt");
        

        [Test]
        public void Example1()
        {
            var startingPosition = (x:0,y:0,direction:'E');
            var newPosition = Move(Example, startingPosition);
            Assert.AreEqual(25,Distance(startingPosition,newPosition));
        }
        
        

        private int Distance((int x, int y, char direction) pos1, (int x, int y, char direction) pos2)
        {
            return Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.x-pos2.y);
        }

        private (int x, int y, char direction) Move(string input, (int x, int y, char direction) startingPosition)
        {
            return input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => (direction: x[0], distance: int.Parse(x.Substring(1))))
                .Aggregate(startingPosition, Move
                );
        }

        (int x, int y, char direction) Move((int x, int y, char direction) startingPosition,
            (char direction, int distance) direction)
        {
            var directions = new Dictionary<char,int>()
            {
                {'N', 0},
                {'E', 90},
                {'S', 180},
                {'W', 270}
            };

            var turn = new Dictionary<char, int>()
            {
                {'L', -1},
                {'R', 1}
            };

            if (direction.direction == 'F')
            {
                return Move(startingPosition, (startingPosition.direction, direction.distance));
            }

            if (directions.TryGetValue(direction.direction, out var angle))
            {
                return (
                    startingPosition.x + (int) Math.Cos(angle * Math.PI / 180) * direction.distance,
                    startingPosition.y + (int) Math.Sin(angle * Math.PI / 180) * direction.distance,
                    startingPosition.direction);
            }

            if (turn.TryGetValue(direction.direction, out var angleModifier))
            {
                return (
                    startingPosition.x, 
                    startingPosition.y,
                    directions.Single(x => x.Value == (direction.distance + angleModifier * direction.distance) % 360).Key);
            }
            

            return startingPosition;

        }

        [Test]
        public void Part1()
        {
            var startingPosition = (x:0,y:0,direction:'E');
            var newPosition = Move(Input, startingPosition);
            Assert.AreEqual(1137,Distance(startingPosition,newPosition));
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