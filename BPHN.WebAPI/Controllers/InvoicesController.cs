using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Requests;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(await _invoiceService.GetInvoices(txtSearch, status, customerType, date, paymentType));
        }

        [ApiAuthorize]
        [Route("get-instance")]
        [HttpGet]
        public async Task<IActionResult> GetInstance(string id)
        {
            return Ok(await _invoiceService.GetInstance(id));
        }

        [ApiAuthorize]
        [Route("get/{bookingDetailId}")]
        [HttpGet]
        public async Task<IActionResult> GetByBooking(string bookingDetailId)
        {
            return Ok(await _invoiceService.GetByBooking(bookingDetailId));
        }

        [Permission(FunctionTypeEnum.ADDINVOICE)]
        [ApiAuthorize]
        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> Insert([FromBody] InsertInvoiceRequest request)
        {
            return Ok(await _invoiceService.Insert(_mapper.Map<Invoice>(request), request.BookingDetailId));
        }

        [Permission(FunctionTypeEnum.EDITINVOICE)]
        [ApiAuthorize]
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateInvoiceRequest request)
        {
            return Ok(await _invoiceService.Update(_mapper.Map<Invoice>(request)));
        }
    }
}
