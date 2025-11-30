using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace World.Island;

/// <summary>
/// Configuration resource for island generation.
/// Contains parameters for island size, shape noise, biome distribution, etc.
/// </summary>
[GlobalClass]
public partial class IslandSeedConf : Resource
{
    /// <summary>
    /// Backing field for <see cref="Seed"/>.
    /// </summary>
    private int _seed = 0;

    /// <summary>
    /// Backing field for <see cref="CellSize"/>.
    /// </summary>
    private int _cellSize = 100;

    /// <summary>
    /// Backing field for <see cref="IslandProbability"/>.
    /// </summary>
    private float _islandProbability = 0.7f;

    /// <summary>
    /// Backing field for <see cref="MinRadius"/>.
    /// </summary>
    private int _minRadius = 15;

    /// <summary>
    /// Backing field for <see cref="MaxRadius"/>.
    /// </summary>
    private int _maxRadius = 35;

    /// <summary>
    /// Backing field for <see cref="CellMargin"/>.
    /// </summary>
    private int _cellMargin = 30;

    /// <summary>
    /// Backing field for <see cref="ApplyShapeNoise"/>.
    /// </summary>
    private bool _applyShapeNoise = true;

    /// <summary>
    /// Backing field for <see cref="ShapeNoise"/>.
    /// </summary>
    private Common.Extended.FastNoiseExt _shapeNoise;

    /// <summary>
    /// Backing field for <see cref="ShapeNoiseStrength"/>.
    /// </summary>
    private float _shapeNoiseStrength = 0.5f;

    /// <summary>
    /// Backing field for <see cref="ApplyBiomeNoiseToPos"/>.
    /// </summary>  
    private bool _applyBiomeNoiseToPos = true;

    /// <summary>
    /// Backing field for <see cref="ApplyBiomeNoiseToOffset"/>.
    /// </summary>
    private bool _applyBiomeNoiseToOffset = false;

    /// <summary>
    /// Backing field for <see cref="BiomeNoise"/>.
    /// </summary>
    private Common.Extended.FastNoiseExt _biomeNoise;

    /// <summary>
    /// Backing field for <see cref="RainNoise"/>.
    /// </summary>
    private Common.Extended.FastNoiseExt _rainNoise;

    /// <summary>
    /// Backing field for <see cref="TemperatureNoise"/>.
    /// </summary>
    private Common.Extended.FastNoiseExt _temperatureNoise;

    /// <summary>
    /// Backing field for <see cref="AltitudeNoise"/>.
    /// </summary>
    private Common.Extended.FastNoiseExt _altitudeNoise;

    /// <summary>
    /// Backing field for <see cref="RiverNoise"/>.
    /// </summary>
    private FastNoiseLite _riverNoise;

    /// <summary>
    /// Backing field for <see cref="BiomeNoiseStrength"/>.
    /// </summary>
    private float _biomeNoiseStrength = 10f;

    /// <summary>
    /// Backing field for <see cref="Biomes"/>.
    /// </summary>
    private Godot.Collections.Array<string> _biomes = new() { "Grass", "Grass", "Sand", "Stone" };

    /// <summary>
    /// Backing field for <see cref="MinBiomeDistance"/>.
    /// </summary>
    private float _minBiomeDistance = 0.4f;

    /// <summary>
    /// Backing field for <see cref="MinBiomeShoreDistance"/>.
    /// </summary>
    private float _minBiomeShoreDistance = 0.1f;

    /// <summary>
    /// Backing field for <see cref="ShowBiomeCenters"/>.
    /// </summary>
    private bool _showBiomeCenters = true;

    /// <summary>
    /// Backing field for <see cref="SubBiomes"/>.
    /// </summary>
    private Godot.Collections.Dictionary<string, string> _subBiomes = new()
    {
        { "Grass0001", "Water"},
        { "Grass0011", "Water"},
        { "Grass0101", "Water"},
        { "Grass0111", "Water"},
        { "Grass1001", "Water"},
        { "Grass1011", "Water"},
        { "Grass1101", "Water"},
        { "Grass1111", "Water"},
        { "Water", "Water" },
        { "Grass1010", "Center" },
        { "Stone", "Stone" },
        { "Sand", "Sand" },
        { "Center", "Center" }
    };

    /// <summary>
    /// Random seed for island generation.
    /// Changing this will regenerate all islands.
    /// </summary>
    [ExportGroup("General")]
    [Export]
    public int Seed
    {
        get => _seed;
        set
        {
            if (_seed == value) return;
            _seed = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Size of each cell in the island grid.
    /// Each cell may contain an island based on <see cref="IslandProbability"/>.
    /// </summary>
    [ExportGroup("General")]
    [Export(PropertyHint.Range, "10,1000,10")]
    public int CellSize
    {
        get => _cellSize;
        set
        {
            if (_cellSize == value) return;
            _cellSize = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Probability of an island being generated in each cell.
    /// </summary>
    [ExportGroup("General")]
    [Export(PropertyHint.Range, "0.0,1.0,0.01")]
    public float IslandProbability
    {
        get => _islandProbability;
        set
        {
            if (Mathf.IsEqualApprox(_islandProbability, value)) return;
            _islandProbability = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Minimum radius of an island.
    /// </summary>
    [ExportGroup("Size")]
    [Export(PropertyHint.Range, "5.0,1000,1.0")]
    public int MinRadius
    {
        get => _minRadius;
        set
        {
            if (_minRadius == value) return;
            _minRadius = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Maximum radius of an island.
    /// </summary>
    [ExportGroup("Size")]
    [Export(PropertyHint.Range, "5.0,1000,1.0")]
    public int MaxRadius
    {
        get => _maxRadius;
        set
        {
            if (_maxRadius == value) return;
            _maxRadius = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Margin within each cell where island's center will not be generated.
    /// Bigger margin results in islands being more centered within their cells.
    /// </summary>
    [ExportGroup("Size")]
    [Export(PropertyHint.Range, "0,1000,1")]
    public int CellMargin
    {
        get => _cellMargin;
        set
        {
            if (_cellMargin == value) return;
            _cellMargin = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Whether to apply shape noise to the island's shape.
    /// Without shape noise, islands will be perfect circles.
    /// </summary>
    [ExportGroup("Shape")]
    [Export]
    public bool ApplyShapeNoise
    {
        get => _applyShapeNoise;
        set
        {
            if (_applyShapeNoise == value) return;
            _applyShapeNoise = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Shape noise generator for island shape variation.
    /// Can be configured for different noise types and parameters.
    /// </summary>
    [ExportGroup("Shape")]
    [Export]
    public Common.Extended.FastNoiseExt ShapeNoise
    {
        get => _shapeNoise;
        set
        {
            if (ReferenceEquals(_shapeNoise, value)) return;
            _shapeNoise = value;
            _shapeNoise.Changed += () => EmitSignal(SignalName.Changed);
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Strength of the shape noise effect on island shape.
    /// Higher values result in bigger deviations from circular shape.
    /// </summary>
    [ExportGroup("Shape")]
    [Export(PropertyHint.Range, "0.0,1.0,0.01")]
    public float ShapeNoiseStrength
    {
        get => _shapeNoiseStrength;
        set
        {
            if (Mathf.IsEqualApprox(_shapeNoiseStrength, value)) return;
            _shapeNoiseStrength = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Whether to apply biome noise to the tile's position before determining biome.
    /// Without this, biome transitions follow straight lines.
    /// </summary>
    [ExportGroup("Biome")]
    [Export]
    public bool ApplyBiomeNoiseToPos
    {
        get => _applyBiomeNoiseToPos;
        set
        {
            if (_applyBiomeNoiseToPos == value) return;
            _applyBiomeNoiseToPos = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Whether to apply biome noise to the tile's offset (distance from island center) before determining biome.
    /// Without this, biome transitions follow straight lines.
    /// </summary>
    [ExportGroup("Biome")]
    [Export]
    public bool ApplyBiomeNoiseToOffset
    {
        get => _applyBiomeNoiseToOffset;
        set
        {
            if (_applyBiomeNoiseToOffset == value) return;
            _applyBiomeNoiseToOffset = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Biome noise generator for biome variation.
    /// Can be configured for different noise types and parameters.
    /// </summary>
    [ExportGroup("Biome")]
    [Export]
    public Common.Extended.FastNoiseExt BiomeNoise
    {
        get => _biomeNoise;
        set
        {
            if (ReferenceEquals(_biomeNoise, value)) return;
            _biomeNoise = value;
            _biomeNoise.Changed += () => EmitSignal(SignalName.Changed);
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Strength of the biome noise effect on biome distribution.
    /// Higher values result in more mixed transitions between biomes.
    /// </summary>
    [ExportGroup("Biome")]
    [Export(PropertyHint.Range, "0.0,100.0,1")]
    public float BiomeNoiseStrength
    {
        get => _biomeNoiseStrength;
        set
        {
            if (Mathf.IsEqualApprox(_biomeNoiseStrength, value)) return;
            _biomeNoiseStrength = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// List of biomes that will be used for island generation.
    /// Island can contain multiple biomes of the same type to form several distinct regions.
    /// </summary>
    [ExportGroup("Biome")]
    [Export]
    public Godot.Collections.Array<string> Biomes
    {
        get => _biomes;
        set
        {
            if (ReferenceEquals(_biomes, value)) return;
            _biomes = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Minimum distance between biome centers as a fraction of island radius.
    /// Higher values result in larger, more distinct biome regions.
    /// Used by the Poisson-disc sampling algorithm when placing biome centers.
    /// </summary>
    [ExportGroup("Biome")]
    [Export(PropertyHint.Range, "0.0,1.0,0.01")]
    public float MinBiomeDistance
    {
        get => _minBiomeDistance;
        set
        {
            if (Mathf.IsEqualApprox(_minBiomeDistance, value)) return;
            _minBiomeDistance = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Minimum distance from shore for biome centers as a fraction of island radius.
    /// Higher values keep biome centers further inland.
    /// Used by the Poisson-disc sampling algorithm when placing biome centers.
    /// </summary>
    [ExportGroup("Biome")]
    [Export(PropertyHint.Range, "0.0,1.0,0.01")]
    public float MinBiomeShoreDistance
    {
        get => _minBiomeShoreDistance;
        set
        {
            if (Mathf.IsEqualApprox(_minBiomeShoreDistance, value)) return;
            _minBiomeShoreDistance = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Whether to show biome center points for debugging purposes.
    /// They are shown as "Center" biome (red color) on the island.
    /// They are calculated without the biome noise applied, so they represent the true centers.
    /// </summary>
    [ExportGroup("Biome")]
    [Export]
    public bool ShowBiomeCenters
    {
        get => _showBiomeCenters;
        set
        {
            if (_showBiomeCenters == value) return;
            _showBiomeCenters = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Rain noise generator for rain variation.
    /// Can be configured for different noise types and parameters.
    /// </summary>
    [ExportGroup("Rain")]
    [Export]
    public Common.Extended.FastNoiseExt RainNoise
    {
        get => _rainNoise;
        set
        {
            if (ReferenceEquals(_rainNoise, value)) return;
            _rainNoise = value;
            _rainNoise.Changed += () => EmitSignal(SignalName.Changed);
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Temperature noise generator for temperature variation.
    /// Can be configured for different noise types and parameters.
    /// </summary>
    [ExportGroup("Temperature")]
    [Export]
    public Common.Extended.FastNoiseExt TemperatureNoise
    {
        get => _rainNoise;
        set
        {
            if (ReferenceEquals(_temperatureNoise, value)) return;
            _temperatureNoise = value;
            _temperatureNoise.Changed += () => EmitSignal(SignalName.Changed);
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Altitude noise generator for altitude variation.
    /// Can be configured for different noise types and parameters.
    /// </summary>
    [ExportGroup("Altitude")]
    [Export]
    public Common.Extended.FastNoiseExt AltitudeNoise
    {
        get => _altitudeNoise;
        set
        {
            if (ReferenceEquals(_altitudeNoise, value)) return;
            _altitudeNoise = value;
            _altitudeNoise.Changed += () => EmitSignal(SignalName.Changed);
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// River noise generator for rivers.
    /// Can be configured for different noise types and parameters.
    /// </summary>
    [ExportGroup("River")]
    [Export]
    public FastNoiseLite RiverNoise
    {
        get => _riverNoise;
        set
        {
            if (ReferenceEquals(_riverNoise, value)) return;
            _riverNoise = value;
            _riverNoise.Changed += () => EmitSignal(SignalName.Changed);
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Dictionary mapping biome environment keys to sub-biome types.
    /// Used to determine specific sub-biome based on combined environmental factors.
    /// </summary>
    [ExportGroup("Biome")]
    [Export]
    public Godot.Collections.Dictionary<string, string> SubBiomes
    {
        get => _subBiomes;
        set
        {
            if (ReferenceEquals(_subBiomes, value)) return;
            _subBiomes = value;
            EmitSignal(SignalName.Changed);
        }
    }

    /// <summary>
    /// Initializes a new instance of the IslandSeedConf class with a random seed.
    /// </summary>
    public IslandSeedConf()
    {
        Seed = Utils.RNG.Instance.Int();
    }
}