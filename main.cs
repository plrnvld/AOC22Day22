using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


class Program
{
    public static void Main(string[] args)
    {
        var lines = ReadInput("Example.txt");
        Console.WriteLine(lines.Count());
    }

    static IEnumerable<string> ReadInput(string file) =>
        File.ReadLines(file);
}