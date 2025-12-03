using Godot;
using System;
using System.Collections.Generic;

namespace Entity.Plant;

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
    /// Spawns a carrot plant at a random valid location within the specified radius of the given position.
    /// </summary>
    /// <param name="position">The central position to spawn around.</param>
    /// <param name="radius">The radius within which to spawn the carrot.</param>
    public void SpawnCarrot(Vector2I position, int radius)
    {
        List<World.Terrain.TerrainTile> tilesInRadius = WorldData.GetTilesInRadius(position, radius);
        List<World.Terrain.TerrainTile> validTiles = tilesInRadius.FindAll(tile => tile.Name == "Grass" && tile.StaticEntity == null);

        var selectedTile = validTiles.Count > 0 ? Utils.RNG.Instance.Choice(validTiles) : null;
        if (selectedTile != null)
        {
            var carrot = new Carrot($"carrot_{_idCounter++}");  // Create new carrot with unique ID
            selectedTile.AddStaticEntity(carrot);  // Add carrot to the terrain tile (this triggers spawning of the node in the world)
            var timer = new Timer()  // Create a timer for reproduction
            {
                WaitTime = (float)carrot.Properties["reproduction_time"].Value,
                OneShot = false,
                Autostart = true,
            };
            AddChild(timer);
            timer.Timeout += () =>  // On timer timeout, attempt to spawn a new carrot
            {
                SpawnCarrot(selectedTile.TilePosition, (int)carrot.Properties["reproduction_radius"].Value);
            };
            carrot.Disposed += timer.QueueFree;  // When the carrot is disposed, free the timer
        }
    }
}
