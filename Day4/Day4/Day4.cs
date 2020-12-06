using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Day4
{
    public class Day4
    {
        private static readonly string[] Example =
        {
            "ecl:gry pid:860033327 eyr:2020 hcl:#fffffd",
            "byr:1937 iyr:2017 cid:147 hgt:183cm",
            "",
            "iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884",
            "hcl:#cfa07d byr:1929",
            "",
            "hcl:#ae17e1 iyr:2013",
            "eyr:2024",
            "ecl:brn pid:760753108 byr:1931",
            "hgt:179cm",
            "",
            "hcl:#cfa07d eyr:2025 pid:166559648",
            "iyr:2011 ecl:brn hgt:59in"
        };

        private static readonly string[] Input = File.ReadAllLines("input.txt");


        [Test]
        public void Example1()
        {
            bool[] expected =
            {
                true,
                false,
                true,
                false
            };
            CollectionAssert.AreEqual(expected, Parse(Example).Select(x => x.IsValid).ToArray());
        }

        [Test]
        public void Assignment1()
        {
            Assert.AreEqual(295, Parse(Input).Count());
            Assert.AreEqual(264, Parse(Input).Count(x => x.IsValid));
        }

        [Test]
        public void Example2_Valid()
        {
            CollectionAssert.AreEquivalent(
                new[] {true, true, true, true},
                Parse(new[]
                {
                    "pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980",
                    "hcl:#623a2f",
                    "",
                    "eyr:2029 ecl:blu cid:129 byr:1989",
                    "iyr:2014 pid:896056539 hcl:#a97842 hgt:165cm",
                    "",
                    "hcl:#888785",
                    "hgt:164cm byr:2001 iyr:2015 cid:88",
                    "pid:545766238 ecl:hzl",
                    "eyr:2022",
                    "",
                    "iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719"
                }).Select(x => x.IsValid2));
        }

        [Test]
        public void Example2_InValid()
        {
            CollectionAssert.AreEquivalent(
                new[] {false, false, false, false},
                Parse(new[]
                {
                    "eyr:1972 cid:100",
                    "hcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926",
                    "",
                    "iyr:2019",
                    "hcl:#602927 eyr:1967 hgt:170cm",
                    "ecl:grn pid:012533040 byr:1946",
                    "",
                    "hcl:dab227 iyr:2012",
                    "ecl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277",
                    "",
                    "hgt:59cm ecl:zzz",
                    "eyr:2038 hcl:74454a iyr:2023",
                    "pid:3556412378 byr:2007"
                }).Select(x => x.IsValid2));
        }
        
        [Test]
        public void Assignment2()
        {
            Assert.AreEqual(295, Parse(Input).Count());
            Assert.AreEqual(224, Parse(Input).Count(x => x.IsValid2));
        }

        private IEnumerable<Passport> Parse(string[] lines)
        {
            return lines.Aggregate(
                    new List<List<string>> {new List<string>()},
                    (agg, line) =>
                    {
                        if (string.IsNullOrEmpty(line))
                        {
                            agg.Add(new List<string>());
                            return agg;
                        }

                        agg.Last().Add(line);
                        return agg;
                    })
                .Select(x => x.SelectMany(y => y.Split(' ')))
                .Select(Passport.Parse);
        }

        internal class Passport
        {
            public IDictionary<string, string> Entries { get; private set; }

            /*
            byr (Birth Year)
            iyr (Issue Year)
            eyr (Expiration Year)
            hgt (Height)
            hcl (Hair Color)
            ecl (Eye Color)
            pid (Passport ID)
            cid (Country ID)
            */

            private readonly IDictionary<string, bool> _entryRequired = new Dictionary<string, bool>
            {
                {"byr", true},
                {"iyr", true},
                {"eyr", true},
                {"hgt", true},
                {"hcl", true},
                {"ecl", true},
                {"pid", true},
                {"cid", false}
            };

            public bool IsValid
            {
                get
                {
                    return Entries.All(x => _entryRequired.ContainsKey(x.Key)) &&
                           _entryRequired.Where(y => y.Value).All(entry => Entries.ContainsKey(entry.Key));
                }
            }

            public bool IsValid2
            {
                get
                {
                    return Entries.All(x => _entryRequired.ContainsKey(x.Key)) &&
                           _entryRequired.Where(y => y.Value).All(entry => Entries.ContainsKey(entry.Key)) &&
                           Entries.All(x =>
                           {
                               var res =  Validate2(x.Key, x.Value);
                               if (!res)
                               {
                                   Console.WriteLine("Not Valid: " + x.Key + ":" + x.Value);
                               }

                               return res;
                           });
                }
            }

            private bool Validate2(string key, string value)
            {
                switch (key)
                {
                    case "byr":
                        return YearBetween(value, 1920, 2002);
                    case "iyr":
                        return YearBetween(value, 2010, 2020);
                    case "eyr":
                        return YearBetween(value, 2020, 2030);
                    case "hgt" when Regex.IsMatch(value, "^\\d*cm$"):
                        return ParseInt(value, 150, 193);
                    case "hgt" when Regex.IsMatch(value, "^\\d*in$"):
                        return ParseInt(value, 59, 76);
                    case "hcl":
                        return Regex.IsMatch(value, "^#[a-z0-9]{6}$");
                    case "ecl":
                        return new[] {"amb", "blu", "brn", "gry", "grn", "hzl", "oth"}.Contains(value);
                    case "pid":
                        return Regex.IsMatch(value, "^\\d{9}$");
                    case "cid":
                        return true;
                }
                return false;
            }

            private static bool ParseInt(string value, int @from, int to)
            {
                return int.TryParse(Regex.Match(value, "\\d*").Value, out var number) && number >= @from &&
                       number <= to;
            }

            private static bool YearBetween(string value, int @from, int to)
            {
                return value.Length == 4 && int.TryParse(value, out var val) && val >= @from && val <= to;
            }


            public static Passport Parse(IEnumerable<string> entries)
            {
                return new Passport
                {
                    Entries = entries.Select(x => x.Split(':')).ToDictionary(k => k[0], v => v[1])
                };
            }
        }
    }
}