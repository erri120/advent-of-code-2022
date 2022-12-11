using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day6;

public static class Program
{
    public static async Task Main()
    {
        var contents = await File.ReadAllTextAsync("input.txt", Encoding.UTF8);

        var partOne = FindMarker(contents, 4);
        var partTwo = FindMarker(contents, 14);

        Console.WriteLine($"start-of-packet marker: \"{contents[partOne]}\" ({partOne})");
        Console.WriteLine($"start-of-message marker: \"{contents[partTwo]}\" ({partTwo})");
    }

    private static Range FindMarker(string contents, int markerLength)
    {
        for (var i = 0; i < contents.Length; i++)
        {
            if (i + markerLength > contents.Length) continue;

            var range = new Range(i, i + markerLength);
            var group = contents[range];

            var onlyUnique = group.Distinct().Count() == markerLength;
            if (onlyUnique) return range;
        }

        throw new UnreachableException();
    }
}
