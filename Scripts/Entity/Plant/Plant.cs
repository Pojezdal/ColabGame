using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Entity.Plant;

/// <summary>
/// Represents a plant entity.
/// </summary>
public abstract partial class Plant : Entity
{
    /// <summary>
    /// Initializes a new instance of the Plant class with the specified ID and properties.
    /// Adds default properties for reproduction time and radius.
    /// </summary>
    /// <param name="id">The unique identifier for the plant</param>
    /// <param name="properties">The properties associated with the plant</param>
    public Plant(string id, IEnumerable<Property> properties) : base(id,
        properties.Concat(new List<Property>() { new("reproduction_time", 5), new("reproduction_radius", 5) }))
    {
    }
}