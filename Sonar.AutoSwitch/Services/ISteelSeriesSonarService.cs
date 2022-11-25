using System.Collections.Generic;

namespace Sonar.AutoSwitch.Services;

public interface ISteelSeriesSonarService
{
    IEnumerable<SonarGamingConfiguration> GetGamingConfigurations();
    void ChangeSelectedGamingConfiguration(SonarGamingConfiguration sonarGamingConfiguration);
}