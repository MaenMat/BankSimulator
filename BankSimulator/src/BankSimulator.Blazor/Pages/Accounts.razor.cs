using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BankSimulator.Accounts;
using BankSimulator.Permissions;
using BankSimulator.Shared;

namespace BankSimulator.Blazor.Pages
{
    public partial class Accounts
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<AccountWithNavigationPropertiesDto> AccountList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateAccount { get; set; }
        private bool CanEditAccount { get; set; }
        private bool CanDeleteAccount { get; set; }
        private AccountCreateDto NewAccount { get; set; }
        private Validations NewAccountValidations { get; set; } = new();
        private AccountUpdateDto EditingAccount { get; set; }
        private Validations EditingAccountValidations { get; set; } = new();
        private Guid EditingAccountId { get; set; }
        private Modal CreateAccountModal { get; set; } = new();
        private Modal EditAccountModal { get; set; } = new();
        private GetAccountsInput Filter { get; set; }
        private DataGridEntityActionsColumn<AccountWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "account-create-tab";
        protected string SelectedEditTab = "account-edit-tab";
        private IReadOnlyList<LookupDto<Guid>> CustomerInfoFiles { get; set; } = new List<LookupDto<Guid>>();
        
        private string SelectedCustomerInfoFileId { get; set; }
        
        private string SelectedCustomerInfoFileText { get; set; }

        private List<LookupDto<Guid>> SelectedCustomerInfoFiles { get; set; } = new List<LookupDto<Guid>>();
        public Accounts()
        {
            NewAccount = new AccountCreateDto();
            EditingAccount = new AccountUpdateDto();
            Filter = new GetAccountsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            AccountList = new List<AccountWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Accounts"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewAccount"], async () =>
            {
                await OpenCreateAccountModalAsync();
            }, IconName.Add, requiredPolicyName: BankSimulatorPermissions.Accounts.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateAccount = await AuthorizationService
                .IsGrantedAsync(BankSimulatorPermissions.Accounts.Create);
            CanEditAccount = await AuthorizationService
                            .IsGrantedAsync(BankSimulatorPermissions.Accounts.Edit);
            CanDeleteAccount = await AuthorizationService
                            .IsGrantedAsync(BankSimulatorPermissions.Accounts.Delete);
        }

        private async Task GetAccountsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await AccountsAppService.GetListAsync(Filter);
            AccountList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetAccountsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await AccountsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("BankSimulator") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/accounts/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AccountWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetAccountsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateAccountModalAsync()
        {
            SelectedCustomerInfoFiles = new List<LookupDto<Guid>>();
            

            NewAccount = new AccountCreateDto{
                
                
            };
            await NewAccountValidations.ClearAll();
            await CreateAccountModal.Show();
        }

        private async Task CloseCreateAccountModalAsync()
        {
            NewAccount = new AccountCreateDto{
                
                
            };
            await CreateAccountModal.Hide();
        }

        private async Task OpenEditAccountModalAsync(AccountWithNavigationPropertiesDto input)
        {
            var account = await AccountsAppService.GetWithNavigationPropertiesAsync(input.Account.Id);
            
            EditingAccountId = account.Account.Id;
            EditingAccount = ObjectMapper.Map<AccountDto, AccountUpdateDto>(account.Account);
            SelectedCustomerInfoFiles = account.CustomerInfoFiles.Select(a => new LookupDto<Guid>{ Id = a.Id, DisplayName = a.CIFNumber}).ToList();

            await EditingAccountValidations.ClearAll();
            await EditAccountModal.Show();
        }

        private async Task DeleteAccountAsync(AccountWithNavigationPropertiesDto input)
        {
            await AccountsAppService.DeleteAsync(input.Account.Id);
            await GetAccountsAsync();
        }

        private async Task CreateAccountAsync()
        {
            try
            {
                if (await NewAccountValidations.ValidateAll() == false)
                {
                    return;
                }
                NewAccount.CustomerInfoFileIds = SelectedCustomerInfoFiles.Select(x => x.Id).ToList();


                await AccountsAppService.CreateAsync(NewAccount);
                await GetAccountsAsync();
                await CloseCreateAccountModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditAccountModalAsync()
        {
            await EditAccountModal.Hide();
        }

        private async Task UpdateAccountAsync()
        {
            try
            {
                if (await EditingAccountValidations.ValidateAll() == false)
                {
                    return;
                }
                EditingAccount.CustomerInfoFileIds = SelectedCustomerInfoFiles.Select(x => x.Id).ToList();


                await AccountsAppService.UpdateAsync(EditingAccountId, EditingAccount);
                await GetAccountsAsync();
                await EditAccountModal.Hide();                
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedCreateTab = name;
        }

        private void OnSelectedEditTabChanged(string name)
        {
            SelectedEditTab = name;
        }
        

        private async Task GetCustomerInfoFileLookupAsync(string? newValue = null)
        {
            CustomerInfoFiles = (await AccountsAppService.GetCustomerInfoFileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private void AddCustomerInfoFile()
        {
            if (SelectedCustomerInfoFileId.IsNullOrEmpty())
            {
                return;
            }
            
            if (SelectedCustomerInfoFiles.Any(p => p.Id.ToString() == SelectedCustomerInfoFileId))
            {
                UiMessageService.Warn(L["ItemAlreadyAdded"]);
                return;
            }

            SelectedCustomerInfoFiles.Add(new LookupDto<Guid>
            {
                Id = Guid.Parse(SelectedCustomerInfoFileId),
                DisplayName = SelectedCustomerInfoFileText
            });
        }

    }
}
