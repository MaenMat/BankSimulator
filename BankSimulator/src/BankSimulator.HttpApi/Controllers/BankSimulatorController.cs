using BankSimulator.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace BankSimulator.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class BankSimulatorController : AbpControllerBase
{
    protected BankSimulatorController()
    {
        LocalizationResource = typeof(BankSimulatorResource);
    }
}
