using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

public class WarpCalculator
{
    int blockSize;
    Dictionary<(int x, int y), Tile> boardDict;

    Dictionary<BlockPos, BlockSide> warpTranslator;

    public WarpCalculator(Dictionary<(int x, int y), Tile> boardDict, int blockSize)
    {
        this.boardDict = boardDict;
        this.blockSize = blockSize;

        warpTranslator = blockSize == 4 ? InitExampleWarpTranslator() : InitInputWarpTranslator();
    }

    Dictionary<BlockPos, BlockSide> InitExampleWarpTranslator()
    {
        return new Dictionary<BlockPos, BlockSide>
        {
            [new BlockPos(3, 1, Facing.Right)] = new BlockSide(3, 1, Side.Left),
            [new BlockPos(3, 2, Facing.Right)] = new BlockSide(1, 2, Side.Left),
            [new BlockPos(4, 3, Facing.Right)] = new BlockSide(3, 3, Side.Left),

            [new BlockPos(3, 1, Facing.Left)] = new BlockSide(3, 1, Side.Right),
            [new BlockPos(1, 2, Facing.Left)] = new BlockSide(3, 2, Side.Right),
            [new BlockPos(3, 3, Facing.Left)] = new BlockSide(4, 3, Side.Right),

            [new BlockPos(1, 2, Facing.Down)] = new BlockSide(1, 2, Side.Up),
            [new BlockPos(2, 2, Facing.Down)] = new BlockSide(2, 2, Side.Up),
            [new BlockPos(3, 3, Facing.Down)] = new BlockSide(3, 1, Side.Up),
            [new BlockPos(4, 3, Facing.Down)] = new BlockSide(4, 3, Side.Up),

            [new BlockPos(1, 2, Facing.Up)] = new BlockSide(1, 2, Side.Down),
            [new BlockPos(2, 2, Facing.Up)] = new BlockSide(2, 2, Side.Down),
            [new BlockPos(3, 1, Facing.Up)] = new BlockSide(3, 3, Side.Down),
            [new BlockPos(4, 3, Facing.Up)] = new BlockSide(4, 3, Side.Down),
        };
    }

    Dictionary<BlockPos, BlockSide> InitInputWarpTranslator()
    {
        return new Dictionary<BlockPos, BlockSide>
        {
            // ########## Change here
            
            [new BlockPos(3, 1, Facing.Right)] = new BlockSide(2, 3, Side.Right),
            [new BlockPos(2, 2, Facing.Right)] = new BlockSide(3, 1, Side.Down),
            [new BlockPos(2, 3, Facing.Right)] = new BlockSide(3, 1, Side.Right),
            [new BlockPos(1, 4, Facing.Right)] = new BlockSide(2, 3, Side.Down),

            [new BlockPos(2, 1, Facing.Left)] = new BlockSide(1, 3, Side.Left),
            [new BlockPos(2, 2, Facing.Left)] = new BlockSide(1, 3, Side.Up),
            [new BlockPos(1, 3, Facing.Left)] = new BlockSide(2, 1, Side.Left),
            [new BlockPos(1, 4, Facing.Left)] = new BlockSide(2, 1, Side.Up),

            [new BlockPos(1, 4, Facing.Down)] = new BlockSide(3, 1, Side.Up),
            [new BlockPos(2, 3, Facing.Down)] = new BlockSide(1, 4, Side.Right),
            [new BlockPos(3, 1, Facing.Down)] = new BlockSide(2, 2, Side.Right),
            
            [new BlockPos(1, 3, Facing.Up)] = new BlockSide(2, 2, Side.Left),
            [new BlockPos(2, 1, Facing.Up)] = new BlockSide(1, 4, Side.Left),
            [new BlockPos(3, 1, Facing.Up)] = new BlockSide(1, 4, Side.Down),
        };
    }

    public Position GetWarpTarget(Position curr)
    {
        Console.WriteLine($"---> Calculating warp target from {curr}");
        
        var xBlock = (int)Math.Ceiling(((double)curr.Tile.X) / blockSize);
        var yBlock = (int)Math.Ceiling(((double)curr.Tile.Y) / blockSize);
        var blockCol = RelPos(curr.Tile.X);
        var blockRow = RelPos(curr.Tile.Y);
        var blockColInv = blockSize + 1 - blockCol;
        var blockRowInv = blockSize + 1 - blockRow;

        Console.WriteLine($"---> block x={xBlock}, block y={yBlock}");
        var targetBlockSide = warpTranslator[new BlockPos(xBlock, yBlock, curr.Facing)];

        var addX = (targetBlockSide.XBlock - 1) * blockSize;
        var addY = (targetBlockSide.YBlock - 1) * blockSize;
        

        var newFacing = FromSide(targetBlockSide.Side);

        var newCol = -1;
        var newRow = -1;

        Console.WriteLine($"---> Aiming at side {targetBlockSide.Side}");

        switch (targetBlockSide.Side)
        {
            case Side.Left:
                newCol = addX + 1;
                newRow = addY + curr.Facing switch
                {
                    Facing.Right => blockRow,
                    Facing.Up => blockCol,
                    Facing.Left => blockRowInv,
                    Facing.Down => blockColInv,
                    _ => throw new Exception("Unhandled")
                };
                break;
            case Side.Up:
                newCol = addX + curr.Facing switch
                {
                    Facing.Right => blockRowInv,
                    Facing.Up => blockColInv,
                    Facing.Left => blockRow,
                    Facing.Down => blockCol,
                    _ => throw new Exception("Unhandled")
                };
                newRow = addY + 1;
                break;
            case Side.Right:
                newCol = addX + blockSize;
                newRow = addY + curr.Facing switch
                {
                    Facing.Right => blockRowInv,
                    Facing.Up => blockColInv,
                    Facing.Left => blockRow,
                    Facing.Down => blockCol,
                    _ => throw new Exception("Unhandled")
                };
                break;
            case Side.Down:
                newCol = addX + curr.Facing switch
                {
                    Facing.Right => blockRow,
                    Facing.Up => blockCol,
                    Facing.Left => blockRowInv,
                    Facing.Down => blockColInv,
                    _ => throw new Exception("Unhandled")
                };
                newRow = addY + blockSize;
                break;
            default:
                throw new Exception("Misery");
        }

        var newTile = boardDict[(newCol, newRow)];
        return new Position(newTile, newFacing);
    }

    int RelPos(int pos)
    {
        var result = pos;

        while (result > blockSize)
            result -= blockSize;

        return result;        
    }

    Facing FromSide(Side side)
    {
        return side switch
        {
            Side.Left => Facing.Right,
            Side.Right => Facing.Left,
            Side.Up => Facing.Down,
            Side.Down => Facing.Up,
            _ => throw new Exception("No side matches")
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

public record BlockPos(int XBlock, int YBlock, Facing Facing);
public record BlockSide(int XBlock, int YBlock, Side Side);