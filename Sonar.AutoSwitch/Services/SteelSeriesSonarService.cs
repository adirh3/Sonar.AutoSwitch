using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace Sonar.AutoSwitch.Services;

public class SteelSeriesSonarService : ISteelSeriesSonarService
{
    private readonly string _connectionString;

    public static SteelSeriesSonarService Instance { get; } = new();

    public SteelSeriesSonarService()
    {
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                @"SteelSeries\GG\sonar\db\database.db")
        }.ToString();
    }

    public IEnumerable<SonarGamingConfiguration> AvailableGamingConfigurations => GetGamingConfigurations();

    public SonarGamingConfiguration GetSelectedGamingConfiguration()
    {
        // Get all the available profiles from SQLite
        using var sqliteConnection = new SqliteConnection(_connectionString);
        sqliteConnection.Open();

        using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
        sqliteCommand.CommandText = "select id, name, vad from configs where vad == 1";
        using SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
        if (!sqliteDataReader.Read())
            throw new InvalidOperationException("Unable to check for selected gaming profile");
        string id = sqliteDataReader.GetString(0);
        string name = sqliteDataReader.GetString(1);
        return new SonarGamingConfiguration(id, name);
    }

    public IEnumerable<SonarGamingConfiguration> GetGamingConfigurations()
    {
        // Get all the available profiles from SQLite
        using var sqliteConnection = new SqliteConnection(_connectionString);
        sqliteConnection.Open();

        using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
        sqliteCommand.CommandText = "select id, name, vad from configs where vad == 1";
        using SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
        while (sqliteDataReader.Read())
        {
            string id = sqliteDataReader.GetString(0);
            string name = sqliteDataReader.GetString(1);
            yield return new SonarGamingConfiguration(id, name);
        }
    }

    public void ChangeSelectedGamingConfiguration(SonarGamingConfiguration sonarGamingConfiguration)
    {
        if (string.IsNullOrEmpty(sonarGamingConfiguration.Id))
            return;

        Process[] processesByName = Process.GetProcessesByName("SteelSeriesSonar");
        if (!processesByName.Any())
            return;
        // Kill sonar process
        using Process sonarProcess = processesByName.First();
        string sonarPath = sonarProcess.MainModule!.FileName!;
        sonarProcess.Kill();

        // Change the config Id in SQLite
        using var sqliteConnection = new SqliteConnection(_connectionString);
        sqliteConnection.Open();
        using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
        sqliteCommand.CommandText = "update selected_config set config_id = $id where vad == 1";
        sqliteCommand.Parameters.AddWithValue("$id", sonarGamingConfiguration.Id);
        sqliteCommand.ExecuteNonQuery();

        // Start sonar again
        Process.Start(sonarPath).Dispose();
    }
}