using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
    
var lines = File.ReadAllLines("input.txt");
var numbers = lines.Select(int.Parse).ToArray();
FindPair(numbers);
FindTriplet(numbers);

static void FindTriplet(int[] numbers)
{
    for (int i = 0; i<numbers.Length; i++)
    {
        for (int j = i+1; j < numbers.Length; j++)
        {
            for (int k = j+1; k < numbers.Length; k++)
            {
                if (numbers[i] + numbers[j] + numbers[k] == 2020)
                {
                    Console.WriteLine(numbers[i] * numbers[j] * numbers[k]);
                    return;
                }
            }
        }
    }
}

static void FindPair(int[] numbers)
{
    for (int i = 0; i<numbers.Length; i++)
    {
        for (int j = i+1; j < numbers.Length; j++)
        {
            if (numbers[i] + numbers[j] == 2020)
            {
                Console.WriteLine(numbers[i] * numbers[j]);
                return;
            }
        }
    }
}