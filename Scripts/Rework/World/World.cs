using System.Collections.Generic;
using Godot;

namespace Rework.World;

/// <summary>
/// Represents the world composed of tiles.
/// </summary>
public partial class World : RefCounted
{
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

                height = (mask <= 0) ? -1f : height;
                var biome = new Biome.Biome(SeedData.GetBiome(height, moisture));
                Tiles[pos] = new Tile(pos, biome, new() { { "height", height }, { "moisture", moisture } });
            }
        }
    }
}