namespace PdfSharpTest.DTOs
{
    public class InvoicePosDto
    {
        public string UtilityAka { get; set; }
        public DateOnly DateFrom  { get; set; }
        public DateOnly DateTo  { get; set; }
        public decimal Amount  { get; set; }
    }
}
