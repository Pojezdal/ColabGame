using Godot;
using System;
using System.Collections.Generic;

namespace Entity.Plant;

/// <summary>
/// Represents the visual and interactive node for a plant in the game world.
/// </summary>
public partial class PlantNode : EntityNode
{
    /// <summary>
    /// Instantiates a new PlantNode for the given plant.
    /// </summary>
    /// <param name="plant">The plant to associate with the node.</param>
    /// <returns>A new PlantNode instance.</returns>
    public static PlantNode Instantiate(Plant plant)
    {
        var instance = Utils.AutoSceneInstantiator.Instantiate<PlantNode>();
        instance.Init(plant);
        return instance;
    }

    /// <summary>
    /// Initializes the PlantNode with the given plant.
    /// </summary>
    /// <param name="plant">The plant to associate with the node.</param>
    public void Init(Plant plant)
    {
        base.Init(plant);
    }
}
