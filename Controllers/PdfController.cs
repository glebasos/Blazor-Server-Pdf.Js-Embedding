using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfSharpTest.DTOs;
using PdfSharpTest.Services;

namespace PdfSharpTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {

        List<InvoiceDto> invoices = new List<InvoiceDto>
            {
                new InvoiceDto
                {
                    InvoiceNumber = "INV-001",
                    InvoiceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)),
                    Address = "123 Main Street, Springfield",
                    Owners = "John Doe, Jane Doe",
                    RealEstateAka = "Apartment A-12",
                    Total = 250.75m,
                    Poses = new List<InvoicePosDto>
                    {
                        new InvoicePosDto
                        {
                            UtilityAka = "Electricity",
                            DateFrom = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)),
                            DateTo = DateOnly.FromDateTime(DateTime.Today),
                            Amount = 120.50m
                        },
                        new InvoicePosDto
                        {
                            UtilityAka = "Water",
                            DateFrom = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)),
                            DateTo = DateOnly.FromDateTime(DateTime.Today),
                            Amount = 80.25m
                        },
                        new InvoicePosDto
                        {
                            UtilityAka = "Gas",
                            DateFrom = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)),
                            DateTo = DateOnly.FromDateTime(DateTime.Today),
                            Amount = 50.00m
                        }
                    }
                },
                new InvoiceDto
                {
                    InvoiceNumber = "INV-002",
                    InvoiceDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)),
                    Address = "456 Oak Avenue, Shelbyville",
                    Owners = "Alice Smith",
                    RealEstateAka = "House #24",
                    Total = 180.00m,
                    Poses = new List<InvoicePosDto>
                    {
                        new InvoicePosDto
                        {
                            UtilityAka = "Electricity",
                            DateFrom = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)),
                            DateTo = DateOnly.FromDateTime(DateTime.Today),
                            Amount = 90.00m
                        },
                        new InvoicePosDto
                        {
                            UtilityAka = "Water",
                            DateFrom = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)),
                            DateTo = DateOnly.FromDateTime(DateTime.Today),
                            Amount = 60.00m
                        },
                        new InvoicePosDto
                        {
                            UtilityAka = "Internet",
                            DateFrom = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)),
                            DateTo = DateOnly.FromDateTime(DateTime.Today),
                            Amount = 30.00m
                        }
                    }
                }
            };

        [HttpGet("invoice-batch")]
        public IActionResult GetInvoiceBatchPdf([FromQuery] string[] invoiceNumbers)
        {
            var invoices = this.invoices;
            var pdfBytes = PdfService.CreateMultipagePdf(invoices);

            var fileName = $"Invoice_Batch_Report_{DateTime.Now:yyyy-MM-dd}.pdf";

            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");
            return File(pdfBytes, "application/pdf");
        }
    }
}
