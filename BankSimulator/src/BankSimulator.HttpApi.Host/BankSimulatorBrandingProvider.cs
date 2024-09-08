using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace BankSimulator;

[Dependency(ReplaceServices = true)]
public class BankSimulatorBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "BankSimulator";
}
