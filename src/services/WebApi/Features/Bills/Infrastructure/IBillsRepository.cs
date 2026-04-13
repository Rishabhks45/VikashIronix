using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedKernel.DTOs.Billing;

namespace WebApi.Features.Bills.Infrastructure
{
    public interface IBillsRepository
    {
        Task<IEnumerable<BillDto>> GetAllBillsAsync();
        Task<BillDto> GetBillByIdAsync(Guid id);
        Task<BillSaveResult> CreateBillAsync(BillDto bill);
        Task<Guid> UpdateBillAsync(BillDto bill);
        Task<bool> DeleteBillAsync(Guid id);
    }
}
