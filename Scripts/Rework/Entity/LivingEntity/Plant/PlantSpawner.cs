using Godot;
using System;
using System.Collections.Generic;

namespace Rework.Entity.LivingEntity.Plant;

/// <summary>
/// Responsible for spawning plant entities in the game world.
/// </summary>
public partial class PlantSpawner : EntitySpawner
{
    /// <summary>
    /// Counter for generating unique IDs for plants.
    /// </summary>
    private int _idCounter = 0;

    /// <summary>
    /// Constructor for the PlantSpawner class.
    /// </summary>
    /// <param name="worldData">The world data used for spawning plants.</param>
    public PlantSpawner(World.World worldData) : base(worldData)
    {
    }

    /// <summary>
    /// Spawns a plant within a specified radius around a center position.
    /// </summary>
    /// <param name="plant">The plant to spawn.</param>
    /// <param name="center">The center position around which to spawn the plant.</param>
    /// <param name="radius">The radius within which to spawn the plant.</param>
    public void Spawn(Plant plant, Vector2I center, float radius)
    {
        List<World.Tile> tilesInRadius = WorldData.GetTilesInRadius(center, radius);
        List<World.Tile> validTiles = tilesInRadius.FindAll(tile =>
        {
            foreach (var condition in plant.SpawnConditions)
            {
                if (condition.IsSatisfied(tile))
                    return tile.StaticEntity == null;
            }
            return false;
        });

        var selectedTile = validTiles.Count > 0 ? Utils.RNG.Instance.Choice(validTiles) : null;
        if (selectedTile != null)
        {
            selectedTile.SetStaticEntity(plant);
            plant.Reproduced += Spawn;
        }
    }

    /// <summary>
    /// Spawns an offspring of the given parent plant within its reproduction radius.
    /// </summary>
    /// <param name="parent">The parent plant.</param>
    public void Spawn(Plant parent)
    {
        Spawn(parent.CreateOffspring($"plant_{_idCounter++}"), parent.TilePosition, parent.Properties["reproduction_radius"]); 
    }
}
