using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    public static void Main(string[] args)
    {
        var boardReader = new BoardReader();
        var (boardDict, instructions) = boardReader.Read("Input.txt");

        var start = boardDict[boardDict.Keys.Where(k => k.y == 1).MinBy(k => k.x)];
        Console.WriteLine($"Starting at {start}");

        var navigator = new Navigator(start, Facing.Right);

        navigator.FollowInstructions(instructions);
    }
}

public class Navigator
{
    Tile curr;
    Facing facing;

    public Navigator(Tile startTile, Facing facing)
    {
        curr = startTile;
        this.facing = facing;
    }

    public void FollowInstructions(IEnumerable<Instruction> instructions)
    {
        var n = 1;
        foreach (var instruction in instructions)
        {
            if (instruction.Steps.HasValue)
            {
                var count = 0;
                var nextTile = StepTile;

                // Console.WriteLine($"    - Next tile {nextTile}");

                while (count < instruction.Steps.Value && nextTile.IsOpen)
                {
                    curr = nextTile;
                    nextTile = StepTile;
                    count++;
                }

                Console.WriteLine($" {n}) Moved {count} steps {facing}");
            }
            else if (instruction.TurnRight.HasValue)
            {
                facing = NextFacing(instruction.TurnRight.Value);
                Console.WriteLine($" {n}) Rotated to {facing}");
            }
            else
                throw new Exception($"Unreadable instruction!");

            n++;
        }

        Console.WriteLine($"Arrived at column {curr?.X}, row {curr?.Y}, facing {facing} ({(int)facing})!");
        var score = 1000 * curr.Y + 4 * curr.X + (int)facing;
        Console.WriteLine($"Score = {score}");
    }

    Tile StepTile
    {
        get
        {
            return facing switch
            {
                Facing.Up => curr.Up,
                Facing.Right => curr.Right,
                Facing.Down => curr.Down,
                _ => curr.Left
            };
        }
    }

    Facing NextFacing(bool TurnRight)
    {
        return facing switch
        {
            Facing.Up => TurnRight ? Facing.Right : Facing.Left,
            Facing.Right => TurnRight ? Facing.Down : Facing.Up,
            Facing.Down => TurnRight ? Facing.Left : Facing.Right,
            _ => TurnRight ? Facing.Up : Facing.Down
        };
    }
}



public record Tile(int X, int Y, bool IsOpen)
{
    public Tile Left { get; set; }
    public Tile Right { get; set; }
    public Tile Up { get; set; }
    public Tile Down { get; set; }

    public override string ToString()
    {
        return $"Tile ({X},{Y}) open={IsOpen}";
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