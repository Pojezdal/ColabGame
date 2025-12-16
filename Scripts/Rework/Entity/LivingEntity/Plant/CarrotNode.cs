using Godot;
using System;

namespace Rework.Entity.LivingEntity.Plant;

/// <summary>
/// Represents the visual node for a Carrot plant in the game.
/// Inherits from PlantNode to ensure it is associated with a Carrot entity.
/// </summary>
public partial class CarrotNode : PlantNode
{

    /// <summary>
    /// Instantiates a CarrotNode for the given Carrot entity at the specified position.
    /// </summary>
    /// <param name="carrot">The carrot entity to associate with the node.</param>
    /// <param name="position">The position to place the node at.</param>
    /// <returns>The instantiated CarrotNode.</returns>
    public static CarrotNode Instantiate(Carrot carrot, Vector2 position)
    {
        var instance = Utils.AutoSceneInstantiator.Instantiate<CarrotNode>();
        instance.Init(carrot, position);
        return instance;
    }

    /// <summary>
    /// Initializes the CarrotNode with the given Carrot entity and position.
    /// </summary>
    /// <param name="carrot">The carrot to associate with the node.</param>
    /// <param name="position">The position to place the node at.</param>
    public void Init(Carrot carrot, Vector2 position)
    {
        base.Init(carrot, position);
    }
}
