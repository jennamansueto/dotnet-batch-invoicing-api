using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contoso.Invoicing.Application.Dtos;
using Contoso.Invoicing.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Invoicing.Api.Controllers
{
    [ApiController]
    [Route("api/batches")]
    public class BatchesController : ControllerBase
    {
        private readonly IBatchService _batchService;

        public BatchesController(IBatchService batchService)
        {
            _batchService = batchService;
        }

        [HttpPost("")]
        public async Task<ActionResult<BatchSummaryDto>> CreateBatch([FromBody] CreateBatchRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            try
            {
                var result = await _batchService.CreateBatchAsync(request);
                return Created($"api/batches/{result.BatchId}", result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{batchId:guid}/run")]
        public async Task<ActionResult<BatchSummaryDto>> RunBatch(Guid batchId)
        {
            try
            {
                var result = await _batchService.RunBatchAsync(batchId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{batchId:guid}")]
        public async Task<ActionResult<BatchSummaryDto>> GetBatch(Guid batchId)
        {
            try
            {
                var result = await _batchService.GetBatchAsync(batchId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{batchId:guid}/invoices")]
        public async Task<ActionResult<System.Collections.Generic.IReadOnlyList<InvoiceDto>>> ListInvoices(Guid batchId)
        {
            try
            {
                var result = await _batchService.ListInvoicesAsync(batchId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
