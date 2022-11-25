using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Sonar.AutoSwitch.Services;

public class StateManager
{
    private readonly string _appDataPath;

    public static StateManager Instance { get; } = new();

    private readonly Dictionary<Type, object?> _states = new();

    public StateManager()
    {
        _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SonarAutoSwitch");
    }

    public void SaveState<T>()
    {
        if (GetState<T>() is not { } state)
            return;

        if (!Directory.Exists(_appDataPath))
        {
            Directory.CreateDirectory(_appDataPath);
        }

        string jsonPath = Path.Combine(_appDataPath, typeof(T).Name + ".json");
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(state));
    }

    public T? GetState<T>()
    {
        if (_states.TryGetValue(typeof(T), out object? existing) && existing is T existingState)
            return existingState;
        return default;
    }

    public T? GetOrLoadState<T>() where T : new()
    {
        if (GetState<T>() is { } existingState)
            return existingState;

        string jsonPath = Path.Combine(_appDataPath, typeof(T).Name + ".json");
        T? loadState = !File.Exists(jsonPath) ? new T() : JsonSerializer.Deserialize<T>(File.ReadAllText(jsonPath));
        _states[typeof(T)] = loadState;
        return loadState;
    }
}