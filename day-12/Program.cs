using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var lines = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);
        var grid = CreateGrid(lines, out var start, out var end);

        var partOne = Dijkstra(grid, start, end, true);
        var partTwo = Dijkstra(grid, end, Point.Empty, false);

        Console.WriteLine($"Part One - Steps needed: {partOne}");
        Console.WriteLine($"Part Two - Steps needed: {partTwo}");
    }

    private static int[][] CreateGrid(IEnumerable<string> lines, out Point start, out Point end)
    {
        var startPoint = Point.Empty;
        var endPoint = Point.Empty;

        var grid = lines.Select((line, y) =>
        {
            return line.Select((c, x) =>
            {
                switch (c)
                {
                    case 'S':
                        startPoint = new Point(x, y);
                        break;
                    case 'E':
                        endPoint = new Point(x, y);
                        break;
                }

                return c switch
                {
                    'S' => 'a',
                    'E' => 'z',
                    _ => (int)c
                };
            }).ToArray();
        }).ToArray();

        start = startPoint;
        end = endPoint;

        return grid;
    }

    private static int Dijkstra(int[][] grid, Point start, Point end, bool partOne)
    {
        var distances = new Dictionary<Point, int>();
        var queue = new PriorityQueue<Point, int>();

        queue.Enqueue(start, 0);

        while (queue.TryDequeue(out var current, out var currentDistance))
        {
            if (distances.ContainsKey(current)) continue;
            distances[current] = currentDistance;

            var foundEnd = false;
            void CheckNeighbor(Point neighbor)
            {
                var currentValue = grid[current.Y][current.X];
                var neighborValue = grid[neighbor.Y][neighbor.X];

                switch (partOne)
                {
                    case true when neighborValue - currentValue > 1:
                    case false when currentValue - neighborValue > 1:
                        return;
                }

                var newDistance = currentDistance + 1;

                if (partOne && neighbor == end || !partOne && neighborValue == 'a')
                {
                    distances[end] = newDistance;
                    foundEnd = true;
                    return;
                }

                queue.Enqueue(neighbor, newDistance);
            }

            if (GetLeft(current, out var left)) CheckNeighbor(left);
            if (GetRight(grid, current, out var right)) CheckNeighbor(right);
            if (GetUp(current, out var up)) CheckNeighbor(up);
            if (GetDown(grid, current, out var down)) CheckNeighbor(down);

            if (foundEnd) break;
        }

        return distances[end];
    }

    private static bool GetLeft(Point current, out Point left)
    {
        left = current with { X = current.X - 1 };
        return left.X >= 0;
    }

    private static bool GetRight(IReadOnlyList<int[]> grid, Point current, out Point right)
    {
        right = current with { X = current.X + 1 };
        return right.X < grid[0].Length;
    }

    private static bool GetUp(Point current, out Point up)
    {
        up = current with { Y = current.Y - 1 };
        return up.Y >= 0;
    }

    private static bool GetDown(IReadOnlyCollection<int[]> grid, Point current, out Point down)
    {
        down = current with { Y = current.Y + 1 };
        return down.Y < grid.Count;
    }
}
