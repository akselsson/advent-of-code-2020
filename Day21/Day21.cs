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
        
        [Test]
        public void Example2()
        {
            Assert.AreEqual("mxmxvkd,sqjhc,fvjkl",DangerousIngredients(Example));
        }
        
        [Test]
        public void Part2()
        {
            Assert.AreEqual("vrzkz,zjsh,hphcb,mbdksj,vzzxl,ctmzsr,rkzqs,zmhnj",DangerousIngredients(Input));
        }

        private string DangerousIngredients(string input)
        {
            var foods = ParseFoods(input);
            var allergenToIngredients = AllergenToIngredients(foods);
            var ingredientToAllergens = allergenToIngredients
                .SelectMany(x => x.Value.Select(v => (allergen: x.Key, ingredient: v)))
                .GroupBy(x => x.ingredient)
                .Select(x => (ingredient: x.Key, allergens: x.Select(y => y.allergen).ToHashSet())).ToArray();
            foreach (var ingredient in ingredientToAllergens)
            {
                Console.WriteLine($"{ingredient.ingredient}: {string.Join(",",ingredient.allergens)}");
            }

            while (ingredientToAllergens.Any(x => x.allergens.Count > 1))
            {
                var toTrim = ingredientToAllergens.Where(x => x.allergens.Count > 1).ToArray();
                var singleAllergen = ingredientToAllergens.Where(x => x.allergens.Count == 1).ToArray();
                var allergensToRemove = singleAllergen.SelectMany(x => x.allergens).ToHashSet();
                ingredientToAllergens = singleAllergen.Concat(
                    toTrim.Select(x=>(x.ingredient,x.allergens.Except(allergensToRemove).ToHashSet())))
                    .ToArray();
            }

            return string.Join(",",ingredientToAllergens.OrderBy(x=>x.allergens.First()).Select(x=>x.ingredient));
        }

        private int CountAllergens(string input)
        {
            var foods = ParseFoods(input);
            var allergenToIngredients = AllergenToIngredients(foods);

            foreach (var possibleMatch in allergenToIngredients)
            {
                Console.WriteLine($"{possibleMatch.Key}: {string.Join(",",possibleMatch.Value)}");
            }

            var definiteNonAllergens = foods.SelectMany(x => x.ingredients).Distinct().Except(allergenToIngredients.Values.SelectMany(x=>x).Distinct()).ToHashSet();
            return foods.SelectMany(f => f.ingredients).Count(x => definiteNonAllergens.Contains(x));

        }

        private static IDictionary<string, string[]> AllergenToIngredients((string[] ingredients, string[] allergens)[] foods)
        {
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

            return possibleMatches;
        }

        private static (string[] ingredients, string[] allergens)[] ParseFoods(string input)
        {
            var foods = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(line =>
                {
                    var parts = line.TrimEnd(')').Split('(', StringSplitOptions.RemoveEmptyEntries);
                    var ingredients = parts[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var allergens = parts[1].Replace("contains", "").Split(',', StringSplitOptions.TrimEntries);
                    return (ingredients, allergens);
                }).ToArray();
            return foods;
        }


        

        
    }
}