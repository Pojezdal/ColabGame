using Godot;
using System;

namespace World.Terrain;

/// <summary>
/// Data class representing a terrain tile.
/// </summary>
public partial class TerrainTile : RefCounted
{
    /// <summary>
    /// Name of the terrain tile.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Initializes a new instance of the TerrainTile class with the specified name.
    /// </summary>
    /// <param name="name">The name of the terrain tile</param>
    public TerrainTile(string name)
    {
        Name = name;
    }
}