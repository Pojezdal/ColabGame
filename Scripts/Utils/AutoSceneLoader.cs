using Godot;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Utils;

/// <summary>
/// Utility class for automatically loading scenes based on script file paths.
/// Used by <see cref="AutoSceneInstantiator"/> to load scenes without hardcoding paths.
/// </summary>
public static class AutoSceneLoader
{
    /// <summary>
    /// Loads a PackedScene based on the provided script file path.
    /// Can be used without the argument - CallerFilePath attribute will supply it automatically.
    /// </summary>
    /// <typeparam name="T">The type of the scene class</typeparam>
    /// <param name="scriptFilePath">The full path to the script file</param>
    /// <returns>The loaded PackedScene</returns>
    public static PackedScene LoadScene<T>([CallerFilePath] string scriptFilePath = "") where T : Node
    {
        string scenePath = ConvertScriptPathToScenePath(scriptFilePath);

        var packedScene = GD.Load<PackedScene>(scenePath);
        if (packedScene == null)
        {
            GD.PrintErr($"Failed to load scene at path: {scenePath}");
            throw new FileNotFoundException($"Scene file not found: {scenePath}");
        }

        return packedScene;
    }

    /// <summary>
    /// Converts a script file path to the corresponding scene file path.
    /// Example: C:\Project\Scripts\Character\Player.cs -> res://Scenes/Character/Player.tscn
    /// </summary>
    /// <param name="scriptFilePath">Full path to the script file</param>
    /// <returns>Godot resource path to the scene file</returns>
    private static string ConvertScriptPathToScenePath(string scriptFilePath)
    {
        // Convert to forward slashes for consistency
        scriptFilePath = scriptFilePath.Replace('\\', '/');

        // Find the Scripts folder in the path
        int scriptsIndex = scriptFilePath.LastIndexOf("/Scripts/");
        if (scriptsIndex == -1)
        {
            // Fallback: try to use the class name directly
            GD.PrintErr($"Could not find 'Scripts' folder in path: {scriptFilePath}!");
            throw new ArgumentException($"Invalid script file path: {scriptFilePath}. 'Scripts' folder not found.");
        }

        // Extract the part after Scripts/
        string relativePath = scriptFilePath.Substring(scriptsIndex + "/Scripts/".Length);

        // Get the directory and file name
        string directory = Path.GetDirectoryName(relativePath)?.Replace('\\', '/') ?? "";
        string fileName = Path.GetFileNameWithoutExtension(relativePath);

        // Build the scene path
        string scenePath = string.IsNullOrEmpty(directory)
            ? $"res://Scenes/{fileName}.tscn"
            : $"res://Scenes/{directory}/{fileName}.tscn";

        return scenePath;
    }
}