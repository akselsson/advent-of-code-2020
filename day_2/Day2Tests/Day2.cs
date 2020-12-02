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
        [Fact]
        public void Example()
        {
            var input = new string[]{
                "1-3 a: abcde",
"1-3 b: cdefg",
"2-9 c: ccccccccc"};

            Assert.Equal(CheckPasswordsAssigment1(input),new[]{true,false,true});
        }

        [Fact]
        public void Assignment1()
        {
            var input = File.ReadAllLines("input.txt");
            Assert.Equal(469,CheckPasswordsAssigment1(input).Count(x=>x));
            Assert.Equal(531,CheckPasswordsAssigment1(input).Count(x=>!x));
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
            var input = new string[]{
                "1-3 a: abcde",
                "1-3 b: cdefg",
                "2-9 c: ccccccccc"};
            Assert.Equal(CheckPasswordsAssigment2(input),new[]{true,false,false});
        }
        
        [Fact]
        public void Assignment2()
        {
            var input = File.ReadAllLines("input.txt");
            Assert.Equal(267,CheckPasswordsAssigment2(input).Count(x=>x));
            Assert.Equal(733,CheckPasswordsAssigment2(input).Count(x=>!x));
        }

        public class PasswordFileLine
        {
            public int Min { get; }
            public int Max { get; }
            public char Character { get; }
            public string Password { get; }

            public PasswordFileLine(int min, int max, char character, string password)
            {
                Min = min;
                Max = max;
                Character = character;
                Password = password;
            }

            public static PasswordFileLine Parse(string line)
            {
                var ruleRegex = new Regex("(\\d+)-(\\d+) (\\w): (.*)");
                var result = ruleRegex.Match(line);
                var passwordData = new PasswordFileLine(int.Parse(result.Groups[1].Value), int.Parse(result.Groups[2].Value),
                    result.Groups[3].Value[0], result.Groups[4].Value);
                return passwordData;
            }
        }

        IEnumerable<bool> CheckPasswordsAssigment2(string[] lines)
        {
            foreach (var line in lines)
            {
                var passwordData = PasswordFileLine.Parse(line);
                var charsAtPosition = new[]
                    {passwordData.Password[passwordData.Min - 1], passwordData.Password[passwordData.Max - 1]};
                yield return charsAtPosition.Count(x => x == passwordData.Character) == 1;
            }
        }
    }
}