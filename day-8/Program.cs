using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day8;

public static class Program
{
    public static async Task Main()
    {
        var lines = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);
        var grid = CreateGrid(lines);

        PartOne(grid);
        PartTwo(grid);
    }

    private static int[][] CreateGrid(IEnumerable<string> lines)
    {
        return lines
            .Select(line => line
                .Select(c => int.Parse($"{c}"))
                .ToArray())
            .ToArray();
    }

    private static IEnumerable<int> ValuesLeft(int[] row, int x)
    {
        return row
            // take all values left of the current position in the row
            .TakeWhile((_, x2) => x2 < x)
            // reverse the sequence, so the first element is the first value to the left
            .Reverse();
    }

    private static IEnumerable<int> ValuesRight(int[] row, int x)
    {
        return row
            // skip to the first value after the current one
            .Skip(x + 1);
    }

    private static IEnumerable<int> ValuesUp(int[][] grid, int x, int y)
    {
        return grid
            // take all rows above the current one
            .TakeWhile((_, y2) => y2 < y)
            // select only the values from the same column
            .Select(row2 => row2[x])
            // reverse the sequence, so the first element is the first value up
            .Reverse();
    }

    private static IEnumerable<int> ValuesDown(int[][] grid, int x, int y)
    {
        return grid
            // skip to the first row after the current one
            .Skip(y + 1)
            // select only the values from the same column
            .Select(row2 => row2[x]);
    }

    private static IEnumerable<(int, int)> IterateGrid(int[][] grid)
    {
        for (var y = 0; y < grid.Length; y++)
        {
            var row = grid[y];
            for (var x = 0; x < row.Length; x++)
            {
                yield return (x, y);
            }
        }
    }

    private static IEnumerable<TSource> TakeUntil<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        foreach (var element in source)
        {
            yield return element;

            if (!predicate(element))
            {
                break;
            }
        }
    }

    private static void PartOne(int[][] grid)
    {
        var totalVisible = IterateGrid(grid)
            .Count(tuple =>
            {
                var (x, y) = tuple;
                var row = grid[y];
                var value = row[x];

                var isVisibleFromTheLeft = ValuesLeft(row, x).All(other => other < value);
                if (isVisibleFromTheLeft) return true;

                var isVisibleFromTheRight = ValuesRight(row, x).All(other => other < value);
                if (isVisibleFromTheRight) return true;

                var isVisibleFromTheTop = ValuesUp(grid, x, y).All(other => other < value);
                if (isVisibleFromTheTop) return true;

                var isVisibleFromTheBottom = ValuesDown(grid, x, y).All(other => other < value);
                if (isVisibleFromTheBottom) return true;

                return false;
            });

        Console.WriteLine($"Total Visible: {totalVisible}");
    }

    private static void PartTwo(int[][] grid)
    {
        var highestScenicScore = IterateGrid(grid)
            .Select(tuple =>
            {
                var (x, y) = tuple;
                var row = grid[y];

                var value = row[x];
                if (value == 0) return 0;

                var treesVisibleLeft = ValuesLeft(row, x).TakeUntil(left => left < value).Count();
                var treesVisibleRight = ValuesRight(row, x).TakeUntil(right => right < value).Count();
                var treesVisibleUp = ValuesUp(grid, x, y).TakeUntil(up => up < value).Count();
                var treesVisibleDown = ValuesDown(grid, x, y).TakeUntil(down => down < value).Count();

                var scenicScore = treesVisibleLeft * treesVisibleRight * treesVisibleUp * treesVisibleDown;
                return scenicScore;
            })
            .Max();

        Console.WriteLine($"Highest Scenic Score: {highestScenicScore}");
    }
}
