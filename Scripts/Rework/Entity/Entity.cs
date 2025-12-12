using System.Collections.Generic;
using Godot;

namespace Rework.Entity;

/// <summary>
/// Represents a generic entity in the game world.
/// All physical objects in the world should inherit from this class.
/// </summary>
public abstract partial class Entity : RefCounted
{
    /// <summary>
    /// Signal emitted when the entity is disposed.
    /// </summary>
    [Signal]
    public delegate void DisposedEventHandler();

    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// The position of the tile the entity is located on.
    /// </summary>
    public Vector2I TilePosition { get; set; }

    /// <summary>
    /// Tags associated with the entity.
    /// </summary>
    public HashSet<string> Tags { get; init; }

    /// <summary>
    /// Properties of the entity.
    /// Define attributes and characteristics of the entity.
    /// Not meant to change frequently.
    /// </summary>
    public Dictionary<string, float> Properties { get; init; }

    /// <summary>
    /// Constructor for the Entity class.
    /// </summary>
    /// <param name="id">The unique identifier for the entity.</param>
    /// <param name="tags">Tags associated with the entity.</param>
    /// <param name="properties">Properties of the entity.</param>
    public Entity(string id, HashSet<string> tags = null, Dictionary<string, float> properties = null)
    {
        Id = id;
        Tags = tags ?? new();
        Tags.Add("Entity");
        Properties = properties ?? new();
    }

    /// <summary>
    /// Disposes of the entity, emitting the <see cref="Disposed"/> signal.
    /// </summary>
    public new void Dispose()
    {
        EmitSignal(SignalName.Disposed);
        base.Dispose();
    }

    /// <summary>
    /// Updates the entity's state over time.
    /// </summary>
    /// <param name="delta">The time elapsed since the last update.</param>
    public abstract void Tick(float delta);

    //public abstract EntityNode CreateNode();
}