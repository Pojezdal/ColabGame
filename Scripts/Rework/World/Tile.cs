using System.Collections.Generic;
using Godot;

namespace Rework.World;

/// <summary>
/// A single tile in the world.
/// </summary>
public partial class Tile : RefCounted
{
    /// <summary>
    /// The position of the tile in the world grid.
    /// </summary>
    public Vector2I Position { get; init; }

    /// <summary>
    /// The biome of the tile.
    /// </summary>
    public Biome.Biome Biome { get; init; }

    /// <summary>
    /// The sub-biome of the tile.
    /// </summary>
    public Biome.SubBiome SubBiome { get; private set; } = null;

    /// <summary>
    /// Noise values associated with the tile.
    /// </summary>
    public Dictionary<string, float> NoiseValues { get; private set; } = new Dictionary<string, float>();

    /// <summary>
    /// Constructor for the Tile class.
    /// </summary>
    /// <param name="position">The position of the tile in the world grid.</param>
    /// <param name="biome">The biome of the tile.</param>
    /// <param name="noiseValues">Noise values associated with the tile.</param>
    public Tile(Vector2I position, Biome.Biome biome, Dictionary<string, float> noiseValues)
    {
        Position = position;
        Biome = biome;
        NoiseValues = noiseValues;
    }
}