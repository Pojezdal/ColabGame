using System;
using System.Collections.Generic;
using Godot;

namespace Rework.World;

/// <summary>
/// The seed data used for world generation.
/// </summary>
[GlobalClass]
public partial class WorldSeed : Resource
{
    /// <summary>
    /// The seed value for random number generation.
    /// </summary>
    [Export]
    public ulong Seed { get; private set; } = 0;

    /// <summary>
    /// Whether to randomize the seed value.
    /// If true, <see cref="Seed"/> is ignored.
    /// </summary>
    [Export]
    public bool Randomize { get; private set; } = true;

    /// <summary>
    /// Noise used for height generation.
    /// </summary>
    [Export]
    public Common.Extended.FastNoiseExt HeightNoise { get; private set; } = null;

    /// <summary>
    /// Noise used for moisture generation.
    /// </summary>
    [Export]
    public Common.Extended.FastNoiseExt MoistureNoise { get; private set; } = null;

    /// <summary>
    /// Noise used for shape generation.
    /// </summary>
    [Export]
    public Common.Extended.FastNoiseExt ShapeNoise { get; private set; } = null;

    /// <summary>
    /// Strength of the shape noise effect.
    /// </summary>
    [Export(PropertyHint.Range, "0.1,2,0.05")]
    public float ShapeNoiseStrength { get; private set; } = 1f;

    /// <summary>
    /// The size of the world in tiles.
    /// </summary>
    [Export]
    public Vector2I WorldSize { get; private set; } = new Vector2I(100, 100);

    /// <summary>
    /// The radius of the island in tiles.
    /// </summary>
    [Export]
    public int IslandRadius { get; private set; } = 40;

    /// <summary>
    /// Height thresholds for biome determination.
    /// </summary>
    public float[] HeightThresholds { get; private set; } =
    {
        -1, // Padding (don't change)
        0.0f,
        0.28f,
        0.7f,
        1f
    };

    /// <summary>
    /// Moisture thresholds for biome determination.
    /// </summary>
    public float[] MoistureThresholds { get; private set; } =
    {
        0.0f, // Padding (don't change)
        0.25f,
        0.45f,
        0.75f,
        1.0f
    };

    /// <summary>
    /// Biome map for determining biome based on height and moisture bands.
    /// </summary>
    public string[,] BiomeMap { get; private set; } =
    {
        { "Pad", "Pad", "Pad", "Pad", "Pad" },
        { "Pad", "Ocean", "Ocean", "Ocean", "Ocean" },
        { "Pad", "Sand", "Sand", "Sand", "Sand" },
        { "Pad", "Grass", "Grass", "Grass", "Grass" },
        { "Pad", "Rock", "Rock", "Rock", "Rock" }
    };

    /// <summary>
    /// Finds the band index for a given value based on the provided thresholds.
    /// </summary>
    /// <param name="value">The value to find the band for.</param>
    /// <param name="thresholds">The thresholds defining the bands.</param>
    /// <returns>A tuple containing the band index and the normalized value within that band.</returns>
    private Tuple<int, float> FindBand(float value, float[] thresholds)
    {
        for (int i = 0; i < thresholds.Length; i++)
        {
            if (value < thresholds[i])
                return Tuple.Create(i, Mathf.InverseLerp(thresholds[i - 1], thresholds[i], value));
        }
        return Tuple.Create(thresholds.Length - 1, 1f);
    }
    
    /// <summary>
    /// Gets the biome name based on height and moisture values.
    /// </summary>
    /// <param name="height">The height value.</param>
    /// <param name="moisture">The moisture value.</param>
    /// <param name="norms">Normalized values within their respective bands.</param>
    /// <returns>The name of the biome.</returns>
    public string GetBiome(float height, float moisture, out Dictionary<string, float> norms)
    {
        var (hI, heightNorm) = FindBand(height, HeightThresholds);
        var (mI, moistureNorm) = FindBand(moisture, MoistureThresholds);
        norms = new Dictionary<string, float>
        {
            { "height", heightNorm },
            { "moisture", moistureNorm }
        };
        return BiomeMap[hI, mI];
    }
}