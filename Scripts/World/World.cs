using Godot;
using System;
using System.Collections.Generic;

namespace World;

/// <summary>
/// Data class representing the game world.
/// Includes terrain tiles and other world-related data.
/// </summary>
public partial class World : RefCounted
{
    /// <summary>
    /// Mapping of coordinates to terrain tiles in the world.
    /// </summary>
    public Dictionary<Vector2I, Terrain.TerrainTile> TerrainTiles { get; private set; } = new();

    /// <summary>
    /// Initializes a new instance of the World class with random terrain tiles.
    /// </summary>
    public World()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                string type = Random.Shared.Next(0, 4) switch
                {
                    0 => "Grass",
                    1 => "Water",
                    2 => "Sand",
                    3 => "Stone",
                    _ => "Grass"
                };
                
                TerrainTiles[new(x, y)] = new Terrain.TerrainTile(type);
            }
        }
    }
}