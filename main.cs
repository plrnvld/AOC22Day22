using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


class Program
{
    public static void Main(string[] args)
    {
        var boardReader = new BoardReader();
        var (boardDict, instructions) = boardReader.Read("Example.txt");

        foreach (var instruction in instructions)
            Console.WriteLine(instruction);
    }
}



public record Tile(int X, int Y, bool Open, Tile Left, Tile Right, Tile Up, Tile Down);

public record Instruction(int? Steps, bool? TurnRight);