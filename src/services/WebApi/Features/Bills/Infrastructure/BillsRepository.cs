using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SharedKernel.DTOs.Billing;
using SharedKernel.Services;

namespace WebApi.Features.Bills.Infrastructure
{
    public class BillsRepository : IBillsRepository
    {
        private readonly DbHelper _dbHelper;

        public BillsRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<IEnumerable<BillDto>> GetAllBillsAsync()
        {
            using var connection = _dbHelper.GetSaasDB();
            var parameters = new DynamicParameters();
            parameters.Add("@QueryType", 1);

            return await connection.QueryAsync<BillDto>(
                "usp_Bill_Management", 
                parameters, 
                commandType: CommandType.StoredProcedure);
        }

        public async Task<BillDto> GetBillByIdAsync(Guid id)
        {
            using var connection = _dbHelper.GetSaasDB();
            var parameters = new DynamicParameters();
            parameters.Add("@QueryType", 2);
            parameters.Add("@Id", id);

            using var multi = await connection.QueryMultipleAsync("usp_Bill_Management", parameters, commandType: CommandType.StoredProcedure);
            
            var bill = await multi.ReadSingleOrDefaultAsync<BillDto>();
            if (bill != null)
            {
                var items = await multi.ReadAsync<BillItemDto>();
                bill.Items = items.ToList();
            }

            return bill;
        }

        public async Task<BillSaveResult> CreateBillAsync(BillDto bill)
        {
            using var connection = _dbHelper.GetSaasDB();
            var parameters = new DynamicParameters();
            parameters.Add("@QueryType", 3);
            parameters.Add("@BillNumber", bill.BillNumber);
            parameters.Add("@CustomerId", bill.CustomerId);
            parameters.Add("@BillDate", bill.BillDate);
            parameters.Add("@SubTotal", bill.SubTotal);
            parameters.Add("@CGSTAmount", bill.CGSTAmount);
            parameters.Add("@SGSTAmount", bill.SGSTAmount);
            parameters.Add("@IGSTAmount", bill.IGSTAmount);
            parameters.Add("@BhadaAmount", bill.BhadaAmount);
            parameters.Add("@LabourAmount", bill.LabourAmount);
            parameters.Add("@RoundingAdjustment", bill.RoundingAdjustment);
            parameters.Add("@GrandTotal", bill.GrandTotal);
            parameters.Add("@CashPaid", bill.CashPaid);
            parameters.Add("@AmountPaid", bill.AmountPaid);
            parameters.Add("@BalanceDue", bill.BalanceDue);
            parameters.Add("@TotalWeightKg", bill.TotalWeightKg);
            parameters.Add("@Notes", bill.Notes);
            parameters.Add("@Status", bill.Status);
            
            string itemsJson = System.Text.Json.JsonSerializer.Serialize(bill.Items, new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });
            parameters.Add("@ItemsJson", itemsJson);

            return await connection.QuerySingleAsync<BillSaveResult>(
                "usp_Bill_Management", 
                parameters, 
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Guid> UpdateBillAsync(BillDto bill)
        {
            using var connection = _dbHelper.GetSaasDB();
            var parameters = new DynamicParameters();
            parameters.Add("@QueryType", 4);
            parameters.Add("@Id", bill.Id);
            parameters.Add("@CustomerId", bill.CustomerId);
            parameters.Add("@BillDate", bill.BillDate);
            parameters.Add("@SubTotal", bill.SubTotal);
            parameters.Add("@CGSTAmount", bill.CGSTAmount);
            parameters.Add("@SGSTAmount", bill.SGSTAmount);
            parameters.Add("@IGSTAmount", bill.IGSTAmount);
            parameters.Add("@BhadaAmount", bill.BhadaAmount);
            parameters.Add("@LabourAmount", bill.LabourAmount);
            parameters.Add("@RoundingAdjustment", bill.RoundingAdjustment);
            parameters.Add("@GrandTotal", bill.GrandTotal);
            parameters.Add("@CashPaid", bill.CashPaid);
            parameters.Add("@AmountPaid", bill.AmountPaid);
            parameters.Add("@BalanceDue", bill.BalanceDue);
            parameters.Add("@TotalWeightKg", bill.TotalWeightKg);
            parameters.Add("@Notes", bill.Notes);
            parameters.Add("@Status", bill.Status);
            
            if (bill.Items != null && bill.Items.Any()) 
            {
                string itemsJson = System.Text.Json.JsonSerializer.Serialize(bill.Items, new System.Text.Json.JsonSerializerOptions { PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase });
                parameters.Add("@ItemsJson", itemsJson);
            }

            return await connection.QuerySingleAsync<Guid>(
                "usp_Bill_Management", 
                parameters, 
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> DeleteBillAsync(Guid id)
        {
            using var connection = _dbHelper.GetSaasDB();
            var parameters = new DynamicParameters();
            parameters.Add("@QueryType", 5);
            parameters.Add("@Id", id);

            await connection.ExecuteAsync("usp_Bill_Management", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
    }
}
