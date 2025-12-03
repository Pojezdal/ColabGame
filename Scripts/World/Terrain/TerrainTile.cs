using Godot;
using System;

namespace World.Terrain;

/// <summary>
/// Data class representing a terrain tile.
/// </summary>
public partial class TerrainTile : RefCounted
{
    /// <summary>
    /// Signal emitted when a new entity is added to the terrain tile.
    /// </summary>
    [Signal]
    public delegate void EntityAddedEventHandler(Entity.Entity newEntity);

    /// <summary>
    /// Name of the terrain tile.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The position of the terrain tile in the tile grid.
    /// </summary>
    public Vector2I TilePosition { get; init; }

    /// <summary>
    /// The static entity present on this terrain tile, if any.
    /// </summary>
    public Entity.Entity StaticEntity { get; private set; } = null;

    /// <summary>
    /// Initializes a new instance of the TerrainTile class with the specified name.
    /// </summary>
    /// <param name="name">The name of the terrain tile</param>
    /// <param name="tilePosition">The position of the terrain tile in the tile grid</param>
    public TerrainTile(string name, Vector2I tilePosition)
    {
        Name = name;
        TilePosition = tilePosition;
    }

    /// <summary>
    /// Adds a static entity to the terrain tile.
    /// </summary>
    /// <param name="entity">The static entity to add</param>
    public void AddStaticEntity(Entity.Entity entity)
    {
        StaticEntity = entity;
        EmitSignal(SignalName.EntityAdded, entity);
    }
}