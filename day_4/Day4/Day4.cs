using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            CollectionAssert.AreEqual(expected, Parse(Example).Select(x=>x.IsValid).ToArray());
        }

        [Test]
        public void Assignment1()
        {
            Assert.AreEqual(295, Parse(Input).Count());
            Assert.AreEqual(264, Parse(Input).Count(x => x.IsValid));
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
            private IDictionary<string, string> _entries;

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
                    return _entries.All(x => _entryRequired.ContainsKey(x.Key)) &&
                           _entryRequired.Where(y => y.Value).All(entry => _entries.ContainsKey(entry.Key));
                }
            }

            public static Passport Parse(IEnumerable<string> entries)
            {
                return new Passport
                {
                    _entries = entries.Select(x => x.Split(':')).ToDictionary(k => k[0], v => v[1])
                };
            }
        }
    }
}