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
using BankSimulator.CustomerInfoFiles;
using BankSimulator.Permissions;
using BankSimulator.Shared;

namespace BankSimulator.Blazor.Pages
{
    public partial class CustomerInfoFiles
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<CustomerInfoFileDto> CustomerInfoFileList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateCustomerInfoFile { get; set; }
        private bool CanEditCustomerInfoFile { get; set; }
        private bool CanDeleteCustomerInfoFile { get; set; }
        private CustomerInfoFileCreateDto NewCustomerInfoFile { get; set; }
        private Validations NewCustomerInfoFileValidations { get; set; } = new();
        private CustomerInfoFileUpdateDto EditingCustomerInfoFile { get; set; }
        private Validations EditingCustomerInfoFileValidations { get; set; } = new();
        private Guid EditingCustomerInfoFileId { get; set; }
        private Modal CreateCustomerInfoFileModal { get; set; } = new();
        private Modal EditCustomerInfoFileModal { get; set; } = new();
        private GetCustomerInfoFilesInput Filter { get; set; }
        private DataGridEntityActionsColumn<CustomerInfoFileDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "customerInfoFile-create-tab";
        protected string SelectedEditTab = "customerInfoFile-edit-tab";
        
        public CustomerInfoFiles()
        {
            NewCustomerInfoFile = new CustomerInfoFileCreateDto();
            EditingCustomerInfoFile = new CustomerInfoFileUpdateDto();
            Filter = new GetCustomerInfoFilesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            CustomerInfoFileList = new List<CustomerInfoFileDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:CustomerInfoFiles"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewCustomerInfoFile"], async () =>
            {
                await OpenCreateCustomerInfoFileModalAsync();
            }, IconName.Add, requiredPolicyName: BankSimulatorPermissions.CustomerInfoFiles.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateCustomerInfoFile = await AuthorizationService
                .IsGrantedAsync(BankSimulatorPermissions.CustomerInfoFiles.Create);
            CanEditCustomerInfoFile = await AuthorizationService
                            .IsGrantedAsync(BankSimulatorPermissions.CustomerInfoFiles.Edit);
            CanDeleteCustomerInfoFile = await AuthorizationService
                            .IsGrantedAsync(BankSimulatorPermissions.CustomerInfoFiles.Delete);
        }

        private async Task GetCustomerInfoFilesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await CustomerInfoFilesAppService.GetListAsync(Filter);
            CustomerInfoFileList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetCustomerInfoFilesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await CustomerInfoFilesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("BankSimulator") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/customer-info-files/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CustomerInfoFileDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetCustomerInfoFilesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateCustomerInfoFileModalAsync()
        {
            NewCustomerInfoFile = new CustomerInfoFileCreateDto{
                
                
            };
            await NewCustomerInfoFileValidations.ClearAll();
            await CreateCustomerInfoFileModal.Show();
        }

        private async Task CloseCreateCustomerInfoFileModalAsync()
        {
            NewCustomerInfoFile = new CustomerInfoFileCreateDto{
                
                
            };
            await CreateCustomerInfoFileModal.Hide();
        }

        private async Task OpenEditCustomerInfoFileModalAsync(CustomerInfoFileDto input)
        {
            var customerInfoFile = await CustomerInfoFilesAppService.GetAsync(input.Id);
            
            EditingCustomerInfoFileId = customerInfoFile.Id;
            EditingCustomerInfoFile = ObjectMapper.Map<CustomerInfoFileDto, CustomerInfoFileUpdateDto>(customerInfoFile);
            await EditingCustomerInfoFileValidations.ClearAll();
            await EditCustomerInfoFileModal.Show();
        }

        private async Task DeleteCustomerInfoFileAsync(CustomerInfoFileDto input)
        {
            await CustomerInfoFilesAppService.DeleteAsync(input.Id);
            await GetCustomerInfoFilesAsync();
        }

        private async Task CreateCustomerInfoFileAsync()
        {
            try
            {
                if (await NewCustomerInfoFileValidations.ValidateAll() == false)
                {
                    return;
                }

                await CustomerInfoFilesAppService.CreateAsync(NewCustomerInfoFile);
                await GetCustomerInfoFilesAsync();
                await CloseCreateCustomerInfoFileModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditCustomerInfoFileModalAsync()
        {
            await EditCustomerInfoFileModal.Hide();
        }

        private async Task UpdateCustomerInfoFileAsync()
        {
            try
            {
                if (await EditingCustomerInfoFileValidations.ValidateAll() == false)
                {
                    return;
                }

                await CustomerInfoFilesAppService.UpdateAsync(EditingCustomerInfoFileId, EditingCustomerInfoFile);
                await GetCustomerInfoFilesAsync();
                await EditCustomerInfoFileModal.Hide();                
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
        

    }
}
