using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace World.Island;

/// <summary>
/// Represents the seed data for generating an island in a specific cell.
/// Contains methods for calculating island shape and biome distribution.
/// </summary>
public partial class IslandSeed : Resource
{
    /// <summary>
    /// Random number generator for island generation.
    /// Private for each island to ensure reproducibility.
    /// </summary>
    private Utils.RNG _rng;

    /// <summary>
    /// Configuration settings for island generation.
    /// </summary>
    public IslandSeedConf Config { get; init; } = new IslandSeedConf();

    /// <summary>
    /// Indicates whether this cell contains an island.
    /// </summary>
    public bool ContainsIsland { get; init; } = true;

    /// <summary>
    /// Position of the cell in the island grid.
    /// </summary>
    public Vector2I CellPosition { get; init; } = Vector2I.Zero;

    /// <summary>
    /// Center position of the island within the cell.
    /// </summary>
    public Vector2I Center { get; init; } = Vector2I.Zero;

    /// <summary>
    /// Radius of the island.
    /// </summary>
    public int Radius { get; init; } = 10;

    /// <summary>
    /// Noise used to shape the island.
    /// </summary>
    public Common.Extended.FastNoiseExt ShapeNoise { get; init; } = null;

    /// <summary>
    /// Noise used to determine biome distribution on the island.
    /// </summary>
    public Common.Extended.FastNoiseExt BiomeNoise { get; init; } = null;

    /// <summary>
    /// Noise used to determine rain distribution on the island.
    /// </summary>
    public Common.Extended.FastNoiseExt RainNoise { get; init; } = null;

    /// <summary>
    /// Noise used to determine temperature distribution on the island.
    /// </summary>
    public Common.Extended.FastNoiseExt TemperatureNoise { get; init; } = null;

    /// <summary>
    /// Noise used to determine altitude distribution on the island.
    /// </summary>
    public Common.Extended.FastNoiseExt AltitudeNoise { get; init; } = null;

    /// <summary>
    /// Dictionary mapping biome center points (offset positions) to biome types.
    /// </summary>
    public Dictionary<Vector2I, string> BiomePoints { get; private set; } = new();

    /// <summary>
    /// Converts a world position to a local position within the cell.
    /// </summary>
    /// <param name="worldPosition">The world position</param>
    /// <returns>The local position within the cell</returns>
    public Vector2I WorldToLocal(Vector2I worldPosition) => worldPosition - (CellPosition * Config.CellSize);

    /// <summary>
    /// Converts a local position within the cell to a world position.
    /// </summary>
    /// <param name="localPosition">The local position within the cell</param>
    /// <returns>The world position</returns>
    public Vector2I LocalToWorld(Vector2I localPosition) => localPosition + (CellPosition * Config.CellSize);

    /// <summary>
    /// Converts a world position to an offset position relative to the island center.
    /// </summary>
    /// <param name="worldPosition">The world position</param>
    /// <returns>The offset position relative to the island center</returns>
    public Vector2I WorldToOffset(Vector2I worldPosition) => WorldToLocal(worldPosition) - Center;

    /// <summary>
    /// Converts an offset position relative to the island center to a world position.
    /// </summary>
    /// <param name="offsetPosition">The offset position relative to the island center</param>
    /// <returns>The world position</returns>
    public Vector2I OffsetToWorld(Vector2I offsetPosition) => LocalToWorld(offsetPosition + Center);

    /// <summary>
    /// Initializes a new instance of the IslandSeed class.
    /// The biome points are generated only if the island is present.
    /// </summary>
    /// <param name="rng">Random number generator</param>
    /// <param name="config">Island generation configuration</param>
    /// <param name="containsIsland">Whether the island is present in this cell</param>
    /// <param name="cellPosition">Position of the cell in the island grid</param
    /// ><param name="center">Center position of the island within the cell</param>
    /// <param name="radius">Radius of the island</param>
    /// <param name="shapeNoise">Noise used to shape the island</param>
    /// <param name="biomeNoise">Noise used to determine biome distribution on the island</param>
    /// <param name="rainNoise">Noise used to determine rain distribution on the island</param>
    /// <param name="temperatureNoise">Noise used to determine temperature distribution on the island</param>
    /// <param name="altitudeNoise">Noise used to determine altitude distribution on the island</param>
    public IslandSeed(Utils.RNG rng, IslandSeedConf config, bool containsIsland, Vector2I cellPosition, Vector2I center, int radius, Common.Extended.FastNoiseExt shapeNoise, Common.Extended.FastNoiseExt biomeNoise, Common.Extended.FastNoiseExt rainNoise, Common.Extended.FastNoiseExt temperatureNoise, Common.Extended.FastNoiseExt altitudeNoise)
    {
        _rng = rng;
        Config = config;
        ContainsIsland = containsIsland;
        CellPosition = cellPosition;
        Center = center;
        Radius = radius;
        ShapeNoise = shapeNoise;
        BiomeNoise = biomeNoise;
        RainNoise = rainNoise;
        TemperatureNoise = temperatureNoise;
        AltitudeNoise = altitudeNoise;
        if (ContainsIsland)
            InitializeBiomePoints(Config.Biomes.ToList());
    }

    /// <summary>
    /// Calculates the mask value at a given world position.
    /// A higher mask value indicates a position closer to the island center.
    /// Positive values are inside the island, negative values are outside.
    /// </summary>
    /// <param name="position">The world position</param>
    /// <returns>The mask value</returns>
    public float Mask(Vector2I position)
    {
        if (!ContainsIsland)
            return float.NegativeInfinity;
        Vector2I offsetPos = WorldToOffset(position);
        float distance = offsetPos.Length();
        var shapeNoiseValue = ShapeNoise.GetNoise2D(offsetPos.X, offsetPos.Y);
        float radiusVariation = 1f + shapeNoiseValue * 1f;
        float effectiveRadius = Radius * (Config.ApplyShapeNoise ? radiusVariation : 1f);
        float maskValue = 1f - (distance / effectiveRadius);
        return maskValue;
    }

    /// <summary>
    /// Determines the biome type at a given world position.
    /// Assumes that the position's mask value is positive (inside the island).
    /// </summary>
    /// <param name="position">The world position</param>
    /// <returns>The biome type</returns>
    public string Biome(Vector2I position)
    {
        if (Config.ShowBiomeCenters && BiomePoints.ContainsKey(WorldToOffset(position)))
            return "Center";

        position += Config.ApplyBiomeNoiseToPos ? new Vector2I(
            Mathf.FloorToInt(BiomeNoise.GetNoise2D(position.X, position.Y) * Config.BiomeNoiseStrength),
            Mathf.FloorToInt(BiomeNoise.GetNoise2D(position.Y, position.X) * Config.BiomeNoiseStrength)
        ) : new Vector2I(0, 0);

        Vector2I offsetPos = WorldToOffset(position);
        offsetPos += Config.ApplyBiomeNoiseToOffset ? new Vector2I(
            Mathf.FloorToInt(BiomeNoise.GetNoise2D(offsetPos.X, offsetPos.Y) * Config.BiomeNoiseStrength),
            Mathf.FloorToInt(BiomeNoise.GetNoise2D(offsetPos.Y, offsetPos.X) * Config.BiomeNoiseStrength)
        ) : new Vector2I(0, 0);


        string closestBiome = "Grass";
        float closestDistance = float.MaxValue;
        foreach (var (biomePos, biome) in BiomePoints)
        {
            float dist = biomePos.DistanceTo(offsetPos);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestBiome = biome;
            }
        }
        return SubBiome(position, closestBiome);
    }

    /// <summary>
    /// Determines the sub-biome type at a given world position based on altitude, rain, and temperature.
    /// </summary>
    /// <param name="position">The world position</param>
    /// <param name="biome">The main biome type</param>
    /// <returns>The sub-biome type</returns>
    public string SubBiome(Vector2I position, string biome)
    {
        int altitude = Mathf.RoundToInt(AltitudeNoise.GetNoise2D(position.X, position.Y));
        int rain = Mathf.RoundToInt(RainNoise.GetNoise2D(position.X, position.Y));
        int temperature = Mathf.RoundToInt(TemperatureNoise.GetNoise2D(position.X, position.Y));

        string env = string.Concat(biome, altitude, rain, temperature);
            
        return Config.SubBiomes.GetValueOrDefault(env, biome);
    }

    /// <summary>
    /// Initializes biome center points using Poisson-disc sampling.
    /// </summary>
    /// <param name="biomes">List of biome types to place</param>
    private void InitializeBiomePoints(List<string> biomes)
    {
        var samples = PoissonDiskSample();

        foreach (var biome in biomes)
        {
            if (samples.Count == 0) break;
            int sampleIndex = _rng.Int(0, samples.Count - 1);
            BiomePoints[samples[sampleIndex]] = biome;
            samples.RemoveAt(sampleIndex);
        }
    }

    /// <summary>
    /// Creates a new IslandSeed with random parameters based on the given cell position and configuration.
    /// For the same configuration and cell position, the generated island will be identical as the RNG is seeded deterministically.
    /// </summary>
    /// <param name="cellPosition">Position of the cell in the island grid</param>
    /// <param name="config">Island generation configuration</param>
    /// <returns>The generated IslandSeed</returns>
    public static IslandSeed CreateRandom(Vector2I cellPosition, IslandSeedConf config)
    {
        var rng = new Utils.RNG();
        rng.Seed((ulong)(config.Seed + cellPosition.X * 73856093 + cellPosition.Y * 19349663));
        var containsIsland = rng.Chance(config.IslandProbability);
        var center = rng.Vector2I(config.CellMargin, config.CellSize - config.CellMargin, config.CellMargin, config.CellSize - config.CellMargin);
        var radius = rng.Int(config.MinRadius, config.MaxRadius);

        var shapeNoise = config.ShapeNoise.Duplicate(true) as Common.Extended.FastNoiseExt;
        shapeNoise.Seed = rng.Int();

        var biomeNoise = config.BiomeNoise.Duplicate(true) as Common.Extended.FastNoiseExt;
        biomeNoise.Seed = rng.Int();

        var rainNoise = config.RainNoise.Duplicate(true) as Common.Extended.FastNoiseExt;
        rainNoise.Seed = rng.Int();

        var temperatureNoise = config.TemperatureNoise.Duplicate(true) as Common.Extended.FastNoiseExt;
        temperatureNoise.Seed = rng.Int();

        var elevationNoise = config.AltitudeNoise.Duplicate(true) as Common.Extended.FastNoiseExt;
        elevationNoise.Seed = rng.Int();

        return new IslandSeed(rng,
            config,
            containsIsland,
            cellPosition,
            center,
            radius,
            shapeNoise,
            biomeNoise,
            rainNoise,
            temperatureNoise,
            elevationNoise
        );
    }

    /// <summary>
    /// Generates sample points using Poisson-disc sampling to place biome centers.
    /// Ensures that points are at least a minimum distance apart and within valid island areas.
    /// </summary>
    /// <param name="initialAttempts">Number of initial random samples to try</param>
    /// <param name="activeAttempts">Number of attempts per active point</param>
    /// <returns>List of offset positions for biome centers</returns>
    private List<Vector2I> PoissonDiskSample(int initialAttempts = 1000, int activeAttempts = 30)
    {
        var minDistance = Radius * Config.MinBiomeDistance;
        var samples = new List<Vector2I>();
        var active = new List<Vector2I>();

        for (int attempt = 0; attempt < initialAttempts; attempt++)
        {
            Vector2I point = _rng.Vector2ICircle(Radius);
            if (Mask(OffsetToWorld(point)) > Config.MinBiomeShoreDistance)
            {
                samples.Add(point);
                active.Add(point);
                break;
            }
        }

        var hashedGrid = new HashedGrid(minDistance / Mathf.Sqrt(2));
        hashedGrid.Insert(samples[0]);

        while (active.Count > 0)
        {
            Vector2I activeSample = _rng.Choice(active, out int activeIndex);
            bool found = false;

            for (int i = 0; i < activeAttempts; i++)
            {
                float angle = _rng.Float(0f, Mathf.Pi * 2);
                float radius = _rng.Float(minDistance, 2 * minDistance);
                Vector2I newSample = activeSample + new Vector2I(
                    Mathf.FloorToInt(radius * Mathf.Cos(angle)),
                    Mathf.FloorToInt(radius * Mathf.Sin(angle))
                );

                if (Mask(OffsetToWorld(newSample)) > Config.MinBiomeShoreDistance)
                {
                    bool isolated = hashedGrid.Isolated(newSample, minDistance);
                    if (isolated)
                    {
                        samples.Add(newSample);
                        active.Add(newSample);
                        hashedGrid.Insert(newSample);
                        found = true;
                    }
                }
            }

            if (!found)
                active.RemoveAt(activeIndex);
        }

        GD.Print($"IslandSeed ({CellPosition}): Generated {samples.Count} biome sample points.");
        return samples;
    }

    /// <summary>
    /// Helper class for spatial hashing to optimize distance checks in Poisson-disc sampling.
    /// </summary>
    private class HashedGrid
    {
        /// <summary>
        /// Spatial hash grid storing sample points for efficient distance checks.
        /// </summary>
        private readonly Dictionary<Vector2I, Vector2I> _grid = new();

        /// <summary>
        /// Size of each cell in the spatial hash grid.
        /// </summary>
        private readonly float _cellSize;

        /// <summary>
        /// Initializes a new instance of the HashedGrid class with the specified cell size.
        /// </summary>
        /// <param name="cellSize">Size of each cell in the spatial hash grid.</param>
        public HashedGrid(float cellSize)
        {
            _cellSize = cellSize;
        }

        /// <summary>
        /// Gets the cell position in the grid for a given sample position.
        /// </summary>
        /// <param name="position">Sample position</param>
        /// <returns>Cell position in the grid</returns>
        public Vector2I GetCellPosition(Vector2I position)
        {
            return new Vector2I(
                Mathf.FloorToInt(position.X / _cellSize),
                Mathf.FloorToInt(position.Y / _cellSize)
            );
        }

        /// <summary>
        /// Inserts a sample point into the spatial hash grid.
        /// </summary>
        /// <param name="sample">Sample point to insert</param>
        public void Insert(Vector2I sample)
        {
            var cellPos = GetCellPosition(sample);
            _grid[cellPos] = sample;
        }

        /// <summary>
        /// Checks if a sample point is isolated from existing points in the grid by at least the specified minimum distance.
        /// </summary>
        /// <param name="sample">Sample point to check</param>
        /// <param name="minDistance">Minimum required distance from other points</param>
        /// <returns>True if the sample is isolated, false otherwise</returns>
        public bool Isolated(Vector2I sample, float minDistance)
        {
            var cellPos = GetCellPosition(sample);
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    var neighborCellPos = cellPos + new Vector2I(dx, dy);
                    if (_grid.TryGetValue(neighborCellPos, out Vector2I neighborSample))
                        if (neighborSample.DistanceTo(sample) < minDistance)
                            return false;
                }
            }
            return true;
        }
    }
}