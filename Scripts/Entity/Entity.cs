using Godot;
using System.Collections.Generic;

namespace Entity;

/// <summary>
/// Base class for all entities in the world.
/// </summary>
public abstract partial class Entity : RefCounted
{
    /// <summary>
    /// Signal emitted when the entity is disposed.
    /// Various systems can listen to this signal to perform cleanup or update their state accordingly.
    /// </summary>
    [Signal]
    public delegate void DisposedEventHandler();

    /// <summary>
    /// The unique identifier for this entity.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// The properties of this entity.
    /// These are variables that define the state of the entity and can change over time
    /// or due to interactions with other entities (e.g., health, hunger, energy, age).
    /// </summary>
    public Dictionary<string, Property> Properties { get; init; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the entity.</param>
    /// <param name="properties">The initial properties of the entity.</param>
    public Entity(string id, IEnumerable<Property> properties)
    {
        Id = id;

        foreach (var property in properties)
        {
            Properties[property.Name] = property;
        }
    }

    /// <summary>
    /// Creates and returns a new EntityNode associated with this entity.
    /// </summary>
    /// <returns>A new EntityNode instance associated with this entity.</returns>
    public abstract EntityNode CreateNode();
}