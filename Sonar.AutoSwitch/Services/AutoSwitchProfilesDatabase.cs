using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Sonar.AutoSwitch.ViewModels;

namespace Sonar.AutoSwitch.Services;

public class AutoSwitchProfilesDatabase
{
    public static AutoSwitchProfilesDatabase Instance { get; } = new();

    public async Task LoadDatabaseAsync()
    {
        try
        {
            using var httpClient = new HttpClient();
            using HttpResponseMessage httpResponseMessage =
                await httpClient.GetAsync(
                    "https://raw.githubusercontent.com/adirh3/Sonar.AutoSwitch/master/game_database.json");
            await using Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var githubConfigs =
                await JsonSerializer.DeserializeAsync<Dictionary<string, WindowInfo>>(stream);
            var switchProfileViewModels = new List<AutoSwitchProfileViewModel>();
            foreach ((string? key, (string? exeName, string? title)) in githubConfigs!)
            {
                SonarGamingConfiguration? sonarGamingConfiguration =
                    SteelSeriesSonarService.Instance.AvailableGamingConfigurations.FirstOrDefault(s => s.Name == key);
                if (sonarGamingConfiguration == null) continue;
                switchProfileViewModels.Add(new AutoSwitchProfileViewModel
                {
                    Title = title,
                    ExeName = exeName!,
                    SonarGamingConfiguration = sonarGamingConfiguration
                });
            }

            GithubProfiles = switchProfileViewModels;
        }
        catch (Exception)
        {
            // ignored, network error
        }
    }

    public List<AutoSwitchProfileViewModel> GithubProfiles { get; private set; } = new();
}