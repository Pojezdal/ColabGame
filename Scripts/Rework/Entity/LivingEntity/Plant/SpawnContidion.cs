using System.Collections.Generic;
using Godot;

namespace Rework.Entity.LivingEntity.Plant;

/// <summary>
/// Defines the conditions under which a plant can spawn on a tile.
/// </summary>
public class SpawnCondition
{
    /// <summary>
    /// Biomes where the plant can spawn.
    /// The tile's biome must be in this set for the condition to be satisfied.
    /// If empty, any biome is allowed.
    /// </summary>
    public HashSet<string> Biomes { get; init; }

    /// <summary>
    /// Properties that the tile must satisfy for the plant to spawn.
    /// All specified property ranges must be met.
    /// </summary>
    public Dictionary<string, Common.Type.Range> Properties { get; init; }

    /// <summary>
    /// States that the tile must satisfy for the plant to spawn.
    /// All specified state ranges must be met.
    /// </summary>
    public Dictionary<string, Common.Type.Range> States { get; init; }

    /// <summary>
    /// Constructor for the SpawnCondition class.
    /// </summary>
    /// <param name="biomes">Biomes where the plant can spawn.</param>
    /// <param name="properties">Properties that the tile must satisfy for the plant to spawn.</param>
    /// <param name="states">States that the tile must satisfy for the plant to spawn.</param>
    public SpawnCondition(HashSet<string> biomes = null, Dictionary<string, Common.Type.Range> properties = null, Dictionary<string, Common.Type.Range> states = null)
    {
        Biomes = biomes ?? new();
        Properties = properties ?? new();
        States = states ?? new();
    }

    /// <summary>
    /// Checks if the spawn condition is satisfied on the given tile.
    /// </summary>
    /// <param name="tile">The tile to check against the spawn condition.</param>
    /// <returns>True if the condition is satisfied; otherwise, false.</returns>
    public bool IsSatisfied(World.Tile tile)
    {
        if (Biomes.Count > 0 && !Biomes.Contains(tile.Biome.Name))
            return false;

        foreach (var (key, range) in Properties)
        {
            if (!tile.Properties.TryGetValue(key, out float value) || !range.Contains(value))
                return false;
        }

        foreach (var (key, range) in States)
        {
            if (!tile.States.TryGetValue(key, out float value) || !range.Contains(value))
                return false;
        }

        return true;
    }
}