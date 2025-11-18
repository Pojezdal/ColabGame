using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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

    public FastNoiseLite Noise = GD.Load<FastNoiseLite>("res://Assets/Resources/island_generating_noise.tres");

    /// <summary>
    /// Initializes a new instance of the World class with random terrain tiles.
    /// </summary>
    public World()
    {
        FillTiles();
    }

    public void FillTiles()
    {
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                float n = Noise.GetNoise2D((float)y, (float)x);
                string type;
                if (n <= -0.2) type = "Sand";
                else type = "Water";
                TerrainTiles[new(x, y)] = new Terrain.TerrainTile(type);
            }
        }
    }
}