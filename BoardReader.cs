using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#nullable enable

public class BoardReader
{
    Tile WarpTile = new Tile(-1, -1, IsOpen: false, IsWarp: true);
    
    public (Dictionary<(int x, int y), Tile> boardDict, IEnumerable<Instruction> instructions, int blockSize) Read(string fileName)
    {
        var lines = ReadInput(fileName).ToList();
        var boardDict = new Dictionary<(int x, int y), Tile>();

        for (var row = 0; row < lines.Count; row++)
        {
            for (var column = 0; column < lines[row].Length; column++)
            {
                var (x, y) = (column + 1, row + 1);
                var tileState = lines[row][column];

                if (tileState is '.' or '#')
                    boardDict[(x, y)] = new Tile(x, y, tileState is '.', IsWarp: false);
            }
        }

        var minRow = boardDict.Keys.MinBy(k => k.y);
        var maxRow = boardDict.Keys.MaxBy(k => k.y);

        var blockSize = fileName == "Input.txt" ? 50 : 4;
                
        ConnectBoard(boardDict, blockSize);

        return (boardDict, ParseInstructions(lines.Last()), blockSize);
    }

    void ConnectBoard(Dictionary<(int x, int y), Tile> boardDict, int blockSize)
    {
        var (minRow, maxRow) = (boardDict.Keys.MinBy(k => k.y), boardDict.Keys.MaxBy(k => k.y));

        for (var row = minRow.y; row <= maxRow.y; row++)
            ConnectRow(boardDict, row);

        var (minCol, maxCol) = (boardDict.Keys.MinBy(k => k.x), boardDict.Keys.MaxBy(k => k.x));
        for (var col = minCol.x; col <= maxCol.x; col++)
            ConnectColumn(boardDict, col);
    }

    void ConnectRow(Dictionary<(int x, int y), Tile> boardDict, int row)
    {
        var sortedRowKeys = boardDict.Keys.Where(k => k.y == row).OrderBy(k => k.x).ToList();

        for (var i = 0; i < sortedRowKeys.Count - 1; i++)
        {
            var tile1 = boardDict[sortedRowKeys[i]];
            var tile2 = boardDict[sortedRowKeys[i + 1]];

            tile1.Right = tile2;
            tile2.Left = tile1;
        }

        var lastTile = boardDict[sortedRowKeys.Last()];
        var firstTile = boardDict[sortedRowKeys.First()];

        lastTile.Right = WarpTile;
        firstTile.Left = WarpTile;
    }

    void ConnectColumn(Dictionary<(int x, int y), Tile> boardDict, int column)
    {
        var sortedColumnKeys = boardDict.Keys.Where(k => k.x == column).OrderBy(k => k.y).ToList();

        for (var i = 0; i < sortedColumnKeys.Count - 1; i++)
        {
            var tile1 = boardDict[sortedColumnKeys[i]];
            var tile2 = boardDict[sortedColumnKeys[i + 1]];

            tile1.Down = tile2;
            tile2.Up = tile1;
        }

        var lastTile = boardDict[sortedColumnKeys.Last()];
        var firstTile = boardDict[sortedColumnKeys.First()];

        lastTile.Down = WarpTile;
        firstTile.Up = WarpTile;
    }

    IEnumerable<Instruction> ParseInstructions(string line)
    {
        bool IsNum(char c) => c >= '0' && c <= '9';
        bool IsTurn(char c) => c is 'L' or 'R';

        for (var i = 0; i < line.Length;)
        {
            var c = line[i];
            if (IsTurn(c))
            {
                i += 1;
                yield return new Instruction(Steps: null, TurnRight: c is 'R');
            }
            else
            {
                var fromIndex = line.Substring(i);
                var numText = string.Concat(fromIndex.TakeWhile(IsNum));
                i += numText.Length;
                yield return new Instruction(Steps: int.Parse(numText), TurnRight: null);
            }
        }
    }

    static IEnumerable<string> ReadInput(string file) =>
        File.ReadLines(file);
}