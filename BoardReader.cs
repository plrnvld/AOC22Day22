using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class BoardReader
{
    public (Dictionary<(int x, int y), Tile> boardDict, IEnumerable<Instruction> instructions) Read(string fileName)
    {
        var lines = ReadInput("Example.txt").ToList();

        var boardDict = new Dictionary<(int x, int y), Tile>();
        
        for (var row = 0; row < lines.Count; row++) 
        {
            for (var column = 0; column < lines[row].Length; column++)
            {
                var (x, y) = (column + 1, row + 1);
                var tileState = lines[row][column];

                if (tileState is '.' or '#')                
                    boardDict[(x, y)] = new Tile(x, y, tileState is '.', null, null, null, null);
            }            
        }

        var minRow = boardDict.Keys.MinBy(k => k.y);
        var maxRow = boardDict.Keys.MaxBy(k => k.y);

        Console.WriteLine(boardDict.Keys.Count());
        Console.WriteLine(lines.Last());

        ConnectBoard(boardDict);
        
        return (boardDict, ParseInstructions(lines.Last()));
    }

    void ConnectBoard(Dictionary<(int x, int y), Tile> boardDict)
    {
        // Horizontal
        var (minRow, maxRow) = (boardDict.Keys.MinBy(k => k.y), boardDict.Keys.MaxBy(k => k.y));

        for (var row = minRow.y; row <= maxRow.y; row++)
            ConnectRow(boardDict, row);

        // Vertical
        var (minCol, maxCol) = (boardDict.Keys.MinBy(k => k.x), boardDict.Keys.MaxBy(k => k.x));
        for (var col = minCol.x; col <= maxCol.x; col++)
            ConnectColumn(boardDict, col);        
    }

    void ConnectRow(Dictionary<(int x, int y), Tile> boardDict, int row)
    {
        // ###################
        
    }

    void ConnectColumn(Dictionary<(int x, int y), Tile> boardDict, int column)
    {
        // #############################
        
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