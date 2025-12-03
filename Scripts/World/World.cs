using Godot;
using System;
using System.Collections.Generic;

namespace World;

/// <summary>
/// Data class representing the game world.
/// Includes terrain tiles and other world-related data.
/// </summary>
public partial class World : Resource
{
    /// <summary>
    /// Signal emitted when an entity is added to the world at a specific position.
    /// </summary>
    [Signal]
    public delegate void EntityAddedEventHandler(Entity.Entity entuty, Vector2I position);

    /// <summary>
    /// Configuration for island generation in the world.
    /// </summary>
    [Export]
    public Island.IslandSeedConf IslandConfig { get; private set; } = null;

    /// <summary>
    /// Mapping of island cell coordinates to island seeds in the world.
    /// </summary>
    private Dictionary<Vector2I, Island.IslandSeed> _islandSeeds = new();

    /// <summary>
    /// Mapping of coordinates to terrain tiles in the world.
    /// </summary>
    public Dictionary<Vector2I, Terrain.TerrainTile> TerrainTiles { get; private set; } = new();

    /// <summary>
    /// Initializes a new instance of the World class and loads the island configuration resource.
    /// </summary>
    public World()
    {
        IslandConfig = GD.Load<Island.IslandSeedConf>("res://Assets/Resources/island_seed_config.tres");
        IslandConfig.Seed = Utils.RNG.Instance.Int();
        IslandConfig.Changed += () =>
        {
            _islandSeeds.Clear();
            TerrainTiles.Clear();
            EmitSignal(SignalName.Changed);
        };
    }

    /// <summary>
    /// Gets the terrain tile at the specified position.
    /// If the tile does not exist, it is generated based on the island seeds and their masks.
    /// </summary>
    /// <param name="position">The position to get the terrain tile for</param>
    /// <returns>The terrain tile at the specified position</returns>
    public Terrain.TerrainTile GetTileAt(Vector2I position)
    {
        if (!TerrainTiles.ContainsKey(position))
        {
            Vector2I islandCell = new Vector2I(Mathf.FloorToInt(position.X / IslandConfig.CellSize), Mathf.FloorToInt(position.Y / IslandConfig.CellSize));
            float bestMask = float.NegativeInfinity;
            Island.IslandSeed bestIsland = null;
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    Vector2I neighborCell = islandCell + new Vector2I(dx, dy);
                    if (!_islandSeeds.ContainsKey(neighborCell))
                        _islandSeeds[neighborCell] = Island.IslandSeed.CreateRandom(neighborCell, IslandConfig);
                    float maskValue = _islandSeeds[neighborCell].Mask(position);
                    if (maskValue > bestMask)
                    {
                        bestMask = maskValue;
                        bestIsland = _islandSeeds[neighborCell];
                    }
                }
            }
            if (bestMask <= 0)
            {
                TerrainTiles[position] = new Terrain.TerrainTile("Water", position);
            }
            else
                TerrainTiles[position] = new Terrain.TerrainTile(bestIsland.Biome(position), position);

            TerrainTiles[position].EntityAdded += (Entity.Entity newEntity) =>
            {
                EmitSignal(SignalName.EntityAdded, newEntity, position);
            };
        }
        return TerrainTiles[position];
    }

    /// <summary>
    /// Gets all terrain tiles within a specified radius from a given position.
    /// </summary>
    /// <param name="position">The center position</param>
    /// <param name="radius">The radius to search within</param>
    /// <returns>A list of terrain tiles within the specified radius</returns>
    public List<Terrain.TerrainTile> GetTilesInRadius(Vector2I position, int radius)
    {
        List<Terrain.TerrainTile> tiles = new();
        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                Vector2I checkPos = position + new Vector2I(dx, dy);
                if (checkPos.DistanceTo(position) <= radius)
                {
                    tiles.Add(GetTileAt(checkPos));
                }
            }
        }
        return tiles;
    }
}