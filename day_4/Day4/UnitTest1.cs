using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;

namespace Day4
{
    public class Tests
    {
    
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

    private static string[] Example = {
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
    
        [Test]
        public void Test1()
        {
            bool[] expected =
            {
                true,
                false,
                true,
                false,
            };
            CollectionAssert.AreEqual(expected,Validate(Example).ToArray());
        }

        private IEnumerable<bool> Validate(string[] passport)
        {
            var passports = passport.Aggregate(new
            {
                Current = new List<string>(),
                Previous = new List<List<string>>()
            }, (agg, line) =>
            {
                if (string.IsNullOrEmpty(line))
                {
                    return new
                    {
                        Current = new List<string>(),
                        Previous = agg.Previous.Concat(new[] {agg.Current}).ToList()
                    };
                }
                else
                {
                    agg.Current.Add(line);
                    return agg;
                }
            });
            var passp = passports.Previous.Concat(new[] {passports.Current});
            return passp
                .Select(x => x.SelectMany(y => y.Split(' ')))
                .Select(Passport.Parse).Select(x=>x.IsValid);

        }
    }

    internal class Passport
    {
        private IDictionary<string, bool> EntryRequired = new Dictionary<string, bool>
        {
            {"byr", true},
            {"iyr", true},
            {"eyr", true},
            {"hgt", true},
            {"hcl", true},
            {"ecl", true},
            {"pid", true},
            {"cid", false},
        };
        private IDictionary<string, string> _entries;
        public static Passport Parse(IEnumerable<string> entries)
        {
            return new Passport()
            {
                _entries = entries.Select(x=>x.Split(':')).ToDictionary(k=>k[0],v=>v[1])
            };
        }

        public bool IsValid
        {
            get
            {
                return _entries.All(x => EntryRequired.ContainsKey(x.Key)) &&
                       EntryRequired.Where(y => y.Value).All(entry => _entries.ContainsKey(entry.Key));
            }
        }
    }
}