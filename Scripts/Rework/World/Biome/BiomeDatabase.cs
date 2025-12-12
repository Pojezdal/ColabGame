using Godot;
using System.Collections.Generic;

namespace Rework.World.Biome;

/// <summary>
/// Static database of predefined biomes.
/// </summary>
public static class BiomeDatabase
{
    /// <summary>
    /// Predefined biomes with their properties.
    /// </summary>
    private static readonly Dictionary<string, Biome> _biomes = new Dictionary<string, Biome>()
    {
        { "Ocean", new Biome("Ocean", new Dictionary<string, Common.Type.Range>()
            {
                { "height", new Common.Type.Range(-10.0f, 0.0f) },
                { "moisture", new Common.Type.Range(1f) },
                { "temperature", new Common.Type.Range(-10f, 10f) },
                { "fertility", new Common.Type.Range(0f) },
            }
        ) },
        {
            "River", new Biome("River", new Dictionary<string, Common.Type.Range>()
            {
                { "height", new Common.Type.Range(0.0f, 20.0f) },
                { "moisture", new Common.Type.Range(1f) },
                { "temperature", new Common.Type.Range(-10f, 10f) },
                { "fertility", new Common.Type.Range(0f) },
            }
        ) },
        {
            "Sand", new Biome("Sand", new Dictionary<string, Common.Type.Range>()
            {
                { "height", new Common.Type.Range(0.0f, 10f) },
                { "moisture", new Common.Type.Range(0.1f, 0.3f) },
                { "temperature", new Common.Type.Range(10f, 30f) },
                { "fertility", new Common.Type.Range(0.0f, 0.4f) },
            }
        ) },
        { "Grass", new Biome("Grass", new Dictionary<string, Common.Type.Range>()
        {
                { "height", new Common.Type.Range(10.0f, 40.0f) },
                { "moisture", new Common.Type.Range(0.2f, 0.5f) },
                { "temperature", new Common.Type.Range(5f, 25f) },
                { "fertility", new Common.Type.Range(0.6f, 1.0f) },
            }
        ) },
        {
            "Rock", new Biome("Rock", new Dictionary<string, Common.Type.Range>()
            {
                { "height", new Common.Type.Range(40.0f, 100.0f) },
                { "moisture", new Common.Type.Range(0.0f, 0.4f) },
                { "temperature", new Common.Type.Range(-10f, 20f) },
                { "fertility", new Common.Type.Range(0.0f, 0.2f) },
            }
        ) },
    };

    /// <summary>
    /// Retrieves a biome by name.
    /// </summary>
    /// <param name="name">The name of the biome.</param>
    /// <returns>The corresponding Biome object.</returns>
    public static Biome Get(string name)
    {
        if (_biomes.TryGetValue(name, out var biome))
        {
            return biome;
        }
        GD.PrintErr($"Biome '{name}' not found in database. Returning empty biome.");
        return new Biome("Enmpty Biome");
    }
}