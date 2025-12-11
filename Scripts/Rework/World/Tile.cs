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
    public Biome.Biome Biome { get; private set; }

    /// <summary>
    /// The sub-biome of the tile.
    /// </summary>
    public Biome.SubBiome SubBiome { get; private set; } = null;

    /// <summary>
    /// Noise values associated with the tile.
    /// </summary>
    public Dictionary<string, float> NoiseValues { get; private set; } = new();

    /// <summary>
    /// Normalized noise values associated with the tile.
    /// They are normalized to the range [0, 1] based on biome-specific min/max values.
    /// </summary>
    public Dictionary<string, float> NoiseValuesNorm { get; private set; } = new();

    /// <summary>
    /// Properties of the tile derived from its biome and noise values.
    /// These can change over time but not frequently (subbiome changes, etc.).
    /// </summary>
    public Dictionary<string, float> Properties { get; private set; } = new();

    /// <summary>
    /// Dynamic states of the tile that can change frequently (e.g., nutrients).
    /// </summary>
    public Dictionary<string, float> States { get; private set; } = new();

    /// <summary>
    /// Constructor for the Tile class.
    /// </summary>
    /// <param name="position">The position of the tile in the world grid.</param>
    /// <param name="biome">The biome of the tile.</param>
    /// <param name="noiseValues">Noise values associated with the tile.</param>
    /// <param name="noiseValuesNorm">Normalized noise values associated with the tile.</param>
    public Tile(Vector2I position, Biome.Biome biome, Dictionary<string, float> noiseValues, Dictionary<string, float> noiseValuesNorm)
    {
        Position = position;
        NoiseValues = noiseValues;
        NoiseValuesNorm = noiseValuesNorm;
        UpdateBiome(biome);

        States["nutrients"] = Properties["fertility"] * Utils.RNG.Instance.Float(50f, 80f);
    }

    /// <summary>
    /// Updates the tile's states over time.
    /// </summary>
    /// <param name="delta">The time elapsed since the last update.</param>
    public void Tick(float delta)
    {
        States["nutrients"] = Mathf.Min(Properties["max_nutrients"], States["nutrients"] + Properties["nutrients_growth"] * delta);
    }

    /// <summary>
    /// Updates the biome of the tile.
    /// This method recalculates the tile's properties based on the new biome.
    /// </summary>
    /// <param name="newBiome">The new biome to assign to the tile.</param>
    public void UpdateBiome(Biome.Biome newBiome)
    {
        Biome = newBiome;
        Properties["height"] = Biome.Properties["height"].FromWeight(NoiseValuesNorm["height"]);
        Properties["moisture"] = Biome.Properties["moisture"].FromWeight(NoiseValuesNorm["moisture"]);
        Properties["temperature"] = Biome.Properties["temperature"].FromNormal();
        Properties["fertility"] = Biome.Properties["fertility"].FromNormal();
        Properties["max_nutrients"] = Properties["fertility"] * 100f;
        Properties["nutrients_growth"] = Properties["fertility"];
    }
}