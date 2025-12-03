using Godot;
using System;

namespace Entity.Plant;

/// <summary>
/// Represents the visual and interactive node for a carrot plant in the game world.
/// </summary>
public partial class CarrotNode : PlantNode
{
    /// <summary>
    /// The data associated with this carrot node.
    /// </summary>
    private Carrot _data = null;

    /// <summary>
    /// Instantiates a new CarrotNode for the given carrot.
    /// </summary>
    /// <param name="carrot">The carrot to associate with the node.</param>
    /// <returns>A new CarrotNode instance.</returns>
    public static CarrotNode Instantiate(Carrot carrot)
    {
        var instance = Utils.AutoSceneInstantiator.Instantiate<CarrotNode>();
        instance.Init(carrot);
        return instance;
    }

    /// <summary>
    /// Initializes the CarrotNode with the given carrot data.
    /// </summary>
    /// <param name="carrot">The carrot to associate with the node.</param>
    public void Init(Carrot carrot)
    {
        base.Init(carrot);
        _data = carrot;
    }
}
