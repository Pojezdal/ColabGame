using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace World;

/// <summary>
/// Node class representing the world in the scene.
/// Responsible for initializing and displaying the world data.
/// </summary>
public partial class WorldNode : Node2D
{
    /// <summary>
    /// Size of the chunk to load and unload at once. The chunk is a square with each side of this size.
    /// </summary>
    [Export]
    private int _chunkSize = 16;

    /// <summary>
    /// Number of chunks that will always be loaded around in each direction around the central chunk.
    /// This results in a square of chunks with each side of size 2 * _chunkViewDistance + 1.
    /// </summary>
    [Export]
    private int _chunkViewDistance = 2;

    /// <summary>
    /// Number of chunks outside of the view distance that will stay loaded. This is used to prevent
    /// the chunks from being unloaded immediately when out of view, because the player might return
    /// to the same area in a short time. Using this together with the chunk view distance results in
    /// a square of chunks with each side of size 2 * _chunkViewDistance + 1 + _chunkRetentionMargin.
    /// </summary>
    [Export]
    private int _chunkRetentionMargin = 1;

    /// <summary>
    /// Number of tiles to set per frame when loading or unloading chunks.
    /// This is used to prevent lagging when loading and unloading chunks.
    /// </summary>
    [Export]
    private int _batchSize = 128;

    [Export]
    /// <summary>
    /// The world data associated with this node.
    /// </summary>
    public World Data { get; private set; }

    /// <summary>
    /// Size of a single tile in the tile map.
    /// Loaded from the TileSet on initialization.
    /// </summary>
    private Vector2I _tileSize;

    /// <summary>
    /// The last center chunk position used to determine if chunks need to be updated.
    /// </summary>
    private Vector2I _lastCenterChunk = new Vector2I(int.MinValue, int.MinValue);

    /// <summary>
    /// Set of currently loaded chunk positions.
    /// </summary>
    private HashSet<Vector2I> _loadedChunks = new HashSet<Vector2I>();

    /// <summary>
    /// Mapping of terrain names to their corresponding terrain indices in the TileSet.
    /// </summary>
    private Dictionary<string, int> _terrainMapping = new Dictionary<string, int>();


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
        var timer = GetNode<Timer>("DebounceTimer");
        Data = data;
        Data.Changed += () =>
        {
            timer.Start();
        };
        timer.Timeout += () =>
        {
            UnloadAllChunks();
            UpdateChunks(_lastCenterChunk);
        };
        // Load tile size and terrain mapping from TileSet
        var terrainTileSet = GetNode<TileMapLayer>("TerrainLayer").TileSet;
        _tileSize = terrainTileSet.TileSize;
        _terrainMapping = new Dictionary<string, int>();
        for (int i = 0; i < terrainTileSet.GetTerrainsCount(0); i++)
            _terrainMapping[terrainTileSet.GetTerrainName(0, i)] = i;

        // Initial chunk update and subscribe to settings change to update chunks if the world changes
        UpdateChunks(Vector2I.Zero);
    }

    /// <summary>
    /// Converts a local position to a chunk position in the tile map.
    /// This is used to determine the chunk that contains the given local position.
    /// </summary>
    /// <param name="localPosition">Local position to convert.</param>
    /// <returns>Chunk position in the tile map.</returns>
    public Vector2I LocalToChunk(Vector2 localPosition)
    {
        return new Vector2I(Mathf.FloorToInt(localPosition.X / (_chunkSize * _tileSize.X)),
                            Mathf.FloorToInt(localPosition.Y / (_chunkSize * _tileSize.Y)));
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
            GD.Print($"WorldNode: Mouse clicked at world position: {worldCoords}, tile coordinates: {tileCoords}, chunk: {LocalToChunk(worldCoords)}, tile type: {Data.TerrainTiles.GetValueOrDefault(tileCoords)?.Name ?? "None"}");
        }
    }

    /// <summary>
    /// Called every frame to update the loaded chunks based on the camera position.
    /// </summary>
    /// <param name="delta">Time elapsed since the last frame</param>
    public override void _Process(double delta)
    {
        var camera = GetNode<WorldCamera>("WorldCamera");
        Vector2I centerChunk = LocalToChunk(camera.GlobalPosition);
        if (centerChunk != _lastCenterChunk)
        {
            _lastCenterChunk = centerChunk;
            UpdateChunks(centerChunk);
        }
    }

    /// <summary>
    /// Updates the loaded chunks based on the given center chunk.
    /// This can be used to load and unload chunks based on the player's position.
    /// </summary>
    /// <param name="centerChunk">Center chunk to use for loading and unloading chunks.</param>
    /// <remarks>
    /// This method is asynchronous to prevent lagging when loading and unloading chunks.
    /// Chunks are unloaded and loaded one after another and each chunks is furthermore handled
    /// over several frames, see <see cref="LoadChunk(Vector2I)"/> and <see cref="UnloadChunk(Vector2I)"/>. 
    /// </remarks>
    public async void UpdateChunks(Vector2I centerChunk)
    {
        GD.Print("Updating chunks");
        List<Vector2I> chunksToUnload = new List<Vector2I>();
        // Determine which chunks to unload based on their distance from the center chunk
        foreach (Vector2I chunk in _loadedChunks)
        {
            if (Mathf.Abs(chunk.X - centerChunk.X) > _chunkViewDistance + _chunkRetentionMargin ||
                Mathf.Abs(chunk.Y - centerChunk.Y) > _chunkViewDistance + _chunkRetentionMargin)
                chunksToUnload.Add(chunk);
        }
        foreach (Vector2I chunk in chunksToUnload)
            await UnloadChunk(chunk);

        // Load new chunks within the view distance
        for (int x = -_chunkViewDistance; x <= _chunkViewDistance; x++)
        {
            for (int y = -_chunkViewDistance; y <= _chunkViewDistance; y++)
            {
                Vector2I chunkPosition = centerChunk + new Vector2I(x, y);
                await LoadChunk(chunkPosition);
            }
        }
    }

    /// <summary>
    /// Asynchronously loads the chunk at the specified position. 
    /// </summary>
    /// <param name="chunkPosition">Chunk position to load.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method is asynchronous to prevent lagging when loading chunks.
    /// </remarks>
    public async Task LoadChunk(Vector2I chunkPosition)
    {
        if (_loadedChunks.Contains(chunkPosition))
            return;

        GD.Print($"Loading chunk at {chunkPosition}");
        var terrainLayer = GetNode<TileMapLayer>("TerrainLayer");
        // Gather terrain tiles in the chunk by type for efficient setting
        Dictionary<string, List<Vector2I>> terrains = new();
        for (int x = 0; x < _chunkSize; x++)
        {
            for (int y = 0; y < _chunkSize; y++)
            {
                Vector2I tilePosition = chunkPosition * _chunkSize + new Vector2I(x, y);
                var tile = Data.GetTileAt(tilePosition);
                if (!terrains.ContainsKey(tile.Name))
                    terrains[tile.Name] = new List<Vector2I>();
                terrains[tile.Name].Add(tilePosition);
            }
        }
        // Set terrain tiles in batches to prevent lagging, one terrain type at a time
        // This means that even with big batch size, the chunk takes at least as many frames to load
        // as there are different terrain types in the chunk
        foreach (var (name, tiles) in terrains)
        {
            int terrainIndex = _terrainMapping.GetValueOrDefault(name, -1);
            GD.Print($"Terrain {name} at chunk {chunkPosition} with {tiles.Count} tiles");
            for (int j = 0; j < tiles.Count; j += _batchSize)
            {
                var batch = tiles.Skip(j).Take(_batchSize);
                terrainLayer.SetCellsTerrainConnect([.. batch], 0, terrainIndex);
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            }
        }

        _loadedChunks.Add(chunkPosition);
    }

    /// <summary>
    /// Asynchronously unloads the chunk at the specified position.
    /// </summary>
    /// <param name="chunkPosition">Chunk position to unload.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method is asynchronous to prevent lagging when unloading chunks.
    /// </remarks>
    public async Task UnloadChunk(Vector2I chunkPosition)
    {
        if (!_loadedChunks.Contains(chunkPosition))
            return;

        GD.Print($"Unloading chunk at {chunkPosition}");
        var terrainLayer = GetNode<TileMapLayer>("TerrainLayer");
        // Gather all tile positions in the chunk to remove
        List<Vector2I> tilesToRemove = new List<Vector2I>();
        for (int x = 0; x < _chunkSize; x++)
        {
            for (int y = 0; y < _chunkSize; y++)
            {
                Vector2I tilePosition = chunkPosition * _chunkSize + new Vector2I(x, y);
                tilesToRemove.Add(tilePosition);
            }
        }
        // Remove terrain tiles in batches to prevent lagging
        for (int i = 0; i < tilesToRemove.Count; i += _batchSize)
        {
            var batch = tilesToRemove.Skip(i).Take(_batchSize);
            // Remove the batch of terrain tiles by setting their terrain index to -1,
            // much faster than using terrainLayer.EraseCell for each tile individually
            terrainLayer.SetCellsTerrainConnect([.. batch], 0, -1);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }

        _loadedChunks.Remove(chunkPosition);
    }

    /// <summary>
    /// Unloads all currently loaded chunks.
    /// Works much faster than unloading them one by one.
    /// </summary>
    public void UnloadAllChunks()
    {
        GD.Print("Clearing all loaded chunks");
        var terrainLayer = GetNode<TileMapLayer>("TerrainLayer");
        terrainLayer.Clear();
        _loadedChunks.Clear();
    }
}
