using Godot;
using System;

namespace Rework.Entity;

/// <summary>
/// Responsible for spawning entities in the game world.
/// </summary>
public abstract partial class EntitySpawner : RefCounted
{
    /// <summary>
    /// The world data used to spawn entities to.
    /// </summary>
    protected World.World WorldData { get; private set; }

    /// <summary>
    /// Constructor for the EntitySpawner class.
    /// </summary>
    /// <param name="worldData">The world data used to spawn entities to.</param>
    public EntitySpawner(World.World worldData)
    {
        WorldData = worldData;
    }
}
