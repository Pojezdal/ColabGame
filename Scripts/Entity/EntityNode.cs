using Godot;
using System;

namespace Entity;

/// <summary>
/// Represents the visual and interactive node for an entity in the game world.
/// </summary>
public partial class EntityNode : Node2D
{
    /// <summary>
    /// Instantiates a new EntityNode for the given entity.
    /// </summary>
    /// <param name="entity">The entity to associate with the node.</param>
    /// <returns>A new EntityNode instance.</returns>
    public static EntityNode Instantiate(Entity entity)
    {
        var instance = Utils.AutoSceneInstantiator.Instantiate<EntityNode>();
        instance.Init(entity);
        return instance;
    }

    /// <summary>
    /// Initializes the EntityNode with the given entity.
    /// </summary>
    /// <param name="entity">The entity to associate with the node.</param>
    public void Init(Entity entity)
    {
        entity.Disposed += QueueFree;
    }
}
