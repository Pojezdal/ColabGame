using Godot;
using System;
using System.Collections.Generic;

namespace Entity;

/// <summary>
/// Represents a property of an entity.
/// These are variables that define the state of the entity and can change over time
/// or due to interactions with other entities (e.g., health, hunger, energy, age).
/// </summary>
public partial class Property : RefCounted
{
    /// <summary>
    /// The name of the property.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The value of the property.
    /// </summary>
    public float Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Property"/> class.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="value">The initial value of the property.</param>
    public Property(string name, float value)
    {
        Name = name;
        Value = value;
    }
}