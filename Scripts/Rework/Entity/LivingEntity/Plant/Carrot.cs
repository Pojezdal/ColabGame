using System.Collections.Generic;
using Godot;

namespace Rework.Entity.LivingEntity.Plant;

/// <summary>
/// Represents a carrot plant in the game world.
/// </summary>
public partial class Carrot : Plant
{
    /// <summary>
    /// Constructor for the Carrot class.
    /// </summary>
    /// <param name="id">The unique identifier for the carrot.</param>
    /// <param name="tags">Tags associated with the carrot.</param>
    /// <param name="properties">Properties of the carrot.</param>
    /// <param name="states">States of the carrot.</param>
    /// <param name="spawnConditions">Conditions that must be met for the carrot to spawn on a tile.</param>
    public Carrot(string id, HashSet<string> tags = null, Dictionary<string, float> properties = null, Dictionary<string, float> states = null, List<SpawnCondition> spawnConditions = null)
        : base(id, tags, properties, states, spawnConditions)
    {
        Tags.Add("Vegetable");
        Tags.Add("Carrot");
    }

    /// <summary>
    /// Default constructor for the Carrot class with predefined properties and spawn conditions.
    /// </summary>
    /// <param name="id">The unique identifier for the carrot.</param>
    public Carrot(string id) : this(id,
        properties: new()
        {
            { "growth_rate", 1 / 5.0f },
            { "reproduction_rate", 1 / 5.0f },
            { "reproduction_radius", 2 },
            { "lifespan", 15 },
            { "nutrient_consumption", 0.5f },
        },
        spawnConditions:
        [
            new SpawnCondition(
                biomes: ["Grass"],
                properties: new()
                {
                    { "fertility", new Common.Type.Range(0.5f, 1.0f) },
                    { "moisture", new Common.Type.Range(0.3f, 0.8f) },
                }
            )
        ])
    {
    }
}