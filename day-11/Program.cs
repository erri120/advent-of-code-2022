using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Day11;

[DebuggerDisplay("Monkey {ID}")]
public sealed class Monkey
{
    public readonly int ID;
    public readonly Queue<long> Items;
    public readonly Func<long, long> Operation;
    public readonly int DivisibleBy;
    public readonly int MonkeyIDIfTrue;
    public readonly int MonkeyIDIfFalse;

    public Monkey(int id, Queue<long> items, Func<long, long> operation, int divisibleBy, int monkeyIDIfTrue, int monkeyIDIfFalse)
    {
        ID = id;
        Items = items;
        DivisibleBy = divisibleBy;
        Operation = operation;
        MonkeyIDIfTrue = monkeyIDIfTrue;
        MonkeyIDIfFalse = monkeyIDIfFalse;
    }

    public static Monkey FromString(string text)
    {
        var lines = text.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        var id = int.Parse(lines[0][7..8]);

        var startingItems = lines[1][16..]
            .Split(',', StringSplitOptions.TrimEntries)
            .Select(long.Parse)
            .ToArray();

        var items = new Queue<long>(startingItems);

        var divisibleBy = int.Parse(lines[3][19..]);
        var operation = ParseOperation(lines[2]);
        var monkeyIDIfTrue = int.Parse(lines[4][25..]);
        var monkeyIDIfFalse = int.Parse(lines[5][26..]);

        return new Monkey(id, items, operation, divisibleBy, monkeyIDIfTrue, monkeyIDIfFalse);
    }

    private static Func<long, long> ParseOperation(string line)
    {
        var rawOperation = line[17..];

        var operationParts = rawOperation.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var parameter = Expression.Parameter(typeof(long), "old");

        Expression ParseOperand(string s)
        {
            if (s == "old") return parameter;
            return Expression.Constant(long.Parse(s));
        }

        var lhs = ParseOperand(operationParts[0]);
        var rhs = ParseOperand(operationParts[2]);

        var body = operationParts[1] switch
        {
            "+" => Expression.Add(lhs, rhs),
            "*" => Expression.Multiply(lhs, rhs),
            _ => throw new UnreachableException()
        };

        var expression = Expression.Lambda<Func<long, long>>(body, parameter);
        return expression.Compile();
    }
}

public static class Program
{
    public static async Task Main(string[] args)
    {
        var text = await File.ReadAllTextAsync("input.txt", Encoding.UTF8);
        RunPart(text, true);
        RunPart(text, false);
    }

    private static void RunPart(string text, bool isPartOne)
    {
        var monkeys = text
            .Split("\n\n")
            .Select(Monkey.FromString)
            .ToDictionary(monkey => monkey.ID, monkey => monkey);

        var product = monkeys.Select(pair => pair.Value.DivisibleBy).Aggregate((a, b) => a * b);

        var monkeyBusiness = Enumerable
            .Range(0, isPartOne ? 20 : 10000)
            .SelectMany(_ => monkeys
                .OrderBy(pair => pair.Key)
                .Select(pair =>
                {
                    var (monkeyID, monkey) = pair;
                    var numInspectedItems = (ulong)monkey.Items.Count;

                    while (monkey.Items.TryDequeue(out var worryLevel))
                    {
                        worryLevel = monkey.Operation(worryLevel);
                        worryLevel = isPartOne ? (long)Math.Floor(worryLevel / 3.0) : worryLevel % product;

                        var test = worryLevel % monkey.DivisibleBy == 0;
                        if (test)
                        {
                            monkeys[monkey.MonkeyIDIfTrue].Items.Enqueue(worryLevel);
                        }
                        else
                        {
                            monkeys[monkey.MonkeyIDIfFalse].Items.Enqueue(worryLevel);
                        }
                    }

                    return (monkeyID, numInspectedItems);
                }))
            .GroupBy(pair => pair.monkeyID, pair => pair.numInspectedItems)
            .Select(group => group.Aggregate((a, b) => a + b))
            .OrderDescending()
            .Take(2)
            .Aggregate((a, b) => a * b);

        Console.WriteLine($"Monkey Business: {monkeyBusiness}");
    }
}
