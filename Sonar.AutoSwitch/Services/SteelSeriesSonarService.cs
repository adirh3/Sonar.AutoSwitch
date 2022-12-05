using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.Data.Sqlite;
using Sonar.AutoSwitch.Services.Win32;

namespace Sonar.AutoSwitch.Services;

public class SteelSeriesSonarService : ISteelSeriesSonarService
{
    private readonly string _connectionString;

    public SteelSeriesSonarService()
    {
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                @"SteelSeries\GG\apps\sonar\db\database.db")
        }.ToString();
    }

    public static SteelSeriesSonarService Instance { get; } = new();

    public IEnumerable<SonarGamingConfiguration> AvailableGamingConfigurations =>
        GetGamingConfigurations().OrderBy(s => s.Name);

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

        Process[] processesByName = Process.GetProcessesByName("SteelSeriesGGClient");
        if (processesByName.Length < 1)
            return;

        int? portById = processesByName.Select(p => NetworkHelper.GetPortById(p.Id)).FirstOrDefault(p => p != null);
        if (portById == null)
            return;

        var httpClient = new HttpClient();
        httpClient.PutAsync($"http://localhost:{portById}/configs/{sonarGamingConfiguration.Id}/select",
            new StringContent(""));
    }

    public string GetSelectedGamingConfiguration()
    {
        // Get all the available profiles from SQLite
        using var sqliteConnection = new SqliteConnection(_connectionString);
        sqliteConnection.Open();

        using SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
        sqliteCommand.CommandText = "select config_id, vad from selected_config where vad == 1";
        using SqliteDataReader sqliteDataReader = sqliteCommand.ExecuteReader();
        if (!sqliteDataReader.Read())
            throw new InvalidOperationException("Unable to check for selected gaming profile");
        return sqliteDataReader.GetString(0);
    }
}