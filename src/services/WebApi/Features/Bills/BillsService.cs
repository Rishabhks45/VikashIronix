using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedKernel.DTOs.Billing;
using WebApi.Features.Bills.Infrastructure;

namespace WebApi.Features.Bills
{
    public class BillsService
    {
        private readonly IBillsRepository _repository;

        public BillsService(IBillsRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<BillDto>> GetAllBillsAsync()
        {
            return _repository.GetAllBillsAsync();
        }

        public Task<BillDto> GetBillByIdAsync(Guid id)
        {
            return _repository.GetBillByIdAsync(id);
        }

        public Task<BillSaveResult> CreateBillAsync(BillDto bill)
        {
            return _repository.CreateBillAsync(bill);
        }

        public Task<Guid> UpdateBillAsync(BillDto bill)
        {
            return _repository.UpdateBillAsync(bill);
        }

        public Task<bool> DeleteBillAsync(Guid id)
        {
            return _repository.DeleteBillAsync(id);
        }
    }
}
