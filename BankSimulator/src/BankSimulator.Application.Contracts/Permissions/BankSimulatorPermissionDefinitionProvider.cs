using BankSimulator.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace BankSimulator.Permissions;

public class BankSimulatorPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(BankSimulatorPermissions.GroupName);

        myGroup.AddPermission(BankSimulatorPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(BankSimulatorPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(BankSimulatorPermissions.MyPermission1, L("Permission:MyPermission1"));

        var customerInfoFilePermission = myGroup.AddPermission(BankSimulatorPermissions.CustomerInfoFiles.Default, L("Permission:CustomerInfoFiles"));
        customerInfoFilePermission.AddChild(BankSimulatorPermissions.CustomerInfoFiles.Create, L("Permission:Create"));
        customerInfoFilePermission.AddChild(BankSimulatorPermissions.CustomerInfoFiles.Edit, L("Permission:Edit"));
        customerInfoFilePermission.AddChild(BankSimulatorPermissions.CustomerInfoFiles.Delete, L("Permission:Delete"));

        var accountPermission = myGroup.AddPermission(BankSimulatorPermissions.Accounts.Default, L("Permission:Accounts"));
        accountPermission.AddChild(BankSimulatorPermissions.Accounts.Create, L("Permission:Create"));
        accountPermission.AddChild(BankSimulatorPermissions.Accounts.Edit, L("Permission:Edit"));
        accountPermission.AddChild(BankSimulatorPermissions.Accounts.Delete, L("Permission:Delete"));

        var transactionPermission = myGroup.AddPermission(BankSimulatorPermissions.Transactions.Default, L("Permission:Transactions"));
        transactionPermission.AddChild(BankSimulatorPermissions.Transactions.Create, L("Permission:Create"));
        transactionPermission.AddChild(BankSimulatorPermissions.Transactions.Edit, L("Permission:Edit"));
        transactionPermission.AddChild(BankSimulatorPermissions.Transactions.Delete, L("Permission:Delete"));

        var otpPermission = myGroup.AddPermission(BankSimulatorPermissions.Otps.Default, L("Permission:Otps"));
        otpPermission.AddChild(BankSimulatorPermissions.Otps.Create, L("Permission:Create"));
        otpPermission.AddChild(BankSimulatorPermissions.Otps.Edit, L("Permission:Edit"));
        otpPermission.AddChild(BankSimulatorPermissions.Otps.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<BankSimulatorResource>(name);
    }
}