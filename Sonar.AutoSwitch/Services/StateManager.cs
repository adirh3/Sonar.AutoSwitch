using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sonar.AutoSwitch.Services;

public class StateManager
{
    private readonly string _appDataPath;
    private readonly Dictionary<Type, object?> _states = new();
    private readonly DelayedDeduplicateAction _delayedDeduplicateAction = new();

    public static StateManager Instance { get; } = new();


    private StateManager()
    {
        _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Sonar.AutoSwitch");
    }

    public void SaveState<T>()
    {
        if (GetState<T>() is not { } state)
            return;

        if (!Directory.Exists(_appDataPath))
        {
            Directory.CreateDirectory(_appDataPath);
        }

        _delayedDeduplicateAction.QueueAction(async () =>
        {
            string jsonPath = Path.Combine(_appDataPath, typeof(T).Name + ".json");
#pragma warning disable IL2026
            await File.WriteAllTextAsync(jsonPath, JsonSerializer.Serialize(state));
#pragma warning restore IL2026
        });
    }


    public T GetOrLoadState<T>() where T : new()
    {
        if (GetState<T>() is { } existingState)
            return existingState;

        string jsonPath = Path.Combine(_appDataPath, typeof(T).Name + ".json");
#pragma warning disable IL2026
        T? loadState = !File.Exists(jsonPath) ? new T() : JsonSerializer.Deserialize<T>(File.ReadAllText(jsonPath));
#pragma warning restore IL2026
        _states[typeof(T)] = loadState;
        return loadState!;
    }

    public bool CheckStateExists<T>()
    {
        return File.Exists(Path.Combine(_appDataPath, typeof(T).Name + ".json"));
    }

    private T? GetState<T>()
    {
        if (_states.TryGetValue(typeof(T), out object? existing) && existing is T existingState)
            return existingState;
        return default;
    }
}