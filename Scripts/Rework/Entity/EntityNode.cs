using Godot;
using System;

namespace Rework.Entity;

/// <summary>
/// Base class for all entity nodes in the game.
/// Ensures that each node is associated with an Entity and handles its initialization
/// and disposal.
/// </summary>
public partial class EntityNode : Node2D
{
    /// <summary>
    /// The entity associated with this node.
    /// </summary>
    public Entity Entity { get; private set; }

    /// <summary>
    /// Initializes the EntityNode with the given entity and position.
    /// </summary>
    /// <param name="entity">The entity to associate with the node.</param>
    /// <param name="position">The position to place the node at.</param>
    public void Init(Entity entity, Vector2 position)
    {
        Entity = entity;
        Position = position;
        entity.Disposed += QueueFree;
    }
}
