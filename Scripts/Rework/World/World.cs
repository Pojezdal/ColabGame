using System.Collections.Generic;
using Godot;

namespace Rework.World;

/// <summary>
/// Represents the world composed of tiles.
/// </summary>
public partial class World : RefCounted
{
    /// <summary>
    /// Signal emitted when a static entity is added to the world.
    /// </summary>
    [Signal]
    public delegate void StaticEntityAddedEventHandler(Entity.Entity entity);

    /// <summary>
    /// Signal emitted when a static entity is removed from the world.
    /// </summary>
    [Signal]
    public delegate void StaticEntityRemovedEventHandler(Entity.Entity entity);

    /// <summary>
    /// The plant spawner responsible for spawning plants in the world.
    /// </summary>
    public readonly Entity.LivingEntity.Plant.PlantSpawner PlantSpawner;

    /// <summary>
    /// The seed data used for world generation.
    /// </summary>
    public WorldSeed SeedData { get; init; }

    /// <summary>
    /// The collection of tiles in the world, indexed by their grid position.
    /// </summary>
    public Dictionary<Vector2I, Tile> Tiles { get; private set; } = new Dictionary<Vector2I, Tile>();

    /// <summary>
    /// Random number generator for procedural generation.
    /// </summary>
    private readonly Utils.RNG _rng;

    /// <summary>
    /// The center position of the world.
    /// </summary>
    private Vector2I _center;

    /// <summary>
    /// Constructor for the World class.
    /// </summary>
    /// <param name="seedData">The seed data used for world generation.</param>
    public World(WorldSeed seedData)
    {
        PlantSpawner = new Entity.LivingEntity.Plant.PlantSpawner(this);
        SeedData = seedData;
        _rng = new Utils.RNG(seedData.Randomize ? null : seedData.Seed);
        _center = new Vector2I(SeedData.WorldSize.X / 2, SeedData.WorldSize.Y / 2);
        SeedData.ShapeNoise.Seed = _rng.Int();
        SeedData.HeightNoise.Seed = _rng.Int();
        SeedData.MoistureNoise.Seed = _rng.Int();
        GenerateTiles();
    }

    /// <summary>
    /// Generates the tiles for the world based on the seed data.
    /// </summary>
    private void GenerateTiles()
    {
        for (int x = 0; x < SeedData.WorldSize.X; x++)
        {
            for (int y = 0; y < SeedData.WorldSize.Y; y++)
            {
                var pos = new Vector2I(x, y);
                var height = SeedData.HeightNoise.GetNoise2D(x, y);
                var moisture = SeedData.MoistureNoise.GetNoise2D(x, y);
                var shape = SeedData.ShapeNoise.GetNoise2D(x, y);

                var distance = _center.DistanceTo(pos);
                float radiusVariation = 1f + shape * SeedData.ShapeNoiseStrength;
                float effectiveRadius = SeedData.IslandRadius * radiusVariation;
                float mask = 1f - (distance / effectiveRadius);

                // Apply island mask to height, negative changes the height to below sea level
                height = (mask <= 0) ? Mathf.Max(mask, -1) : height;

                var biome = Biome.BiomeDatabase.Get(SeedData.GetBiome(height, moisture, out var norms));
                Tiles[pos] = new Tile(pos, biome, new() { { "height", height }, { "moisture", moisture } }, norms);
                Tiles[pos].StaticEntityAdded += entity => EmitSignal(SignalName.StaticEntityAdded, entity);
                Tiles[pos].StaticEntityRemoved += entity => EmitSignal(SignalName.StaticEntityRemoved, entity);
            }
        }

        GenerateRiver(_center);
    }

    /// <summary>
    /// Generates a river starting from the given position, the river flows downhill until it reaches the ocean.
    /// </summary>
    /// <param name="start">The starting position of the river.</param>
    private void GenerateRiver(Vector2I start)
    {
        PriorityQueue<Tile, float> possibleSpills = new();
        HashSet<Vector2I> river = new();
        possibleSpills.Enqueue(Tiles[start], 0f);

        while (true)
        {
            var current = possibleSpills.Dequeue();
            if (current.Biome.Name == "Ocean")
                break;
            if (river.Contains(current.Position))
                continue;

            current.UpdateBiome(Biome.BiomeDatabase.Get("River"));
            river.Add(current.Position);
            if (!river.Contains(current.Position + Vector2I.Up))
                possibleSpills.Enqueue(Tiles[current.Position + Vector2I.Up], Tiles[current.Position + Vector2I.Up].NoiseValues["height"]);
            if (!river.Contains(current.Position + Vector2I.Down))
                possibleSpills.Enqueue(Tiles[current.Position + Vector2I.Down], Tiles[current.Position + Vector2I.Down].NoiseValues["height"]);
            if (!river.Contains(current.Position + Vector2I.Left))
                possibleSpills.Enqueue(Tiles[current.Position + Vector2I.Left], Tiles[current.Position + Vector2I.Left].NoiseValues["height"]);
            if (!river.Contains(current.Position + Vector2I.Right))
                possibleSpills.Enqueue(Tiles[current.Position + Vector2I.Right], Tiles[current.Position + Vector2I.Right].NoiseValues["height"]);
        }
    }

    /// <summary>
    /// Updates the world's state over time.
    /// </summary>
    /// <param name="delta">The time elapsed since the last update.</param>
    public void Tick(float delta)
    {
        foreach (var tile in Tiles.Values)
        {
            tile.Tick(delta);
        }
    }

    /// <summary>
    /// Retrieves all tiles within a specified radius from a given position.
    /// </summary>
    /// <param name="position">The center position to search from.</param>
    /// <param name="radius">The radius within which to search for tiles.</param>
    public List<Tile> GetTilesInRadius(Vector2I position, float radius)
    {
        List<Tile> tiles = new();
        int intRadius = Mathf.FloorToInt(radius);
        for (int dx = -intRadius; dx <= intRadius; dx++)
        {
            for (int dy = -intRadius; dy <= intRadius; dy++)
            {
                Vector2I checkPos = position + new Vector2I(dx, dy);
                if (checkPos.DistanceTo(position) <= radius)
                {
                    tiles.Add(Tiles[checkPos]);
                }
            }
        }
        return tiles;
    }
}