using Godot;
using System;
using System.Collections.Generic;

namespace Entity.Plant;

/// <summary>
/// Represents a carrot plant entity.
/// </summary>
public partial class Carrot : Plant
{
    /// <summary>
    /// Initializes a new instance of the Carrot class with the specified ID.
    /// Adds a default property for nutrients.
    /// </summary>
    /// <param name="id">The unique identifier for the carrot</param>
    public Carrot(string id) : base(id, new List<Property>() { new("nutrients", 30.0f) })
    {
    }

    /// <summary>
    /// Creates and returns a new CarrotNode associated with this carrot.
    /// </summary>
    /// <returns>A new CarrotNode instance associated with this carrot.</returns>
    public override CarrotNode CreateNode()
    {
        return CarrotNode.Instantiate(this);
    }

}
