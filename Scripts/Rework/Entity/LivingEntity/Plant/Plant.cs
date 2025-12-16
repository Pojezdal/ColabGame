using System;
using System.Collections.Generic;
using Godot;


namespace Rework.Entity.LivingEntity.Plant;

/// <summary>
/// Represents a plant entity in the game world.
/// Plants can grow, reproduce, and have specific spawn conditions.
/// </summary>
public abstract partial class Plant : LivingEntity
{
    /// <summary>
    /// Emitted when the plant reproduces.
    /// </summary>
    [Signal]
    public delegate void ReproducedEventHandler(Plant plant);

    /// <summary>
    /// Conditions that must be met for the plant to spawn on a tile.
    /// </summary>
    public List<SpawnCondition> SpawnConditions { get; init; }

    /// <summary>
    /// Constructor for the Plant class.
    /// </summary>
    /// <param name="id">The unique identifier for the plant.</param>
    /// <param name="tags">Tags associated with the plant.</param>
    /// <param name="properties">Properties of the plant.</param>
    /// <param name="states">States of the plant.</param>
    /// <param name="spawnConditions">Conditions that must be met for the plant to spawn on a tile.</param>
    public Plant(string id, HashSet<string> tags = null, Dictionary<string, float> properties = null, Dictionary<string, float> states = null, List<SpawnCondition> spawnConditions = null)
        : base(id, tags, properties, states)
    {
        SpawnConditions = spawnConditions ?? new();
        Tags.Add("Plant");

        Properties.TryAdd("growth_rate", 1 / 5.0f); // Grows fully in 5 seconds
        Properties.TryAdd("reproduction_rate", 1 / 5.0f); // Reproduces every 5 seconds
        Properties.TryAdd("reproduction_radius", 2);
        Properties.TryAdd("lifespan", 15);
        Properties.TryAdd("nutrient_consumption", 0.5f);

        States.TryAdd("health", 5);
        States.TryAdd("age", 0);
        States.TryAdd("growth_progress", 0);
        States.TryAdd("reproduction_progress", 0);
    }

    /// <summary>
    /// Updates the plant's state over time.
    /// </summary>
    /// <param name="delta">The time elapsed since the last update.</param>
    public override void Tick(float delta)
    {
        // Update age and growth
        States["age"] += delta;
        States["growth_progress"] = Mathf.Min(1.0f, States["growth_progress"] + Properties["growth_rate"] * delta);

        // Update reproduction
        if (States["growth_progress"] == 1.0f)
        {
            States["reproduction_progress"] += Properties["reproduction_rate"] * delta;
            if (States["reproduction_progress"] >= 1.0f)
            {
                States["reproduction_progress"] -= 1.0f;
                EmitSignal(SignalName.Reproduced, this);
            }
        }

        // Check for death
        if (States["age"] >= Properties["lifespan"] || States["health"] <= 0)
        {
            EmitSignal(SignalName.Disposed);
        }
    }

    /// <summary>
    /// Creates an offspring plant based on the current plant's attributes.
    /// Right now, it's essentially a clone of the parent.
    /// </summary>
    /// <param name="id">The unique identifier for the offspring plant.</param>
    /// <returns>A new Plant instance representing the offspring.</returns>
    public Plant CreateOffspring(string id)
    {
        var ctor = GetType().GetConstructor(
        [
            typeof(string),
            typeof(HashSet<string>),
            typeof(Dictionary<string, float>),
            typeof(Dictionary<string, float>),
            typeof(List<SpawnCondition>)
        ]);

        if (ctor == null)
        {
            GD.PushError($"{GetType().Name} is missing a reproduction constructor.");
            return null;
        }

        return (Plant)ctor.Invoke([id,
            new HashSet<string>(Tags),
            new Dictionary<string, float>(Properties),
            null,//new Dictionary<string, float>(States),
            new List<SpawnCondition>(SpawnConditions) // Right now, this creates a shallow copy, might need deep copy later
        ]);
    }
}