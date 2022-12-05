using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day5;

public readonly struct RearrangementProcedure
{
    private static readonly Regex ParseRegex = new(@"move (?<count>\d+) from (?<from>\d+) to (?<to>\d+)",
        RegexOptions.Compiled, TimeSpan.FromMilliseconds(500));

    public readonly int Count;
    public readonly int From;
    public readonly int To;

    public RearrangementProcedure(int count, int from, int to)
    {
        Count = count;
        From = from;
        To = to;
    }

    public static RearrangementProcedure FromString(string line)
    {
        var match = ParseRegex.Match(line);
        if (!match.Success) throw new ArgumentException($"Invalid argument: {line}", nameof(line));

        var sCount = match.Groups["count"].Value;
        var sFrom = match.Groups["from"].Value;
        var sTo = match.Groups["to"].Value;

        return new RearrangementProcedure(int.Parse(sCount), int.Parse(sFrom), int.Parse(sTo));
    }

    public override string ToString()
    {
        return $"move {Count} from {From} to {To}";
    }
}

public static class Program
{
    public static async Task Main()
    {
        var input = await File.ReadAllTextAsync("input.txt", Encoding.UTF8);

        var stacksPartOne = RunPart(input, true);
        var stacksPartTwo = RunPart(input, false);

        Console.WriteLine($"Part One: {CreateMessage(stacksPartOne)}");
        Console.WriteLine($"Part Two: {CreateMessage(stacksPartTwo)}");
    }

    private static string CreateMessage(IReadOnlyDictionary<int, Stack<char>> stacks)
    {
        return stacks
            .Select(kv => kv.Value)
            .Select(stack => stack.First())
            .Aggregate("", (a, b) => $"{a}{b}");
    }

    private static IReadOnlyDictionary<int, Stack<char>> RunPart(string input, bool isPartOne)
    {
        var split = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

        var stacks = CreateStacks(split[0]);
        var procedures = CreateRearrangementProcedures(split[1]);

        ApplyProceduresOnStacks(stacks, procedures, isPartOne);
        return stacks;
    }

    private static Dictionary<int, Stack<char>> CreateStacks(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var stackIDLine = lines.Last();

        var stackIDs = stackIDLine
            .Where(char.IsAsciiDigit)
            .Select(c => int.Parse($"{c}"))
            .ToArray();

        var stacks = stackIDs.ToDictionary(stackID => stackID, _ => new Stack<char>(lines.Length - 1));

        var enumerable = lines.Take(lines.Length - 1).Reverse();

        foreach (var line in enumerable)
        {
            var i = 0;

            foreach (var stackID in stackIDs)
            {
                var crate = line[i + 1];
                i += 4;

                if (crate == ' ') continue;
                stacks[stackID].Push(crate);
            }
        }

        return stacks;
    }

    private static IEnumerable<RearrangementProcedure> CreateRearrangementProcedures(string input)
    {
        return input.Split('\n')
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(RearrangementProcedure.FromString)
            .ToArray();
    }

    private static void ApplyProceduresOnStacks(
        IReadOnlyDictionary<int, Stack<char>> stacks,
        IEnumerable<RearrangementProcedure> procedures,
        bool isPartOne)
    {
        foreach (var procedure in procedures)
        {
            var fromStack = stacks[procedure.From];
            var toStack = stacks[procedure.To];
            var count = procedure.Count;

            var enumerable = isPartOne ? fromStack.Take(count).ToArray() : fromStack.Take(count).Reverse().ToArray();

            foreach (var value in enumerable)
            {
                fromStack.Pop();
                toStack.Push(value);
            }
        }
    }
}
