using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day3;

public static class Program
{
    public static async Task Main()
    {
        var lines = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);

        Console.WriteLine($"Part One: {PartOne(lines)}");
        Console.WriteLine($"Part Two: {PartTwo(lines)}");
    }

    private static int ToPriority(char c)
    {
        return c switch
        {
            >= 'a' and <= 'z' => c - 96,
            >= 'A' and <= 'Z' => c - 38,
            _ => throw new UnreachableException()
        };
    }

    private static int PartOne(IEnumerable<string> lines)
    {
        var summedPriorities = lines
            .Select(line =>
            {
                var firstCompartment = line[..(line.Length / 2)].ToHashSet();
                var secondCompartment = line[(line.Length / 2)..].ToHashSet();

                var intersection = firstCompartment.Intersect(secondCompartment).ToArray();
                var priorities = intersection
                    .Select(ToPriority)
                    .First();

                return priorities;
            }).Sum();

        return summedPriorities;
    }

    private static int PartTwo(IEnumerable<string> lines)
    {
        var result = lines
            .Chunk(3)
            .Select(chunk => chunk
                .Select(line => line.ToHashSet())
                .Aggregate((accumulate, next) =>
                {
                    accumulate.IntersectWith(next);
                    return accumulate;
                })
                .Select(ToPriority)
                .First()
            ).Sum();

        return result;
    }
}
