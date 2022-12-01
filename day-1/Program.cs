using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main()
    {
        await Method1();
        await Method2();
    }

    /// <summary>
    /// "Naive" approach to the problem.
    /// </summary>
    private static async ValueTask Method1()
    {
        var contents = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);

        var elves = new List<int>();
        var currentCalories = 0;

        foreach (var line in contents)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                elves.Add(currentCalories);
                currentCalories = 0;
                continue;
            }

            var calories = int.Parse(line);
            currentCalories += calories;
        }

        // sort ascending
        elves.Sort((a, b) => a.CompareTo(b));

        Console.WriteLine($"Elf with most Calories: {elves.Last()}");
        Console.WriteLine($"Top 3 Elves combined: {elves.TakeLast(3).Sum()}");
    }

    /// <summary>
    /// Using the power of LINQ.
    /// </summary>
    private static async ValueTask Method2()
    {
        var elves = (await File.ReadAllTextAsync("input.txt", Encoding.UTF8))
            .Split("\n\n")
            .Select(chunk => chunk
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(line => int.Parse(line))
                .Sum())
            .Order()
            .ToArray();

        Console.WriteLine($"Elf with most Calories: {elves.Last()}");
        Console.WriteLine($"Top 3 Elves combined: {elves.TakeLast(3).Sum()}");
    }
}
