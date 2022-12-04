using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day4;

public readonly struct AssignmentRange
{
    public readonly int Start;
    public readonly int End;

    public AssignmentRange(int start, int end)
    {
        Start = start;
        End = end;
    }

    public static AssignmentRange FromString(string s)
    {
        var dashIndex = s.IndexOf('-');
        var startIndex = int.Parse(s[..dashIndex]);
        var endIndex = int.Parse(s[(dashIndex + 1)..]);

        return new AssignmentRange(startIndex, endIndex);
    }

    public bool ContainsOther(AssignmentRange other)
    {
        return Start <= other.Start && End >= other.End;
    }

    public override string ToString()
    {
        return $"{Start}..{End}";
    }
}

public static class Program
{
    public static async Task Main()
    {
        var lines = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);

        Console.WriteLine($"Number of pairs with assignment ranges that fully contain the other: {PartOne(lines)}");
        Console.WriteLine($"Number of pairs with assignment ranges that overlap: {PartTwo(lines)}");
    }

    private static IEnumerable<(AssignmentRange, AssignmentRange)> ToRanges(this IEnumerable<string> lines)
    {
        return lines.Select(line =>
        {
            var commaIndex = line.IndexOf(',');
            var firstPart = line[..commaIndex];
            var secondPart = line[(commaIndex + 1)..];

            var first = AssignmentRange.FromString(firstPart);
            var second = AssignmentRange.FromString(secondPart);

            return (first, second);
        });
    }

    private static int PartOne(IEnumerable<string> lines)
    {
        var count = lines.ToRanges()
            .Select(tuple =>
            {
                var (first, second) = tuple;
                return first.ContainsOther(second) ||
                       second.ContainsOther(first);
            }).Count(x => x);

        return count;
    }

    private static bool RangesOverlap(AssignmentRange a, AssignmentRange b)
    {
        return a.Start <= b.End && b.Start <= a.End;
    }

    private static int PartTwo(IEnumerable<string> lines)
    {
        var count = lines.ToRanges()
            .Select(tuple =>
            {
                var (first, second) = tuple;
                return RangesOverlap(first, second);
            }).Count(x => x);

        return count;
    }
}
