using Volo.Abp.Settings;

namespace BankSimulator.Settings;

public class BankSimulatorSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(BankSimulatorSettings.MySetting1));
    }
}
