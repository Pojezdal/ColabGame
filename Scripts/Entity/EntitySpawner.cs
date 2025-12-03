using Godot;
using System;

namespace Entity;

/// <summary>
/// Responsible for spawning entities in the game world.
/// </summary>
public partial class EntitySpawner : Node2D
{
    /// <summary>
    /// The world data used for spawning entities.
    /// </summary>
    protected World.World WorldData { get; private set; }

    /// <summary>
    /// Initializes the spawner with the given world data.
    /// </summary>
    /// <param name="worldData">The world data to use for spawning entities.</param
    public void Init(World.World worldData)
    {
        WorldData = worldData;
    }
}
