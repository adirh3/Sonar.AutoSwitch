using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sonar.AutoSwitch.Services;

public interface ISteelSeriesSonarService
{
    IEnumerable<SonarGamingConfiguration> GetGamingConfigurations();
    Task ChangeSelectedGamingConfiguration(SonarGamingConfiguration sonarGamingConfiguration);
}