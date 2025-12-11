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
    /// <param name="info">The type of information to display ("HM" for height/moisture, "TF" for temperature/fertility).</param>
    public void Init(Tile tile, string info)
    {
        _data = tile;
        if (info == "HM")
            GetNode<Label>("Label").Text = $"H:{tile.Properties["height"]:0.##}\nM:{tile.Properties["moisture"]:0.##}";
        else if (info == "TF")
            GetNode<Label>("Label").Text = $"T:{tile.Properties["temperature"]:0.##}\nF:{tile.Properties["fertility"]:0.##}";
    }
}
