﻿using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Requests;
using BPHN.ModelLayer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace BPHN.WebAPI.Controllers
{
    public class InvoicesController : BaseController
    {
        private readonly IInvoiceService _invoiceService;
        public InvoicesController(IServiceProvider provider) : base(provider)
        {
            _invoiceService = provider.GetRequiredService<IInvoiceService>();
        }

        [Permission(FunctionTypeEnum.VIEWLISTINVOICE)]
        [ApiAuthorize]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetInvoices(string txtSearch, string status, int? customerType, DateTime? date, int? paymentType)
        {
            Log.Debug($"Invoice/GetInvoices start: txtSearch: {txtSearch}, status: {status}, customerType: {customerType}, date: {date?.ToString("dd/MM/yyyy")}, paymentType: {paymentType}");
            return Ok(await _invoiceService.GetInvoices(txtSearch, status, customerType, date, paymentType));
        }

        [ApiAuthorize]
        [Route("get-instance")]
        [HttpGet]
        public async Task<IActionResult> GetInstance(string id)
        {
            Log.Debug($"Invoice/GetInstance start: {id}");
            return Ok(await _invoiceService.GetInstance(id));
        }

        [ApiAuthorize]
        [Route("get/{bookingDetailId}")]
        [HttpGet]
        public async Task<IActionResult> GetByBooking(string bookingDetailId)
        {
            Log.Debug($"Invoice/GetByBooking start: {bookingDetailId}");
            return Ok(await _invoiceService.GetByBooking(bookingDetailId));
        }

        [Permission(FunctionTypeEnum.ADDINVOICE)]
        [ApiAuthorize]
        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> Insert([FromBody] InsertInvoiceRequest request)
        {
            Log.Debug($"Invoice/Insert start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _invoiceService.Insert(_mapper.Map<Invoice>(request), request.BookingDetailId));
        }

        [Permission(FunctionTypeEnum.EDITINVOICE)]
        [ApiAuthorize]
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateInvoiceRequest request)
        {
            Log.Debug($"Invoice/Update start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _invoiceService.Update(_mapper.Map<Invoice>(request)));
        }
    }
}
