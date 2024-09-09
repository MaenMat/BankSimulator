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
using BankSimulator.Otps;
using BankSimulator.Permissions;
using BankSimulator.Shared;

namespace BankSimulator.Blazor.Pages
{
    public partial class Otps
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<OtpDto> OtpList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateOtp { get; set; }
        private bool CanEditOtp { get; set; }
        private bool CanDeleteOtp { get; set; }
        private OtpCreateDto NewOtp { get; set; }
        private Validations NewOtpValidations { get; set; } = new();
        private OtpUpdateDto EditingOtp { get; set; }
        private Validations EditingOtpValidations { get; set; } = new();
        private Guid EditingOtpId { get; set; }
        private Modal CreateOtpModal { get; set; } = new();
        private Modal EditOtpModal { get; set; } = new();
        private GetOtpsInput Filter { get; set; }
        private DataGridEntityActionsColumn<OtpDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "otp-create-tab";
        protected string SelectedEditTab = "otp-edit-tab";
        
        public Otps()
        {
            NewOtp = new OtpCreateDto();
            EditingOtp = new OtpUpdateDto();
            Filter = new GetOtpsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            OtpList = new List<OtpDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
            await SetPermissionsAsync();
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Otps"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewOtp"], async () =>
            {
                await OpenCreateOtpModalAsync();
            }, IconName.Add, requiredPolicyName: BankSimulatorPermissions.Otps.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateOtp = await AuthorizationService
                .IsGrantedAsync(BankSimulatorPermissions.Otps.Create);
            CanEditOtp = await AuthorizationService
                            .IsGrantedAsync(BankSimulatorPermissions.Otps.Edit);
            CanDeleteOtp = await AuthorizationService
                            .IsGrantedAsync(BankSimulatorPermissions.Otps.Delete);
        }

        private async Task GetOtpsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await OtpsAppService.GetListAsync(Filter);
            OtpList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetOtpsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private  async Task DownloadAsExcelAsync()
        {
            var token = (await OtpsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("BankSimulator") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/otps/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OtpDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetOtpsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateOtpModalAsync()
        {
            NewOtp = new OtpCreateDto{
                ExpiryDate = DateTime.Now,

                
            };
            await NewOtpValidations.ClearAll();
            await CreateOtpModal.Show();
        }

        private async Task CloseCreateOtpModalAsync()
        {
            NewOtp = new OtpCreateDto{
                ExpiryDate = DateTime.Now,

                
            };
            await CreateOtpModal.Hide();
        }

        private async Task OpenEditOtpModalAsync(OtpDto input)
        {
            var otp = await OtpsAppService.GetAsync(input.Id);
            
            EditingOtpId = otp.Id;
            EditingOtp = ObjectMapper.Map<OtpDto, OtpUpdateDto>(otp);
            await EditingOtpValidations.ClearAll();
            await EditOtpModal.Show();
        }

        private async Task DeleteOtpAsync(OtpDto input)
        {
            await OtpsAppService.DeleteAsync(input.Id);
            await GetOtpsAsync();
        }

        private async Task CreateOtpAsync()
        {
            try
            {
                if (await NewOtpValidations.ValidateAll() == false)
                {
                    return;
                }

                await OtpsAppService.CreateAsync(NewOtp);
                await GetOtpsAsync();
                await CloseCreateOtpModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditOtpModalAsync()
        {
            await EditOtpModal.Hide();
        }

        private async Task UpdateOtpAsync()
        {
            try
            {
                if (await EditingOtpValidations.ValidateAll() == false)
                {
                    return;
                }

                await OtpsAppService.UpdateAsync(EditingOtpId, EditingOtp);
                await GetOtpsAsync();
                await EditOtpModal.Hide();                
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
