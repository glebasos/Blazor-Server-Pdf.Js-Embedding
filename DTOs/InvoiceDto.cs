namespace PdfSharpTest.DTOs
{
    public class InvoiceDto
    {
        public string InvoiceNumber { get; set; }
        public DateOnly InvoiceDate {  get; set; }
        public string Address { get; set; }
        public string Owners { get; set; }
        public string RealEstateAka { get; set; }
        public decimal Total { get; set; }

        public List<InvoicePosDto> Poses { get; set; }
    }
}
