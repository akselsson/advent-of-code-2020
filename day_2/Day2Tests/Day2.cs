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
                var ruleRegex = new Regex("(\\d+)-(\\d+) (\\w): (.*)");
                var result = ruleRegex.Match(line);
                var passwordData = new
                {
                    Min = int.Parse(result.Groups[1].Value),
                    Max = int.Parse(result.Groups[2].Value),
                    Character = result.Groups[3].Value[0],
                    Password = result.Groups[4].Value
                };
                var characterCount = passwordData.Password.Count(x => x == passwordData.Character);
                yield return characterCount >= passwordData.Min && characterCount <= passwordData.Max;
            }
        }
    }
}