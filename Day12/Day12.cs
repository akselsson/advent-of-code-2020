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
            var startingPosition = (x: 0, y: 0, direction: 'E');
            var newPosition = MoveAll(Example, startingPosition);
            Assert.AreEqual(25, Distance(startingPosition, newPosition));
        }

        [Test]
        public void Part1_CanRotate()
        {
            var startingPosition = (x: 0, y: 0, direction: 'N');
            var newPosition = Move(startingPosition, ('L', 90));
            Assert.AreEqual((0, 0, 'W'), newPosition);
        }

        [Test]
        public void Part1()
        {
            var startingPosition = (x: 0, y: 0, direction: 'E');
            var newPosition = MoveAll(Input, startingPosition);
            Assert.AreEqual(521, Distance(startingPosition, newPosition));
        }


        private int Distance((int x, int y, char direction) pos1, (int x, int y, char direction) pos2)
        {
            return Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y);
        }

        private (int x, int y, char direction) MoveAll(string input, (int x, int y, char direction) startingPosition)
        {
            return input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => (direction: x[0], distance: int.Parse(x.Substring(1))))
                .Aggregate(startingPosition, (tuple, valueTuple) =>
                {
                    var newPosition = Move(tuple, valueTuple);
                    Console.WriteLine($"{tuple} {valueTuple} {newPosition}");
                    return newPosition;
                });
        }

        (int x, int y, char direction) Move((int x, int y, char direction) startingPosition,
            (char direction, int distance) direction)
        {
            var directions = new Dictionary<char, int>()
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
                Console.WriteLine(angleModifier * direction.distance);
                return (
                    startingPosition.x,
                    startingPosition.y,
                    directions.Single(x =>
                        x.Value == (directions[startingPosition.direction] + angleModifier * direction.distance + 360) %
                        360).Key);
            }


            return startingPosition;
        }

        [Test]
        public void Example2()
        {
            var startingPosition = new ShipWithWaypoint(new Position(0, 0), new Position(1, 10));
            var newPosition = MoveAllWithWaypoint(Example, startingPosition);
            Assert.AreEqual(286, Distance(startingPosition.Ship, newPosition.Ship));
        }
        
        [Test]
        public void Part2()
        {
            var startingPosition = new ShipWithWaypoint(new Position(0, 0), new Position(1, 10));
            var newPosition = MoveAllWithWaypoint(Input, startingPosition);
            Assert.AreEqual(33420, Distance(startingPosition.Ship, newPosition.Ship));
        }

        private ShipWithWaypoint MoveAllWithWaypoint(string example, ShipWithWaypoint startingPosition)
        {
            return example.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new Action(x[0], int.Parse(x.Substring(1))))
                .Aggregate(startingPosition, (waypoint, action) =>
                {
                    var result = Move(waypoint, action);
                    Console.WriteLine($"{waypoint} {action} {result}");
                    return result;
                });
        }

        private ShipWithWaypoint Move(ShipWithWaypoint current, Action action)
        {
            ShipWithWaypoint MoveWaypoint(Position delta, ShipWithWaypoint current)
            {
                return current with{
                    Waypoint = new Position(current.Waypoint.Y + delta.Y, current.Waypoint.X + delta.X)
                    };
            }

            ShipWithWaypoint RotateWaypointInternal(int unit, ShipWithWaypoint current)
            {
                return current with {
                    Waypoint = RotateWaypoint(unit, current.Waypoint)
                    };
            }
            var actions = new Dictionary<char, Func<ShipWithWaypoint, Action, ShipWithWaypoint>>()
            {
                {'N', (c, a) => MoveWaypoint(new Position(a.Unit,0),c)},
                {'S', (c, a) => MoveWaypoint(new Position(-a.Unit,0),c)},
                {'E', (c, a) => MoveWaypoint(new Position(0, a.Unit),c)},
                {'W', (c, a) => MoveWaypoint(new Position(0, -a.Unit),c)},
                {'L', (c, a) => RotateWaypointInternal(a.Unit,c)},
                {'R', (c, a) => RotateWaypointInternal(-a.Unit,c)},
                {'F', (c,a) => MoveToWayPoint(a.Unit,c)}
            };
            return actions[action.Direction](current, action);
        }

        private ShipWithWaypoint MoveToWayPoint(in int unit, ShipWithWaypoint shipWithWaypoint)
        {
            return shipWithWaypoint with {
                Ship = new(
                    X: shipWithWaypoint.Waypoint.X * unit + shipWithWaypoint.Ship.X,
                    Y: shipWithWaypoint.Waypoint.Y * unit + shipWithWaypoint.Ship.Y
                )
                };
        }

        [Test]
        public void TestRotateWaypoint()
        {
            Assert.AreEqual(new Position(0,1),RotateWaypoint(90,new Position(1,0)));
            Assert.AreEqual(new Position(1,0),RotateWaypoint(180,new Position(-1,0)));
            Assert.AreEqual(new Position(-1,1),RotateWaypoint(90,new Position(1,1)));
            Assert.AreEqual(new Position(-10,4),RotateWaypoint(90,new Position(4,10)));
        }

        private Position RotateWaypoint(in int degrees, Position current)
        {
            return new(
                X: current.X * (int) Math.Cos(degrees * Math.PI / 180) + 
                   current.Y * (int) Math.Sin(degrees * Math.PI / 180),
                Y: current.X * -(int) Math.Sin(degrees * Math.PI / 180) + 
                   current.Y * (int) Math.Cos(degrees * Math.PI / 180)
                   
                
                
            );
        }


        private int Distance(Position pos1, Position pos2)
        {
            return Math.Abs(pos1.Y - pos2.Y) + Math.Abs(pos1.X - pos2.X);
        }

       

        record Position(int X, int Y);

        record ShipWithWaypoint(Position Ship, Position Waypoint);

        record Action(char Direction, int Unit);
    }
}