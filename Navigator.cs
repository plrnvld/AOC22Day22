using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

public class Navigator
{
    Dictionary<(int x, int y), Tile> boardDict;
    Position Position { get; set; }
    int blockSize;
    WarpCalculator warpCalculator;
    
    public Navigator(Tile startTile, Facing facing, int blockSize, Dictionary<(int x, int y), Tile> boardDict)
    {
        Position = new(startTile, facing);

        this.blockSize = blockSize;
        this.boardDict = boardDict;

        warpCalculator = new WarpCalculator(boardDict, blockSize);
    }

    public void FollowInstructions(IEnumerable<Instruction> instructions)
    {       
        var n = 1;
        foreach (var instruction in instructions)
        {
            if (instruction.Steps.HasValue)
            {
                var count = 0;
                Tile? nextTile = null;
                Position? warpPosition;
                
                void HandleStep()
                {
                    nextTile = StepTile;
                    
                    warpPosition = nextTile!.IsWarp ? warpCalculator.GetWarpTarget(Position) : null;
                    
                    if (warpPosition is not null)
                    {
                        Console.WriteLine($"⚡⚡ Warping from {Position} to {warpPosition}?");
                        
                        nextTile = warpPosition.Tile;
                    }
                }

                HandleStep();

                while (count < instruction.Steps.Value && nextTile!.IsOpen)
                {                   
                    Position = Position with { Tile = nextTile, Facing = warpPosition?.Facing ?? Position.Facing };                    
                    HandleStep();
                
                    count++;
                }

                // Console.WriteLine($" {n}) Moved {count} steps {Position.Facing}");
            }
            else if (instruction.TurnRight.HasValue)
            {
                Position = Position with { Facing = NextFacing(instruction.TurnRight.Value) };
                // Console.WriteLine($" {n}) Rotated to {Position.Facing}");
            }
            else
                throw new Exception($"Unreadable instruction!");

            n++;
        }

        var curr = Position.Tile;
        var facing = Position.Facing;
        Console.WriteLine($"Arrived at column {curr?.X}, row {curr?.Y}, facing {facing} ({(int)facing})!");
        var score = 1000 * curr?.Y + 4 * curr?.X + (int)facing;
        Console.WriteLine($"Score = {score}");
    }

    Tile? StepTile
    {
        get
        {
            var facing = Position.Facing;
            
            return facing switch
            {
                Facing.Up => Position.Tile.Up,
                Facing.Right => Position.Tile.Right,
                Facing.Down => Position.Tile.Down,
                _ => Position.Tile.Left
            };
        }
    }

    Facing NextFacing(bool TurnRight)
    {
        return Position.Facing switch
        {
            Facing.Up => TurnRight ? Facing.Right : Facing.Left,
            Facing.Right => TurnRight ? Facing.Down : Facing.Up,
            Facing.Down => TurnRight ? Facing.Left : Facing.Right,
            _ => TurnRight ? Facing.Up : Facing.Down
        };
    }
}