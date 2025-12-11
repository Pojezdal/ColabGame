using Godot;
using System;

namespace Rework.World;

/// <summary>
/// A visual overlay for a tile, displaying additional information.
/// </summary>
public partial class TileOverlay : Node2D
{
    private Tile _data = null;

    /// <summary>
    /// Initializes the overlay with the given tile data.
    /// </summary>
    /// <param name="tile">The tile data to display.</param>
    public void Init(Tile tile)
    {
        _data = tile;
        GetNode<Label>("Label").Text = $"H:{tile.NoiseValues["height"]:0.##}\nM:{tile.NoiseValues["moisture"]:0.##}";
    }
}
