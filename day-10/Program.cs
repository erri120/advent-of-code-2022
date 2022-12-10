using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day10;

public class CPU : IEnumerable<int>
{
    public Instruction[] Instructions { get; }

    public int X { get; set; } = 1;

    public CPU(Instruction[] instructions)
    {
        Instructions = instructions;
    }

    public IEnumerator<int> GetEnumerator()
    {
        return new CyclesEnumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private class CyclesEnumerator : IEnumerator<int>
    {
        private readonly CPU _cpu;

        private Instruction _currentInstruction;
        private int _currentInstructionIndex;
        private int _currentInstructionCycleStart;

        public int Current { get; private set; }
        object IEnumerator.Current => Current;

        public CyclesEnumerator(CPU cpu)
        {
            _cpu = cpu;
            _currentInstruction = cpu.Instructions[0];
            _currentInstructionIndex = 0;
        }

        public bool MoveNext()
        {
            // instruction requires more cycles
            if (_currentInstructionCycleStart + _currentInstruction.CycleCount != Current)
            {
                Current += 1;
                return true;
            }

            // execute the instruction after the specified number of cycles
            _currentInstruction.Execute(_cpu);
            _currentInstructionIndex += 1;

            // no more instructions left
            if (_currentInstructionIndex >= _cpu.Instructions.Length) return false;

            // move to the next instruction
            _currentInstruction = _cpu.Instructions[_currentInstructionIndex];
            _currentInstructionCycleStart = Current;
            Current += 1;

            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose() {}
    }
}

public abstract class Instruction
{
    public abstract int CycleCount { get; }
    public abstract void Execute(CPU cpu);

    public static Instruction FromString(string line)
    {
        if (line == "noop") return new NoOpInstruction();

        var index = line.IndexOf(' ');
        var value = int.Parse(line[(index + 1)..]);
        return new AddXInstruction(value);
    }
}

public class NoOpInstruction : Instruction
{
    public override int CycleCount => 1;

    public override void Execute(CPU cpu) {}

    public override string ToString()
    {
        return "noop";
    }
}

public class AddXInstruction : Instruction
{
    private readonly int _value;

    public override int CycleCount => 2;

    public AddXInstruction(int value)
    {
        _value = value;
    }

    public override void Execute(CPU cpu)
    {
        cpu.X += _value;
    }

    public override string ToString()
    {
        return $"addx {_value}";
    }
}

public static class Program
{
    public static async Task Main(string[] args)
    {
        var lines = await File.ReadAllLinesAsync("input.txt", Encoding.UTF8);
        var instructions = lines.Select(Instruction.FromString).ToArray();

        PartOne(instructions);
        PartTwo(instructions);
    }

    private static void PartOne(Instruction[] instructions)
    {
        var cpu = new CPU(instructions);

        var signalStrength = cpu
            .Where(cycle => cycle is >= 20 and <= 220 && (cycle == 20 || (cycle - 20) % 40 == 0))
            .Select(cycle => cycle * cpu.X)
            .Sum();

        Console.WriteLine($"Signal Strength is {signalStrength}");
    }

    private static void PartTwo(Instruction[] instructions)
    {
        var cpu = new CPU(instructions);

        foreach (var cycle in cpu)
        {
            var spritePosition = cpu.X;
            var crtPosition = (cycle - 1) % 40;

            if (spritePosition - 1 > crtPosition || spritePosition + 1 < crtPosition)
            {
                // sprite is not visible
                Console.Write('.');
            }
            else
            {
                // sprite is visible
                Console.Write('#');
            }

            if (cycle % 40 == 0) Console.Write('\n');
        }
    }
}
