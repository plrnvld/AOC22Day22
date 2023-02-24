using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

class Program
{
    public static void Main(string[] args)
    {
        var boardReader = new BoardReader();
        var (boardDict, instructions, blockSize) = boardReader.Read("Example.txt");

        var start = boardDict[boardDict.Keys.Where(k => k.y == 1).MinBy(k => k.x)];
        Console.WriteLine($"Starting at {start}");

        var navigator = new Navigator(start, Facing.Right, blockSize, boardDict);

        navigator.FollowInstructions(instructions);
    }
}

public record Instruction(int? Steps, bool? TurnRight);

public enum Facing
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3
}

public enum Side
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3
}

public record Position(Tile Tile, Facing Facing);