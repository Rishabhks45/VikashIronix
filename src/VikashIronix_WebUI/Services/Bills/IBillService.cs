using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedKernel.DTOs.Billing;

namespace VikashIronix_WebUI.Services.Bills
{
    public interface IBillService
    {
        Task<List<BillDto>> GetBillsAsync();
        Task<BillDto> GetBillByIdAsync(Guid id);
        Task<BillSaveResult> CreateBillAsync(BillDto bill);
        Task<Guid> UpdateBillAsync(BillDto bill);
        Task<bool> DeleteBillAsync(Guid id);
    }
}
