using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace World;

/// <summary>
/// Data class representing the game world.
/// Includes terrain tiles and other world-related data.
/// </summary>
public partial class World : Resource
{
    /// <summary>
    /// Signal emitted when world settings change, such as thresholds or the noise function.
    /// </summary>
    [Signal]
    public delegate void SettingsChangedEventHandler();

    /// <summary>
    /// The noise function used to generate terrain heights.
    /// </summary>
    [Export]
    public Common.Extended.FastNoiseExt Noise { get; private set; } = null;

    /// <summary>
    /// Mapping of coordinates to terrain tiles in the world.
    /// </summary>
    public Dictionary<Vector2I, Terrain.TerrainTile> TerrainTiles { get; private set; } = new();

    /// <summary>
    /// Backing field for <see cref="WaterLevel"/>.
    /// </summary>
    private float _waterLevel = 0.3f;

    /// <summary>
    /// Noise threshold for water terrain.
    /// Noise values below this level will be considered water.
    /// </summary>
    [Export]
    public float WaterLevel
    {
        get => _waterLevel;
        set
        {
            if (_waterLevel != value)
            {
                _waterLevel = value;
                TerrainTiles.Clear();
                EmitSignal(SignalName.SettingsChanged);
            }
        }
    }

    /// <summary>
    /// Backing field for <see cref="SandLevel"/>.
    /// </summary>
    private float _sandLevel = 0.4f;

    /// <summary>
    /// Noise threshold for sand terrain.
    /// Noise values between WaterLevel and SandLevel will be considered sand.
    /// </summary>
    [Export]
    public float SandLevel
    {
        get => _sandLevel;
        set
        {
            if (_sandLevel != value)
            {
                _sandLevel = value;
                TerrainTiles.Clear();
                EmitSignal(SignalName.SettingsChanged);
            }
        }
    }

    /// <summary>
    /// Backing field for <see cref="GrassLevel"/>.
    /// </summary>
    private float _grassLevel = 0.6f;

    /// <summary>
    /// Noise threshold for grass terrain.
    /// Noise values between SandLevel and GrassLevel will be considered grass.
    /// </summary>
    [Export]
    public float GrassLevel
    {
        get => _grassLevel;
        set
        {
            if (_grassLevel != value)
            {
                _grassLevel = value;
                TerrainTiles.Clear();
                EmitSignal(SignalName.SettingsChanged);
            }
        }
    }

    /// <summary>
    /// Backing field for <see cref="StoneLevel"/>.
    /// </summary>
    private float _stoneLevel = 1f;

    /// <summary>
    /// Noise threshold for stone terrain.
    /// Noise values between GrassLevel and StoneLevel will be considered stone.
    /// </summary>
    [Export]
    public float StoneLevel
    {
        get => _stoneLevel;
        set
        {
            if (_stoneLevel != value)
            {
                _stoneLevel = value;
                TerrainTiles.Clear();
                EmitSignal(SignalName.SettingsChanged);
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the World class and loads the noise resource.
    /// </summary>
    public World()
    {
        Noise = GD.Load<Common.Extended.FastNoiseExt>("res://Assets/Resources/island_generating_noise.tres");
        Noise.Changed += () => // Reload terrain tiles when noise settings change
        {
            TerrainTiles.Clear();
            EmitSignal(SignalName.SettingsChanged);
        };
    }

    /// <summary>
    /// Gets the terrain tile at the specified position.
    /// If the tile does not exist, it is generated based on the noise function and thresholds.
    /// </summary>
    /// <param name="position">The position to get the terrain tile for</param>
    /// <returns>The terrain tile at the specified position</returns>
    public Terrain.TerrainTile GetTileAt(Vector2I position)
    {
        if (!TerrainTiles.ContainsKey(position))
        {
            float n = Noise.GetNoise2Dv(position);
                string type;
            if (n < WaterLevel)
                type = "Water";
            else if (n < SandLevel)
                type = "Sand";
            else if (n < GrassLevel)
                type = "Grass";
            else if (n < StoneLevel)
                type = "Stone";
            else
                type = "Stone";
            TerrainTiles[position] = new Terrain.TerrainTile(type);
        }
        return TerrainTiles[position];
    }
}