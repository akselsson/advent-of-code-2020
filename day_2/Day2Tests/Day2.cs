using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Schema;
using Xunit;

namespace Day2Tests
{
    public class Day2
    {
        private static readonly string[] ExampleData = {
            "1-3 a: abcde",
            "1-3 b: cdefg",
            "2-9 c: ccccccccc"
        };

        [Fact]
        public void Example()
        {
            Assert.Equal(CheckPasswordsAssigment1(ExampleData), new[] {true, false, true});
        }

        [Fact]
        public void Assignment1()
        {
            var input = File.ReadAllLines("input.txt");
            Assert.Equal(469, CheckPasswordsAssigment1(input).Count(x => x));
            Assert.Equal(531, CheckPasswordsAssigment1(input).Count(x => !x));
        }

        IEnumerable<bool> CheckPasswordsAssigment1(string[] lines)
        {
            foreach (var line in lines)
            {
                var passwordData = PasswordFileLine.Parse(line);
                var characterCount = passwordData.Password.Count(x => x == passwordData.Character);
                yield return characterCount >= passwordData.Min && characterCount <= passwordData.Max;
            }
        }

        [Fact]
        public void Eample2()
        {
            Assert.Equal(CheckPasswordsAssigment2(ExampleData), new[] {true, false, false});
        }

        [Fact]
        public void Assignment2()
        {
            var input = File.ReadAllLines("input.txt");
            Assert.Equal(267, CheckPasswordsAssigment2(input).Count(x => x));
            Assert.Equal(733, CheckPasswordsAssigment2(input).Count(x => !x));
        }

        

        IEnumerable<bool> CheckPasswordsAssigment2(string[] lines)
        {
            foreach (var line in lines)
            {
                var passwordData = PasswordFileLine.Parse(line);
                var charsAtPosition = new[]
                {
                    passwordData.Password[passwordData.Min - 1], 
                    passwordData.Password[passwordData.Max - 1]
                };
                yield return charsAtPosition.Count(x => x == passwordData.Character) == 1;
            }
        }
        
        public class PasswordFileLine
        {
            public int Min { get; private set; }
            public int Max { get; private set; }
            public char Character { get; private set; }
            public string Password { get; private set; }

            public static PasswordFileLine Parse(string line)
            {
                var ruleRegex = new Regex("(\\d+)-(\\d+) (\\w): (.*)");
                var result = ruleRegex.Match(line);
                return new PasswordFileLine
                {
                    Min = int.Parse(result.Groups[1].Value),
                    Max = int.Parse(result.Groups[2].Value),
                    Character = result.Groups[3].Value[0],
                    Password = result.Groups[4].Value
                };
            }
        }
    }
}