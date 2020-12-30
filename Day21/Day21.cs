using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Day21
{
    public class Day21
    {
        private readonly string Example =
            @"mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
trh fvjkl sbzzf mxmxvkd (contains dairy)
sqjhc fvjkl (contains soy)
sqjhc mxmxvkd sbzzf (contains fish)";

        private readonly string Input = File.ReadAllText("input.txt");

        [Test]
        public void Example1()
        {
            Assert.AreEqual(5,CountAllergens(Example));
        }
        
        [Test]
        public void Part1()
        {
            Assert.AreEqual(2282,CountAllergens(Input));
        }

        private int CountAllergens(string input)
        {
            var foods = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(line =>
                {
                    var parts = line.TrimEnd(')').Split('(', StringSplitOptions.RemoveEmptyEntries);
                    var ingredients = parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var allergens = parts[1].Replace("contains", "").Split(',', StringSplitOptions.TrimEntries);
                    return (ingredients, allergens);
                }).ToArray();
            IDictionary<string, string[]> possibleMatches = new Dictionary<string, string[]>();
            foreach (var food in foods)
            {
                foreach (var allergen in food.allergens)
                {
                    if (possibleMatches.TryGetValue(allergen, out var matchList))
                    {
                        possibleMatches[allergen] = matchList.Intersect(food.ingredients).ToArray();
                    }
                    else
                    {
                        possibleMatches[allergen] = food.ingredients;
                    }
                }
            }

            foreach (var possibleMatch in possibleMatches)
            {
                Console.WriteLine($"{possibleMatch.Key}: {string.Join(",",possibleMatch.Value)}");
            }

            var definiteNonAllergens = foods.SelectMany(x => x.ingredients).Distinct().Except(possibleMatches.Values.SelectMany(x=>x).Distinct()).ToHashSet();
            return foods.SelectMany(f => f.ingredients).Count(x => definiteNonAllergens.Contains(x));

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