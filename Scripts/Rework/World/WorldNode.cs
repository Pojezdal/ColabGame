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
    /// The size of each tile in the world.
    /// </summary>
    [Export]
    public Vector2I TileSize { get; private set; } = new(32, 32);

    /// <summary>
    /// Maps biome names to terrain indices.
    /// </summary>
    private readonly Dictionary<string, int> _biomeToTerrainMap = new();

    /// <summary>
    /// The world data associated with this node.
    /// </summary>
    public World Data { get; private set; }

    /// <summary>
    /// Initializes the world node with the given world data.
    /// </summary>
    /// <param name="data">The world data to initialize with.</param>
    public void Init(World data)
    {
        Data = data;
        Data.StaticEntityAdded += CreateEntityNode;
        Data.PlantSpawner.Spawn(new Entity.LivingEntity.Plant.Carrot("carrot_0"), new Vector2I(75, 75), 10);
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
        var tickTimer = GetNode<Timer>("TickTimer");
        tickTimer.Timeout += () => Data.Tick((float)tickTimer.WaitTime);
    }

    /// <summary>
    /// Called when the node is added to the scene tree.
    /// Right now, it initializes the world with the provided seed data.
    /// </summary>
    public override void _Ready()
    {
        Init(new World(SeedData));
    }

    /// <summary>
    /// Creates and adds an entity node to the scene for the given entity.
    /// </summary>
    /// <param name="entity">The entity for which to create the node.</param>
    private void CreateEntityNode(Entity.Entity entity)
    {
        var worldPos = entity.TilePosition * TileSize + TileSize / 2;
        var entityNode = entity.CreateNode(worldPos);
        GetNode<Node2D>("EntityLayer").AddChild(entityNode);
    }
}
