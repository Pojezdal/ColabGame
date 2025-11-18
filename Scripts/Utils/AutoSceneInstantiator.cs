using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Utils;

/// <summary>
/// Static helper class that provides automatic scene instantiation for any Node type.
/// The <see cref="Instantiate"/> should be called from the node's instantiating method within its own script file
/// to ensure correct scene path resolution.
/// </summary>
public static class AutoSceneInstantiator
{
    /// <summary>
    /// Cached PackedScenes - loaded once per type and reused
    /// </summary>
    private static Dictionary<Type, PackedScene> _cachedScenes = new();

    /// <summary>
    /// Lock object for thread-safe lazy initialization
    /// </summary>
    private static readonly object _lock = new object();

    /// <summary>
    /// Gets the cached PackedScene for type T, loading it if not already cached.
    /// </summary>
    /// <typeparam name="T">The type of the scene class</typeparam>
    /// <param name="callerFilePath">The full path to the script file</param>
    /// <returns>The cached PackedScene</returns>
    public static PackedScene GetScene<T>([CallerFilePath] string callerFilePath = "") where T : Node
    {
        // Fast path: if scene is already cached, return it immediately
        if (_cachedScenes.TryGetValue(typeof(T), out var cachedScene))
            return cachedScene;

        // Slow path: scene needs to be loaded and cached
        lock (_lock)
        {
            // Double-check: another thread might have loaded it while we were waiting for the lock
            if (!_cachedScenes.TryGetValue(typeof(T), out cachedScene))
            {
                cachedScene = AutoSceneLoader.LoadScene<T>(callerFilePath);
                _cachedScenes[typeof(T)] = cachedScene;
                GD.Print($"AutoSceneInstantiator: Cached scene for {typeof(T).Name}");
            }
        }
        return cachedScene;
    }

    /// <summary>
    /// Automatically instantiates the scene for type T using cached PackedScene.
    /// Scene is loaded once and cached for maximum performance.
    /// </summary>
    /// <typeparam name="T">The type of the scene class</typeparam>
    /// <param name="callerFilePath">The full path to the script file</param
    /// <returns>A new instance of T</returns>
    public static T Instantiate<T>([CallerFilePath] string callerFilePath = "") where T : Node
    {
        var cachedScene = GetScene<T>(callerFilePath);
        return cachedScene.Instantiate<T>();
    }

    /// <summary>
    /// Clears the cached PackedScene for type T, if it exists.
    /// </summary>
    /// <typeparam name="T">The type of the scene class</typeparam>
    public static void ClearCache<T>()
    {
        lock (_lock)
        {
            if (_cachedScenes.TryGetValue(typeof(T), out var cachedScene))
            {
                cachedScene.Dispose();
                _cachedScenes.Remove(typeof(T));
                GD.Print($"AutoSceneInstantiator: Cleared cache for {typeof(T).Name}");
            }
        }
    }

    /// <summary>
    /// Clears all cached PackedScenes.
    /// </summary>
    public static void ClearAllCache()
    {
        lock (_lock)
        {
            foreach (var scene in _cachedScenes.Values)
                scene.Dispose();

            _cachedScenes.Clear();
            GD.Print("AutoSceneInstantiator: Cleared all cached scenes");
        }
    }
}
