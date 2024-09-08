using BankSimulator.Localization;
using Volo.Abp.AspNetCore.Components;

namespace BankSimulator.Blazor;

public abstract class BankSimulatorComponentBase : AbpComponentBase
{
    protected BankSimulatorComponentBase()
    {
        LocalizationResource = typeof(BankSimulatorResource);
    }
}
