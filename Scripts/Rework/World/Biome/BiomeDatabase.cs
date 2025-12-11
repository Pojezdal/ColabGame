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
        { "Ocean", new Biome("Ocean", new Dictionary<string, Property>()
            {
                { "height", new Property(-10.0f, 0.0f) },
                { "moisture", new Property(1f) },
                { "temperature", new Property(-10f, 10f) },
                { "fertility", new Property(0f) },
            }
        ) },
        {
            "River", new Biome("River", new Dictionary<string, Property>()
            {
                { "height", new Property(0.0f, 20.0f) },
                { "moisture", new Property(1f) },
                { "temperature", new Property(-10f, 10f) },
                { "fertility", new Property(0f) },
            }
        ) },
        {
            "Sand", new Biome("Sand", new Dictionary<string, Property>()
            {
                { "height", new Property(0.0f, 10f) },
                { "moisture", new Property(0.1f, 0.3f) },
                { "temperature", new Property(10f, 30f) },
                { "fertility", new Property(0.0f, 0.4f) },
            }
        ) },
        { "Grass", new Biome("Grass", new Dictionary<string, Property>()
        {
                { "height", new Property(10.0f, 40.0f) },
                { "moisture", new Property(0.2f, 0.5f) },
                { "temperature", new Property(5f, 25f) },
                { "fertility", new Property(0.6f, 1.0f) },
            }
        ) },
        {
            "Rock", new Biome("Rock", new Dictionary<string, Property>()
            {
                { "height", new Property(40.0f, 100.0f) },
                { "moisture", new Property(0.0f, 0.4f) },
                { "temperature", new Property(-10f, 20f) },
                { "fertility", new Property(0.0f, 0.2f) },
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