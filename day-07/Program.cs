using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day7;

public sealed class FileSystem
{
    public DirectoryEntry TopDirectory { get; }
    public DirectoryEntry CurrentDirectory { get; private set; }

    public FileSystem()
    {
        TopDirectory = new DirectoryEntry("/", null);
        CurrentDirectory = TopDirectory;
    }

    public DirectoryEntry ChangeDirectory(string to)
    {
        switch (to)
        {
            case "/":
                CurrentDirectory = TopDirectory;
                break;
            case "..":
                CurrentDirectory = CurrentDirectory.Parent ?? TopDirectory;
                break;
            default:
            {
                var subDirectory = CurrentDirectory.SubDirectories.First(dir => dir.Name.Equals(to, StringComparison.OrdinalIgnoreCase));
                CurrentDirectory = subDirectory;
                break;
            }
        }

        return CurrentDirectory;
    }
}

public sealed class DirectoryEntry
{
    private readonly List<DirectoryEntry> _subDirectories = new();
    private readonly List<FileEntry> _files = new();

    public string Name { get; }
    public DirectoryEntry? Parent { get; }

    public IEnumerable<DirectoryEntry> SubDirectories => _subDirectories;
    public IEnumerable<FileEntry> Files => _files;

    public DirectoryEntry(string name, DirectoryEntry? parent)
    {
        Name = name;
        Parent = parent;
    }

    public void AddSubDirectory(DirectoryEntry directoryEntry)
    {
        _subDirectories.Add(directoryEntry);
    }

    public void AddFile(FileEntry fileEntry)
    {
        _files.Add(fileEntry);
    }

    private long _cachedSize;
    public long CalculateSize()
    {
        if (_cachedSize != 0) return _cachedSize;

        var subDirectorySizes = _subDirectories
            .Select(subDir => subDir.CalculateSize())
            .Sum();

        _cachedSize = _files.Select(file => file.Size).Sum() + subDirectorySizes;
        return _cachedSize;
    }

    public IEnumerable<DirectoryEntry> EnumerateAllDirectories()
    {
        foreach (var subDirectory in _subDirectories)
        {
            yield return subDirectory;

            foreach (var directory in subDirectory.EnumerateAllDirectories())
            {
                yield return directory;
            }
        }
    }
}

public sealed class FileEntry
{
    public string Name { get; }
    public DirectoryEntry Parent { get; }
    public long Size { get; }

    public FileEntry(string name, DirectoryEntry parent, long size)
    {
        Name = name;
        Parent = parent;
        Size = size;
    }
}

public static class Program
{
    public static async Task Main()
    {
        var lines = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);
        var fs = BuildFileSystem(lines);

        PartOne(fs);
        PartTwo(fs);
    }

    private static FileSystem BuildFileSystem(IEnumerable<string> lines)
    {
        var fs = new FileSystem();

        var inLs = false;
        foreach (var line in lines)
        {
            if (line.StartsWith('$'))
            {
                inLs = false;

                var fullCommand = line[2..];
                var executable = fullCommand[..2];
                switch (executable)
                {
                    case "cd":
                    {
                        var target = fullCommand[3..];
                        fs.ChangeDirectory(target);
                        break;
                    }
                    case "ls":
                        inLs = true;
                        break;
                    default:
                        throw new UnreachableException("aoc doesn't lie");
                }

                continue;
            }

            if (!inLs) throw new UnreachableException("what is this?");

            var split = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2) throw new UnreachableException("aoc please don't lie");

            var firstPart = split[0];
            if (firstPart == "dir")
            {
                fs.CurrentDirectory.AddSubDirectory(new DirectoryEntry(split[1], fs.CurrentDirectory));
            }
            else
            {
                var fileSize = long.Parse(firstPart);
                fs.CurrentDirectory.AddFile(new FileEntry(split[1], fs.CurrentDirectory, fileSize));
            }
        }

        return fs;
    }

    private static void PartOne(FileSystem fs)
    {
        var totalSize = fs.TopDirectory
            .EnumerateAllDirectories()
            .Select(dir => dir.CalculateSize())
            .Where(size => size <= 100000)
            .Sum();

        Console.WriteLine($"Total Size: {totalSize}");
    }

    private static void PartTwo(FileSystem fs)
    {
        const long maxAvailableSpace = 70000000;
        const long requiredAvailableSpace = 30000000;

        var totalSize = fs.TopDirectory.CalculateSize();
        var currentAvailableSpace = maxAvailableSpace - totalSize;

        if (currentAvailableSpace >= requiredAvailableSpace)
            throw new UnreachableException("aoc doesn't lie");

        var currentSpaceRequired = requiredAvailableSpace - currentAvailableSpace;

        var (dir, size) = fs.TopDirectory
            .EnumerateAllDirectories()
            .Select(dir => (Dir: dir, Size: dir.CalculateSize()))
            .OrderBy(pair => pair.Size)
            .First(pair => pair.Size >= currentSpaceRequired);

        Console.WriteLine($"Directory {dir.Name} needs to be deleted to reclaim {size} bytes");
    }
}
