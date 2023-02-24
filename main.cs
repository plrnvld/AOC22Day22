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

public class Navigator
{
    Dictionary<(int x, int y), Tile> boardDict;
    Position Position { get; set; }
    int blockSize;
    
    public Navigator(Tile startTile, Facing facing, int blockSize, Dictionary<(int x, int y), Tile> boardDict)
    {
        Position = new(startTile, facing);

        this.blockSize = blockSize;
        this.boardDict = boardDict;
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
                    
                    warpPosition = nextTile!.IsWarp ? ResolveWarpPosition(Position) : null;
                    
                    if (warpPosition is not null)
                    {
                        var blockOld = BlockNum(Position.Tile.X, Position.Tile.Y);
                        var blockNew = BlockNum(warpPosition.Tile.X, warpPosition.Tile.Y);
                        Console.WriteLine($"⚡⚡ Warping from {Position} ({blockOld}) to {warpPosition} ({blockNew})?");
                        
                        nextTile = warpPosition.Tile;
                        // ############### Handle changed facing
                    }
                }

                HandleStep();

                while (count < instruction.Steps.Value && nextTile!.IsOpen)
                {                   
                    Position = Position with { Tile = nextTile, Facing = warpPosition?.Facing ?? Position.Facing };                    

                    HandleStep();
                
                    count++;
                }

                Console.WriteLine($" {n}) Moved {count} steps {Position.Facing}");
            }
            else if (instruction.TurnRight.HasValue)
            {
                Position = Position with { Facing = NextFacing(instruction.TurnRight.Value) };
                Console.WriteLine($" {n}) Rotated to {Position.Facing}");
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

    Position ResolveWarpPosition(Position curr)
    {
        var facing = curr.Facing;
        var col = curr.Tile.X;
        var row = curr.Tile.Y;        
        var blockNum = BlockNum(curr.Tile.X, curr.Tile.Y);

        var newCol = -1;
        var newRow = -1;

        if (facing is Facing.Right)
        {
            newCol = blockNum switch
            {
                1 => blockSize * 2 + 1,
                4 => blockSize * 0 + 1,
                6 => blockSize * 2 + 1,
                _ => throw new Exception("not supported")
            };

            newRow = row;
        }
        else if (facing is Facing.Left)
        {
            newCol = blockNum switch
            {
                1 => blockSize * 3,
                2 => blockSize * 3,
                5 => blockSize * 4,
                _ => throw new Exception("not supported")
            };

            newRow = row;
        }
        else if (facing is Facing.Up)
        {
            newRow = blockNum switch
            {
                1 => blockSize * 3,
                2 => blockSize * 2,
                3 => blockSize * 2,
                6 => blockSize * 3,
                _ => throw new Exception("not supported")
            };

            newCol = col;
        }
        else // Facing.Down
        {
            newRow = blockNum switch
            {
                2 => blockSize * 1 + 1, 
                3 => blockSize * 1 + 1,
                5 => blockSize * 0 + 1,
                6 => blockSize * 2 + 1,
                _ => throw new Exception("not supported")
            };

            newCol = col;
        }

        var nextTile = boardDict[(newCol, newRow)];
        return new Position(nextTile, facing);
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

    public int BlockNum(int x, int y)
    {
        var row = y;
        var col = x;
        var rowBlock = Math.Ceiling(((double)row) / blockSize);
        var colBlock = Math.Ceiling(((double)col) / blockSize);

        return (rowBlock, colBlock) switch
        {
            (1, 3) => 1,
            (2, 1) => 2,
            (2, 2) => 3,
            (2, 3) => 4,
            (3, 3) => 5,
            (3, 4) => 6,
            _ => throw new Exception($"No match with ({rowBlock},{colBlock}), Input x={x} and y={y} and blockSize={blockSize}")          
        };
    }    
}

public record Tile(int X, int Y, bool IsOpen, bool IsWarp)
{
    public Tile? Left { get; set; }
    public Tile? Right { get; set; }
    public Tile? Up { get; set; }
    public Tile? Down { get; set; }

    public override string ToString()
    {
        if (IsWarp)
            return $"⚡WARP⚡TILE⚡";
        
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

public record Position(Tile Tile, Facing Facing);