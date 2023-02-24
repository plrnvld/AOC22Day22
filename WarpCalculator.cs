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

        warpTranslator = InitWarpTranslator();
    }

    Dictionary<BlockPos, BlockSide> InitWarpTranslator()
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

    public Position GetWarpTarget(Position curr)
    {
        var xBlock = (int)Math.Ceiling(((double)curr.Tile.X) / blockSize);
        var yBlock = (int)Math.Ceiling(((double)curr.Tile.Y) / blockSize);
        
        var targetBlockSide = warpTranslator[new BlockPos(xBlock, yBlock, curr.Facing)];

        var newFacing = FromSide(targetBlockSide.Side);
        var newCol = targetBlocSide.XBlock // #######################
        
        int BlockStart(int block) => blockSize * (block - 1) + 1;
        int BlockEnd(int block) => blockSize * block;
        
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
                1 => BlockStart(3),
                4 => BlockStart(1),
                6 => BlockStart(3),
                _ => throw new Exception("not supported")
            };

            newRow = row;
        }
        else if (facing is Facing.Left)
        {
            newCol = blockNum switch
            {
                1 => BlockEnd(3),
                2 => BlockEnd(3),
                5 => BlockEnd(4),
                _ => throw new Exception("not supported")
            };

            newRow = row;
        }
        else if (facing is Facing.Up)
        {
            newRow = blockNum switch
            {
                1 => BlockEnd(3),
                2 => BlockEnd(2),
                3 => BlockEnd(2),
                6 => BlockEnd(3),
                _ => throw new Exception("not supported")
            };

            newCol = col;
        }
        else // Facing.Down
        {
            newRow = blockNum switch
            {
                2 => BlockStart(2), 
                3 => BlockStart(2),
                5 => BlockStart(1),
                6 => BlockStart(3),
                _ => throw new Exception("not supported")
            };

            newCol = col;
        }

        var nextTile = boardDict[(newCol, newRow)];
        return new Position(nextTile, facing);        
    }

    Facing FromSide(Side side)
    {
        return side switch
        {
            Side.Left => Facing.Right,
            Side.Right => Facing.Left,
            Side.Up => Facing.Down,
            Side.Down => Facing.Up
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