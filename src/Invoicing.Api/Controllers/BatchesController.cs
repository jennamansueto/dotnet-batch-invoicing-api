using System;
using System.Collections.Generic;
using System.Web.Http;
using Contoso.Invoicing.Application.Dtos;
using Contoso.Invoicing.Application.Services;

namespace Contoso.Invoicing.Api.Controllers
{
    [RoutePrefix("api/batches")]
    public class BatchesController : ApiController
    {
        private IBatchService BatchService =>
            (IBatchService)Configuration.Properties["IBatchService"];

        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateBatch([FromBody] CreateBatchRequest request)
        {
            if (request == null)
                return BadRequest("Request body is required.");

            try
            {
                var result = BatchService.CreateBatch(request);
                return Created($"api/batches/{result.BatchId}", result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("{batchId:guid}/run")]
        public IHttpActionResult RunBatch(Guid batchId)
        {
            try
            {
                var result = BatchService.RunBatch(batchId);
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

        [HttpGet]
        [Route("{batchId:guid}")]
        public IHttpActionResult GetBatch(Guid batchId)
        {
            try
            {
                var result = BatchService.GetBatch(batchId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{batchId:guid}/invoices")]
        public IHttpActionResult ListInvoices(Guid batchId)
        {
            try
            {
                var result = BatchService.ListInvoices(batchId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
