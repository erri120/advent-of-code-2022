using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day1;

public static class Program
{
    public static async Task Main()
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
