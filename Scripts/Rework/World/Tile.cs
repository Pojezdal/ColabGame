using System.Collections.Generic;
using Rework.World.Biome;
using Godot;

namespace Rework.World;

/// <summary>
/// A single tile in the world.
/// </summary>
public partial class Tile : RefCounted
{
    /// <summary>
    /// Emitted when the biome or sub-biome of the tile changes.
    /// </summary>
    [Signal]
    public delegate void BiomeChangedEventHandler(Tile tile);

    /// <summary>
    /// Emitted when a static entity is added to the tile.
    /// </summary>
    [Signal]
    public delegate void StaticEntityAddedEventHandler(Entity.Entity entity);

    /// <summary>
    /// Emitted when a static entity is removed from the tile.
    /// </summary>
    [Signal]
    public delegate void StaticEntityRemovedEventHandler(Entity.Entity entity);

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
    public SubBiome SubBiome { get; private set; } = null;

    /// <summary>
    /// The effective biome of the tile, considering sub-biome if present.
    /// </summary>
    public Biome.Biome EffectiveBiome => SubBiome ?? Biome;

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
    /// The static entity present on the tile, if any.
    /// </summary>
    public Entity.Entity StaticEntity { get; private set; } = null;

    /// <summary>
    /// Influence values from entities present on or near the tile.
    /// Aggregated by entity tags.
    /// </summary>
    public Dictionary<string, float> EntityInfluence { get; private set; } = new();

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
    /// Sets the static entity on the tile.
    /// </summary>
    /// <param name="entity">The static entity to set on the tile.</param>
    public void SetStaticEntity(Entity.Entity entity)
    {
        if (StaticEntity != null)
        {
            StaticEntity.Disposed -= RemoveStaticEntity;
            EmitSignal(SignalName.StaticEntityRemoved, StaticEntity);
        }
        StaticEntity = entity;
        if (StaticEntity != null)
        {
            StaticEntity.TilePosition = Position;
            StaticEntity.Disposed += RemoveStaticEntity;
            EmitSignal(SignalName.StaticEntityAdded, StaticEntity);
        }
    }

    /// <summary>
    /// Removes the static entity from the tile.
    /// </summary>
    public void RemoveStaticEntity() => SetStaticEntity(null);

    /// <summary>
    /// Adds influence from an entity to the tile.
    /// The influence is calculated based on the entity's properties and distance to the tile.
    /// Right now it uses a simple formula: influence / (1 + distance), so it falls off quickly, and then slows down.
    /// Automatically updates the tile's sub-biome if necessary.
    /// </summary>
    /// <param name="entity">The entity whose influence is to be added.</param>
    public void AddEntityInfluence(Entity.Entity entity)
    {
        float influence = entity.Properties.GetValueOrDefault("influence", 0f);
        float distance = Position.DistanceTo(entity.TilePosition);
        float influenceStrength = influence / (1 + distance);
        foreach (var tag in entity.Tags)
        {
            if (EntityInfluence.ContainsKey(tag))
                EntityInfluence[tag] += influenceStrength;
            else
                EntityInfluence[tag] = influenceStrength;
        }
        CheckSubBiome();
    }

    /// <summary>
    /// Removes influence from an entity from the tile.
    /// The influence is calculated based on the entity's properties and distance to the tile.
    /// Right now it uses a simple formula: influence / (1 + distance), so it falls off quickly, and then slows down.
    /// Automatically updates the tile's sub-biome if necessary.
    /// </summary>
    /// <param name="entity">The entity whose influence is to be removed.</param>
    public void RemoveEntityInfluence(Entity.Entity entity)
    {
        float influence = entity.Properties.GetValueOrDefault("influence", 0f);
        float distance = Position.DistanceTo(entity.TilePosition);
        float influenceStrength = influence / (1 + distance);
        foreach (var tag in entity.Tags)
        {
            if (EntityInfluence.ContainsKey(tag))
            {
                EntityInfluence[tag] -= influenceStrength;
                if (EntityInfluence[tag] <= 0)
                    EntityInfluence.Remove(tag);
            }
        }
        CheckSubBiome();
    }

    /// <summary>
    /// Updates the tile's states over time.
    /// </summary>
    /// <param name="delta">The time elapsed since the last update.</param>
    public void Tick(float delta)
    {
        States["nutrients"] += Properties["nutrients_growth"] * delta;
        States["nutrients"] -= StaticEntity?.Properties.GetValueOrDefault("nutrient_consumption", 0f) * delta ?? 0f;
        States["nutrients"] = Mathf.Clamp(States["nutrients"], 0, Properties["max_nutrients"]);
        if (States["nutrients"] == 0 && StaticEntity != null && StaticEntity.Properties.ContainsKey("nutrient_consumption"))
        {
            StaticEntity.States["health"] -= 1 * delta;
        }
        if (StaticEntity != null)
        {
            StaticEntity.Tick(delta);
        }
    }

    /// <summary>
    /// Updates the biome of the tile.
    /// This method recalculates the tile's properties based on the new biome.
    /// </summary>
    /// <param name="newBiome">The new biome to assign to the tile.</param>
    public void UpdateBiome(Biome.Biome newBiome)
    {
        Biome = newBiome;
        Properties["height"] = Biome.Properties["height"].Lerp(NoiseValuesNorm["height"]);
        Properties["moisture"] = Biome.Properties["moisture"].Lerp(NoiseValuesNorm["moisture"]);
        Properties["temperature"] = Utils.RNG.Instance.FloatGaussian(Biome.Properties["temperature"]);
        Properties["fertility"] = Utils.RNG.Instance.Float(Biome.Properties["fertility"]);
        Properties["max_nutrients"] = Properties["fertility"] * 100f;
        Properties["nutrients_growth"] = Properties["fertility"];
        EmitSignal(SignalName.BiomeChanged, this);
    }

    /// <summary>
    /// Checks and updates the sub-biome of the tile based on entity influences.
    /// The first matching sub-biome rule is applied. If no rules match, the sub-biome is set to null.
    /// </summary>
    public void CheckSubBiome()
    {
        foreach (var rule in Biome.Rules)
        {
            bool match = true;
            foreach (var (tag, threshold) in rule.InfluenceThresholds)
            {
                float influenceValue = EntityInfluence.GetValueOrDefault(tag, 0f);
                if (influenceValue < threshold)
                {
                    match = false;
                    break;
                }
            }
            if (match)
            {
                if (SubBiome?.Name != rule.SubBiomeName)
                {
                    SubBiome = BiomeDatabase.GetSubBiome(rule.SubBiomeName);
                    EmitSignal(SignalName.BiomeChanged, this);
                }
                return;
            }
        }
        if (SubBiome != null)
        {
            SubBiome = null;
            EmitSignal(SignalName.BiomeChanged, this);   
        }
    }
}