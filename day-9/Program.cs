using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Day9;

public enum Direction
{
    Right,
    Left,
    Up,
    Down
}

public readonly struct Motion
{
    public readonly Direction Direction;
    public readonly int Steps;

    public Motion(Direction direction, int steps)
    {
        Direction = direction;
        Steps = steps;
    }

    public static Motion FromString(string line)
    {
        var sDirection = line[0];
        var direction = sDirection switch
        {
            'R' => Direction.Right,
            'L' => Direction.Left,
            'U' => Direction.Up,
            'D' => Direction.Down,
            _ => throw new UnreachableException()
        };

        var count = int.Parse(line[1..]);
        return new Motion(direction, count);
    }

    public override string ToString()
    {
        var sDirection = Direction switch
        {
            Direction.Right => "R",
            Direction.Left => "L",
            Direction.Up => "U",
            Direction.Down => "D",
            _ => throw new UnreachableException()
        };

        return $"{sDirection} {Steps}";
    }
}

public static class Program
{
    public static async Task Main(string[] args)
    {
        var lines = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);
        var motions = lines.Select(Motion.FromString).ToArray();

        Console.WriteLine($"Part One: {RunPart(motions, 2)}");
        Console.WriteLine($"Part Two: {RunPart(motions, 10)}");
    }

    private static Vector2 NewHeadPosition(Vector2 current, Direction direction)
    {
        return direction switch
        {
            Direction.Right => current with { X = current.X += 1 },
            Direction.Left => current with { X = current.X -= 1 },
            Direction.Up => current with { Y = current.Y += 1 },
            Direction.Down => current with { Y = current.Y -= 1 },
            _ => throw new UnreachableException()
        };
    }

    private static Vector2 NewTailPosition(Vector2 tail, Vector2 head)
    {
        var distance = Vector2.DistanceSquared(head, tail);
        if (distance <= 2.0f) return tail;

        var delta = Vector2.Subtract(head, tail);
        return new Vector2(tail.X + MathF.Sign(delta.X), tail.Y + MathF.Sign(delta.Y));
    }

    private static void UpdateNodes(LinkedListNode<Vector2> node, Vector2 head)
    {
        while (true)
        {
            node.Value = NewTailPosition(node.Value, head);
            if (node.Next is null) break;

            var node1 = node;
            node = node.Next;
            head = node1.Value;
        }
    }

    private static int RunPart(IEnumerable<Motion> motions, int knotCount)
    {
        var linkedList = new LinkedList<Vector2>(Enumerable.Range(0, knotCount).Select(_ => Vector2.Zero));

        var visitedPositions = motions.SelectMany(motion =>
        {
            return Enumerable.Range(0, motion.Steps)
                .Select(_ =>
                {
                    var headNode = linkedList.First!;
                    var tailNode = linkedList.Last!;

                    var newHeadPos = NewHeadPosition(headNode.Value, motion.Direction);
                    headNode.Value = newHeadPos;

                    UpdateNodes(headNode.Next!, newHeadPos);
                    return tailNode.Value;
                });
        }).Distinct().Count();

        return visitedPositions;
    }
}
