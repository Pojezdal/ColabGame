using System.Collections.Generic;
using Godot;

namespace Rework.Entity.LivingEntity;

/// <summary>
/// Represents a living entity in the game world.
/// </summary>
public abstract partial class LivingEntity : Entity
{
    /// <summary>
    /// States of the living entity.
    /// Define dynamic attributes that can change over time.
    /// </summary>
    public Dictionary<string, float> States { get; init; }

    /// <summary>
    /// Constructor for the LivingEntity class.
    /// </summary>
    /// <param name="id">The unique identifier for the living entity.</param>
    /// <param name="tags">Tags associated with the living entity.</param>
    /// <param name="properties">Properties of the living entity.</param>
    /// <param name="states">States of the living entity.</param>
    public LivingEntity(string id, HashSet<string> tags = null, Dictionary<string, float> properties = null, Dictionary<string, float> states = null)
        : base(id, tags, properties)
    {
        Tags.Add("LivingEntity");
        States = states ?? new();
    }
}