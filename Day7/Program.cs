using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks.Sources;

var file = args[0];
var input = File.ReadAllLines(file);

var containerPattern = new Regex(@"^(?<container_colour>[a-z ]*?) bags?");
var contentPattern = new Regex(@"(?<count>\d+) (?<colour>[a-z ]*?) bags?");
var rules = input.Select(Extract).ToArray();

Console.WriteLine($"Part 1: {rules.Count(x => CanContain("shiny gold", x))}");
Console.WriteLine($"Part 2: {CountBags("shiny gold",0)}");

// Part 1: 238
// Part 2: 82930

bool CanContain(string colour, BagRule rule)
{

    if (rule.Contents.Any(x => x.Colour == colour))
    {
        return true;
    }

    return rule.Contents.Any(x =>
        rules.Where(y => y.Colour == x.Colour)
            .Any(y => CanContain(colour, y)));

}

int CountBags(string colour, int depth)
{
    var rule = rules.FirstOrDefault(x => x.Colour == colour);
    if (rule == null)
    {
        return 0;
    }

    var self = depth == 0 ? 0 : 1;
    return self + rule.Contents.Sum(x => x.Count * CountBags(x.Colour, depth + 1));
}



BagRule Extract(string line)
{
    var match = containerPattern.Match(line);
    if (!match.Success)
    {
        throw new Exception("Failed to parse " + line);
    }

    var colour = match.Groups["container_colour"].Value;


    return new BagRule
    {
        Colour = colour,
        Input = line,
        Contents = contentPattern.Matches(line).Select(x=> (x.Groups["colour"].Value, int.Parse(x.Groups["count"].Value))).ToList()
    };
}

record BagRule
{
    public string Colour { get; init; }
    public List<(string Colour, int Count)> Contents { get; init; } = new();
    public string Input { get; init; }
}

