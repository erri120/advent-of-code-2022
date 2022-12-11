using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day2;

public enum RoundOutcome
{
    Lost = 0,
    Draw = 3,
    Won = 6
}

public enum Shape
{
    Rock = 1,
    Paper = 2,
    Scissors = 3
}

public static class Program
{
    public static async Task Main()
    {
        var scores = (await File.ReadAllLinesAsync("input.txt", Encoding.UTF8))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var a = line[0];
                var b = line[2];

                var partOne = PartOneScore(a, b);
                var partTwo = PartTwoScore(a, b);

                return (partOne, partTwo);
            })
            .ToArray();

        var partOneTotalScore = scores.Select(x => x.partOne).Sum();
        var partTwoTotalScore = scores.Select(x => x.partTwo).Sum();

        Console.WriteLine($"Part One - Total Score: {partOneTotalScore}");
        Console.WriteLine($"Part Two - Total Score: {partTwoTotalScore}");
    }

    private static Shape OpponentShape(char opponentInput)
    {
        return opponentInput switch
        {
            'A' => Shape.Rock,
            'B' => Shape.Paper,
            'C' => Shape.Scissors,
            _ => throw new UnreachableException()
        };
    }

    private static RoundOutcome CalculateRoundOutcome(Shape opponentShape, Shape playerShape)
    {
        var opponentValue = (int)opponentShape;
        var playerValue = (int)playerShape;

        if (opponentValue == playerValue) return RoundOutcome.Draw;
        return opponentValue % 3 + 1 == playerValue
            ? RoundOutcome.Won
            : RoundOutcome.Lost;
    }

    private static int RoundScore(RoundOutcome outcome, Shape playedShape)
    {
        var shapePoints = (int)playedShape;
        return shapePoints + (int)outcome;
    }

    private static int PartOneScore(char opponentInput, char playerInput)
    {
        var opponentShape = OpponentShape(opponentInput);
        var playerShape = playerInput switch
        {
            'X' => Shape.Rock,
            'Y' => Shape.Paper,
            'Z' => Shape.Scissors,
            _ => throw new UnreachableException()
        };

        var outcome = CalculateRoundOutcome(opponentShape, playerShape);
        return RoundScore(outcome, playerShape);
    }

    private static int PartTwoScore(char opponent, char predictedOutcome)
    {
        var opponentShape = OpponentShape(opponent);
        var opponentValue = (int)opponentShape;

        var outcome = predictedOutcome switch
        {
            'X' => RoundOutcome.Lost,
            'Y' => RoundOutcome.Draw,
            'Z' => RoundOutcome.Won,
            _ => throw new UnreachableException()
        };

        var playerValue = outcome switch
        {
            RoundOutcome.Lost => opponentValue == 1 ? 3 : opponentValue - 1,
            RoundOutcome.Won => opponentValue % 3 + 1,
            RoundOutcome.Draw => (int)opponentShape,
            _ => throw new UnreachableException()
        };

        var playerShape = (Shape)playerValue;
        return RoundScore(outcome, playerShape);
    }
}
