using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace day_1
{
    class Program
    {
        async static Task Main(string[] args)
        {
            var lines = await File.ReadAllLinesAsync("input.txt");
            var numbers = lines.Select(int.Parse).ToArray();
            FindPair(numbers);
            FindTriplet(numbers);
        }

        private static void FindTriplet(int[] numbers)
        {
            for (int i = 0; i < numbers.Length - 2; i++)
            {
                for (int j = 1; j < numbers.Length -1 ; j++)
                {
                    for (int k = 2; k < numbers.Length; k++)
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

        private static void FindPair(int[] numbers)
        {
            for (int i = 0; i < numbers.Length - 1; i++)
            {
                for (int j = 1; j < numbers.Length; j++)
                {
                    if (numbers[i] + numbers[j] == 2020)
                    {
                        Console.WriteLine(numbers[i] * numbers[j]);
                        return;
                    }
                }
            }
        }
    }
}