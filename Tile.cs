#nullable enable

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