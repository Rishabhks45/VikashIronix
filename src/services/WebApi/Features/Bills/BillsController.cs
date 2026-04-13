using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.DTOs.Billing;

namespace WebApi.Features.Bills
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillsController : ControllerBase
    {
        private readonly BillsService _service;

        public BillsController(BillsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetAll()
        {
            var bills = await _service.GetAllBillsAsync();
            return Ok(bills);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BillDto>> GetById(Guid id)
        {
            var bill = await _service.GetBillByIdAsync(id);
            if (bill == null) return NotFound();
            return Ok(bill);
        }

        [HttpPost]
        public async Task<ActionResult<BillSaveResult>> Create([FromBody] BillDto bill)
        {
            var result = await _service.CreateBillAsync(bill);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] BillDto bill)
        {
            bill.Id = id;
            var updatedId = await _service.UpdateBillAsync(bill);
            return Ok(updatedId);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _service.DeleteBillAsync(id);
            return NoContent();
        }
    }
}
