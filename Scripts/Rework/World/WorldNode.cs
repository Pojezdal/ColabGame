using Godot;
using System;
using System.Collections.Generic;

namespace Rework.World;

/// <summary>
/// The main node representing the world in the scene tree.
/// </summary>
public partial class WorldNode : Node2D
{
    /// <summary>
    /// The seed data used for world generation.
    /// </summary>
    [Export]
    public WorldSeed SeedData { get; private set; } = null;

    /// <summary>
    /// Maps biome names to terrain indices.
    /// </summary>
    private readonly Dictionary<string, int> _biomeToTerrainMap = new();

    /// <summary>
    /// Initializes the world node with the given world data.
    /// </summary>
    /// <param name="data">The world data to initialize with.</param>
    public void Init(World data)
    {
        var worldlayer = GetNode<TileMapLayer>("WorldLayer");
        var overlaylayerHM = GetNode<TileMapLayer>("OverlayLayerHM");
        overlaylayerHM.ChildEnteredTree += (Node child) =>
        {
            if (child is TileOverlay cell)
            {
                var gridPosition = overlaylayerHM.LocalToMap(cell.Position);
                cell.Init(data.Tiles[gridPosition], "HM");
            }
        };
        var overlaylayerTF = GetNode<TileMapLayer>("OverlayLayerTF");
        overlaylayerTF.ChildEnteredTree += (Node child) =>
        {
            if (child is TileOverlay cell)
            {
                var gridPosition = overlaylayerTF.LocalToMap(cell.Position);
                cell.Init(data.Tiles[gridPosition], "TF");
            }
        };

        for (int i = 0; i < worldlayer.TileSet.GetTerrainsCount(0); i++)
            _biomeToTerrainMap[worldlayer.TileSet.GetTerrainName(0, i)] = i;

        foreach (var tile in data.Tiles.Values)
        {
            worldlayer.SetCellsTerrainConnect([tile.Position], 0, _biomeToTerrainMap[tile.Biome.Name]);
            overlaylayerHM.SetCell(tile.Position, 0, Vector2I.Zero, 1);
            overlaylayerTF.SetCell(tile.Position, 0, Vector2I.Zero, 1);
        }
    }

    /// <summary>
    /// Called when the node is added to the scene tree.
    /// Right now, it initializes the world with the provided seed data.
    /// </summary>
    public override void _Ready()
    {
        Init(new World(SeedData));
    }
}
