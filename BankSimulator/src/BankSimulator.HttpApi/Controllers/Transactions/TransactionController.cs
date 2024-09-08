using BankSimulator.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using BankSimulator.Transactions;
using Volo.Abp.Content;
using BankSimulator.Shared;

namespace BankSimulator.Controllers.Transactions
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Transaction")]
    [Route("api/app/transactions")]

    public class TransactionController : AbpController, ITransactionsAppService
    {
        private readonly ITransactionsAppService _transactionsAppService;

        public TransactionController(ITransactionsAppService transactionsAppService)
        {
            _transactionsAppService = transactionsAppService;
        }

        [HttpGet]
        public Task<PagedResultDto<TransactionWithNavigationPropertiesDto>> GetListAsync(GetTransactionsInput input)
        {
            return _transactionsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<TransactionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _transactionsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<TransactionDto> GetAsync(Guid id)
        {
            return _transactionsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("account-lookup")]
        public Task<PagedResultDto<LookupDto<Guid>>> GetAccountLookupAsync(LookupRequestDto input)
        {
            return _transactionsAppService.GetAccountLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<TransactionDto> CreateAsync(TransactionCreateDto input)
        {
            return _transactionsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<TransactionDto> UpdateAsync(Guid id, TransactionUpdateDto input)
        {
            return _transactionsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _transactionsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(TransactionExcelDownloadDto input)
        {
            return _transactionsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _transactionsAppService.GetDownloadTokenAsync();
        }
    }
}