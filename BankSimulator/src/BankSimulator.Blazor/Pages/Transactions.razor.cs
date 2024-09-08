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
using BankSimulator.Transactions;
using BankSimulator.Permissions;
using BankSimulator.Shared;

namespace BankSimulator.Blazor.Pages
{
    public partial class Transactions
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<TransactionWithNavigationPropertiesDto> TransactionList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateTransaction { get; set; }
        private bool CanEditTransaction { get; set; }
        private bool CanDeleteTransaction { get; set; }
        private TransactionCreateDto NewTransaction { get; set; }
        private Validations NewTransactionValidations { get; set; } = new();
        private TransactionUpdateDto EditingTransaction { get; set; }
        private Validations EditingTransactionValidations { get; set; } = new();
        private Guid EditingTransactionId { get; set; }
        private Modal CreateTransactionModal { get; set; } = new();
        private Modal EditTransactionModal { get; set; } = new();
        private GetTransactionsInput Filter { get; set; }
        private DataGridEntityActionsColumn<TransactionWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "transaction-create-tab";
        protected string SelectedEditTab = "transaction-edit-tab";
        private IReadOnlyList<LookupDto<Guid>> AccountsCollection { get; set; } = new List<LookupDto<Guid>>();

        public Transactions()
        {
            NewTransaction = new TransactionCreateDto();
            EditingTransaction = new TransactionUpdateDto();
            Filter = new GetTransactionsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            TransactionList = new List<TransactionWithNavigationPropertiesDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Transactions"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewTransaction"], async () =>
            {
                await OpenCreateTransactionModalAsync();
            }, IconName.Add, requiredPolicyName: BankSimulatorPermissions.Transactions.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateTransaction = await AuthorizationService
                .IsGrantedAsync(BankSimulatorPermissions.Transactions.Create);
            CanEditTransaction = await AuthorizationService
                            .IsGrantedAsync(BankSimulatorPermissions.Transactions.Edit);
            CanDeleteTransaction = await AuthorizationService
                            .IsGrantedAsync(BankSimulatorPermissions.Transactions.Delete);
        }

        private async Task GetTransactionsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await TransactionsAppService.GetListAsync(Filter);
            TransactionList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetTransactionsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await TransactionsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("BankSimulator") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/transactions/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TransactionWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetTransactionsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateTransactionModalAsync()
        {
            NewTransaction = new TransactionCreateDto{
                TransactionDate = DateTime.Now,

                
            };
            await NewTransactionValidations.ClearAll();
            await CreateTransactionModal.Show();
        }

        private async Task CloseCreateTransactionModalAsync()
        {
            NewTransaction = new TransactionCreateDto{
                TransactionDate = DateTime.Now,

                
            };
            await CreateTransactionModal.Hide();
        }

        private async Task OpenEditTransactionModalAsync(TransactionWithNavigationPropertiesDto input)
        {
            var transaction = await TransactionsAppService.GetWithNavigationPropertiesAsync(input.Transaction.Id);
            
            EditingTransactionId = transaction.Transaction.Id;
            EditingTransaction = ObjectMapper.Map<TransactionDto, TransactionUpdateDto>(transaction.Transaction);
            await EditingTransactionValidations.ClearAll();
            await EditTransactionModal.Show();
        }

        private async Task DeleteTransactionAsync(TransactionWithNavigationPropertiesDto input)
        {
            await TransactionsAppService.DeleteAsync(input.Transaction.Id);
            await GetTransactionsAsync();
        }

        private async Task CreateTransactionAsync()
        {
            try
            {
                if (await NewTransactionValidations.ValidateAll() == false)
                {
                    return;
                }

                await TransactionsAppService.CreateAsync(NewTransaction);
                await GetTransactionsAsync();
                await CloseCreateTransactionModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditTransactionModalAsync()
        {
            await EditTransactionModal.Hide();
        }

        private async Task UpdateTransactionAsync()
        {
            try
            {
                if (await EditingTransactionValidations.ValidateAll() == false)
                {
                    return;
                }

                await TransactionsAppService.UpdateAsync(EditingTransactionId, EditingTransaction);
                await GetTransactionsAsync();
                await EditTransactionModal.Hide();                
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
        

        private async Task GetAccountCollectionLookupAsync(string? newValue = null)
        {
            AccountsCollection = (await TransactionsAppService.GetAccountLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

    }
}
