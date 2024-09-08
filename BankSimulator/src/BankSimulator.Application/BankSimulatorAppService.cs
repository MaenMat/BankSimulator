using BankSimulator.Localization;
using Volo.Abp.Application.Services;

namespace BankSimulator;

/* Inherit your application services from this class.
 */
public abstract class BankSimulatorAppService : ApplicationService
{
    protected BankSimulatorAppService()
    {
        LocalizationResource = typeof(BankSimulatorResource);
    }
}
