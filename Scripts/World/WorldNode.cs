using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace World;

/// <summary>
/// Node class representing the world in the scene.
/// Responsible for initializing and displaying the world data.
/// </summary>
public partial class WorldNode : Node2D
{
    /// <summary>
    /// The world data associated with this node.
    /// </summary>
    public World Data { get; private set; }

    [Export]
    public FastNoiseLite Noise;

    /// <summary>
    /// Instantiates a WorldNode with the provided world data.
    /// </summary>
    /// <param name="data">The world data to initialize the node with</param>
    /// <returns>The instantiated WorldNode</returns>
    public static WorldNode Instantiate(World data)
    {
        var instance = Utils.AutoSceneInstantiator.Instantiate<WorldNode>();
        instance.Init(data);
        return instance;
    }

    /// <summary>
    /// Initializes the WorldNode with the given world data.
    /// Sets up the terrain tiles based on the world data.
    /// </summary>
    /// <param name="data">The world data to initialize the node with</param>
    public void Init(World data)
    {
        Data = data;
        Noise = data.Noise;

        DrawTiles();

    }

    public void DrawTiles()
    {
        var terrainLayer = GetNode<TileMapLayer>("TerrainLayer");
        var terrainTileSet = terrainLayer.TileSet;
        var terrainMapping = new Dictionary<string, int>();
        for (int i = 0; i < terrainTileSet.GetTerrainsCount(0); i++)
            terrainMapping[terrainTileSet.GetTerrainName(0, i)] = i;
        foreach (var (coords, tile) in Data.TerrainTiles)
        {
            terrainLayer.SetCellsTerrainConnect([coords], 0, terrainMapping[tile.Name]);
        }
    }

    /// <summary>
    /// Called when the node is added to the scene.
    /// Initializes the world with a new instance of the World class for now.
    /// </summary>
    public override void _Ready()
    {
        Init(new World());
    }

    /// <summary>
    /// Simple input handling to demonstrate interaction with the world.
    /// Prints the tile type at the clicked position.
    /// </summary>
    /// <param name="event">The input event</param>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            Vector2 worldCoords = GetLocalMousePosition();
            Vector2I tileCoords = GetNode<TileMapLayer>("TerrainLayer").LocalToMap(worldCoords);
            GD.Print($"WorldNode: Mouse clicked at world position: {worldCoords}, tile coordinates: {tileCoords}, tile type: {Data.TerrainTiles.GetValueOrDefault(tileCoords)?.Name ?? "None"}");
        }

        else if (@event is InputEventMouseButton mouseEvent1 && mouseEvent1.ButtonIndex == MouseButton.WheelUp)
        {
            GetNode<Camera2D>("Camera2D").Zoom += new Vector2(0.1f, 0.1f);
        }

        else if (@event is InputEventMouseButton mouseEvent2 && mouseEvent2.ButtonIndex == MouseButton.WheelDown)
        {
            GetNode<Camera2D>("Camera2D").Zoom -= new Vector2(0.1f, 0.1f);
        }
    }

    public override void _Process(double delta)
    {
        Data.FillTiles();
        //DrawTiles();
    }

}
