using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Fields;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using PdfSharp.Snippets.Font;
using PdfSharpTest.DTOs;

namespace PdfSharpTest.Services
{
    public class PdfService
    {

        public static byte[] CreateMultipagePdf(List<InvoiceDto> invoices)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    if (Capabilities.Build.IsCoreBuild)
                        GlobalFontSettings.FontResolver = new FailsafeFontResolver();

                    // Create a new MigraDoc document
                    var document = new Document();

                    // Define styles
                    DefineStyles(document);

                    // Add title page
                    AddTitlePage(document, invoices.Count);

                    // Add each invoice on separate pages
                    for (int i = 0; i < invoices.Count; i++)
                    {
                        var dto = invoices[i];

                        // Add a new section for each invoice
                        var section = document.AddSection();

                        // Set page margins
                        section.PageSetup.LeftMargin = Unit.FromCentimeter(2);
                        section.PageSetup.RightMargin = Unit.FromCentimeter(2);
                        section.PageSetup.TopMargin = Unit.FromCentimeter(2);
                        section.PageSetup.BottomMargin = Unit.FromCentimeter(2.5);

                        // Add invoice header with index
                        AddInvoiceHeaderWithIndex(section, dto, i + 1, invoices.Count);

                        // Add invoice details
                        AddInvoiceDetails(section, dto);

                        // Add line items table
                        AddLineItemsTable(section, dto);

                        // Add total
                        AddTotal(section, dto);

                        // Add footer
                        AddFooter(section);
                    }

                    // Create a renderer for the MigraDoc document
                    var pdfRenderer = new PdfDocumentRenderer
                    {
                        Document = document,
                        PdfDocument =
                {
                    PageLayout = PdfPageLayout.SinglePage,
                    ViewerPreferences =
                    {
                        FitWindow = true
                    }
                }
                    };

                    pdfRenderer.RenderDocument();
                    pdfRenderer.PdfDocument.Save(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public static byte[] CreatePdf(InvoiceDto dto)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    if (Capabilities.Build.IsCoreBuild)
                        GlobalFontSettings.FontResolver = new FailsafeFontResolver();

                    // Create a new MigraDoc document
                    var document = new Document();

                    // Define styles
                    DefineStyles(document);

                    // Add a section to the document
                    var section = document.AddSection();

                    // Set page margins
                    section.PageSetup.LeftMargin = Unit.FromCentimeter(2);
                    section.PageSetup.RightMargin = Unit.FromCentimeter(2);
                    section.PageSetup.TopMargin = Unit.FromCentimeter(2);
                    section.PageSetup.BottomMargin = Unit.FromCentimeter(2.5);

                    // Add header
                    AddHeader(section, dto);

                    // Add invoice details
                    AddInvoiceDetails(section, dto);

                    // Add line items table
                    AddLineItemsTable(section, dto);

                    // Add total
                    AddTotal(section, dto);

                    // Add footer
                    AddFooter(section);

                    // Create a renderer for the MigraDoc document
                    var pdfRenderer = new PdfDocumentRenderer
                    {
                        Document = document,
                        PdfDocument =
                    {
                        PageLayout = PdfPageLayout.SinglePage,
                        ViewerPreferences =
                        {
                            FitWindow = true
                        }
                    }
                    };

                    pdfRenderer.RenderDocument();
                    pdfRenderer.PdfDocument.Save(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private static void DefineStyles(Document document)
        {
            // Normal style
            var style = document.Styles[StyleNames.Normal];
            style.Font.Name = "Arial";
            style.Font.Size = 10;

            // Header style
            style = document.Styles.AddStyle("Header", StyleNames.Normal);
            style.Font.Size = 24;
            style.Font.Bold = true;
            style.Font.Color = Colors.DarkBlue;
            style.ParagraphFormat.SpaceBefore = 0;
            style.ParagraphFormat.SpaceAfter = 12;

            // Subheader style
            style = document.Styles.AddStyle("Subheader", StyleNames.Normal);
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.Font.Color = Colors.DarkBlue;
            style.ParagraphFormat.SpaceBefore = 12;
            style.ParagraphFormat.SpaceAfter = 6;

            // Label style
            style = document.Styles.AddStyle("Label", StyleNames.Normal);
            style.Font.Bold = true;
            style.Font.Size = 9;

            // Value style
            style = document.Styles.AddStyle("Value", StyleNames.Normal);
            style.Font.Size = 9;

            // Table header style
            style = document.Styles.AddStyle("TableHeader", StyleNames.Normal);
            style.Font.Bold = true;
            style.Font.Size = 9;
            style.Font.Color = Colors.White;

            // Table cell style
            style = document.Styles.AddStyle("TableCell", StyleNames.Normal);
            style.Font.Size = 9;

            // Total style
            style = document.Styles.AddStyle("Total", StyleNames.Normal);
            style.Font.Bold = true;
            style.Font.Size = 12;
            style.Font.Color = Colors.DarkBlue;
        }

        private static void AddTitlePage(Document document, int invoiceCount)
        {
            // Add title page section
            var titleSection = document.AddSection();

            // Set page margins
            titleSection.PageSetup.LeftMargin = Unit.FromCentimeter(2);
            titleSection.PageSetup.RightMargin = Unit.FromCentimeter(2);
            titleSection.PageSetup.TopMargin = Unit.FromCentimeter(4);
            titleSection.PageSetup.BottomMargin = Unit.FromCentimeter(2.5);

            // Add title
            var titleParagraph = titleSection.AddParagraph("INVOICE BATCH REPORT");
            titleParagraph.Style = "Header";
            titleParagraph.Format.Alignment = ParagraphAlignment.Center;
            titleParagraph.Format.SpaceAfter = 24;

            // Add summary information
            var summaryTable = titleSection.AddTable();
            summaryTable.AddColumn(Unit.FromCentimeter(6));
            summaryTable.AddColumn(Unit.FromCentimeter(6));

            var row = summaryTable.AddRow();

            // Left side - report info
            var cell = row.Cells[0];
            var para = cell.AddParagraph();
            para.AddFormattedText("Report Information", "Subheader");
            para.AddLineBreak();
            para.AddLineBreak();

            para.AddFormattedText("Total Invoices: ", "Label");
            para.AddText(invoiceCount.ToString());
            para.AddLineBreak();

            para.AddFormattedText("Generated On: ", "Label");
            para.Add(new DateField { Format = "yyyy-MM-dd HH:mm:ss" });
            para.AddLineBreak();

            // Right side - summary
            cell = row.Cells[1];
            para = cell.AddParagraph();
            para.AddFormattedText("Document Structure", "Subheader");
            para.AddLineBreak();
            para.AddLineBreak();

            para.AddText("• Title Page (this page)");
            para.AddLineBreak();
            para.AddText($"• {invoiceCount} Individual Invoice Pages");
            para.AddLineBreak();
            para.AddText("• Each invoice on separate page");

            // Add footer to title page
            AddFooter(titleSection);
        }

        private static void AddHeader(Section section, InvoiceDto dto)
        {
            // Invoice title
            var paragraph = section.AddParagraph("INVOICE");
            paragraph.Style = "Header";
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            // Add some space
            section.AddParagraph().Format.SpaceAfter = 12;
        }

        private static void AddInvoiceHeaderWithIndex(Section section, InvoiceDto dto, int currentIndex, int totalCount)
        {
            // Invoice title with index
            var paragraph = section.AddParagraph($"INVOICE ({currentIndex} of {totalCount})");
            paragraph.Style = "Header";
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            // Add some space
            section.AddParagraph().Format.SpaceAfter = 12;
        }

        private static void AddInvoiceDetails(Section section, InvoiceDto dto)
        {
            // Create a table for invoice details layout
            var table = section.AddTable();
            table.Format.SpaceAfter = 12;

            // Define columns (left side for invoice info, right side for billing info)
            var column = table.AddColumn(Unit.FromCentimeter(8));
            column = table.AddColumn(Unit.FromCentimeter(8));

            // First row - Invoice details and billing address
            var row = table.AddRow();

            // Left cell - Invoice details
            var cell = row.Cells[0];
            var para = cell.AddParagraph();
            para.AddFormattedText("Invoice Number: ", "Label");
            para.AddText(dto.InvoiceNumber ?? "N/A");
            para.AddLineBreak();

            para.AddFormattedText("Invoice Date: ", "Label");
            para.AddText(dto.InvoiceDate.ToString("yyyy-MM-dd"));
            para.AddLineBreak();

            // Right cell - Billing information
            cell = row.Cells[1];
            para = cell.AddParagraph();
            para.Format.Alignment = ParagraphAlignment.Right;

            para.AddFormattedText("Bill To:", "Subheader");
            para.AddLineBreak();
            para.AddFormattedText("Property: ", "Label");
            para.AddText(dto.RealEstateAka ?? "N/A");
            para.AddLineBreak();
            para.AddFormattedText("Address: ", "Label");
            para.AddText(dto.Address ?? "N/A");
            para.AddLineBreak();
            para.AddFormattedText("Owners: ", "Label");
            para.AddText(dto.Owners ?? "N/A");
        }

        private static void AddLineItemsTable(Section section, InvoiceDto dto)
        {
            // Add section title
            var paragraph = section.AddParagraph("Utility Charges");
            paragraph.Style = "Subheader";

            // Create the items table
            var table = section.AddTable();
            table.Style = "Table";
            table.Borders.Color = Colors.Black;
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;

            // Define columns
            var column = table.AddColumn(Unit.FromCentimeter(4));
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn(Unit.FromCentimeter(3));
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn(Unit.FromCentimeter(3));
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn(Unit.FromCentimeter(3));
            column.Format.Alignment = ParagraphAlignment.Right;

            // Create the header row
            var row = table.AddRow();
            row.HeightRule = RowHeightRule.AtLeast;
            row.Height = 18;
            row.Shading.Color = Colors.DarkBlue;
            row.Format.Font.Color = Colors.White;
            row.Format.Font.Bold = true;

            row.Cells[0].AddParagraph("Utility");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

            row.Cells[1].AddParagraph("From Date");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[2].AddParagraph("To Date");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Center;

            row.Cells[3].AddParagraph("Amount");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Right;

            // Add data rows
            if (dto.Poses != null && dto.Poses.Count > 0)
            {
                foreach (var item in dto.Poses)
                {
                    row = table.AddRow();
                    row.Height = 15;

                    row.Cells[0].AddParagraph(item.UtilityAka ?? "N/A");
                    row.Cells[0].Format.Alignment = ParagraphAlignment.Left;

                    row.Cells[1].AddParagraph(item.DateFrom.ToString("yyyy-MM-dd"));
                    row.Cells[1].Format.Alignment = ParagraphAlignment.Center;

                    row.Cells[2].AddParagraph(item.DateTo.ToString("yyyy-MM-dd"));
                    row.Cells[2].Format.Alignment = ParagraphAlignment.Center;

                    row.Cells[3].AddParagraph($"${item.Amount:F2}");
                    row.Cells[3].Format.Alignment = ParagraphAlignment.Right;

                    // Alternate row shading
                    if ((dto.Poses.IndexOf(item) + 1) % 2 == 0)
                    {
                        row.Shading.Color = Color.FromRgb(245, 245, 245);
                    }
                }
            }
            else
            {
                // Add a row indicating no items
                row = table.AddRow();
                row.Height = 15;

                var cell = row.Cells[0];
                cell.MergeRight = 3;
                cell.AddParagraph("No utility charges found");
                cell.Format.Alignment = ParagraphAlignment.Center;
                cell.Format.Font.Italic = true;
            }

            table.SetEdge(0, 0, table.Columns.Count, table.Rows.Count, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);
        }

        private static void AddTotal(Section section, InvoiceDto dto)
        {
            // Add some space before total
            section.AddParagraph().Format.SpaceAfter = 12;

            // Create a table for the total
            var table = section.AddTable();
            table.AddColumn(Unit.FromCentimeter(10)); // Empty space
            table.AddColumn(Unit.FromCentimeter(3));  // Label
            table.AddColumn(Unit.FromCentimeter(3));  // Amount

            var row = table.AddRow();

            // Empty cell
            row.Cells[0].AddParagraph();

            // Total label
            var cell = row.Cells[1];
            var para = cell.AddParagraph("TOTAL:");
            para.Style = "Total";
            para.Format.Alignment = ParagraphAlignment.Right;
            cell.Borders.Top.Width = 1;
            cell.Borders.Bottom.Width = 2;

            // Total amount
            cell = row.Cells[2];
            para = cell.AddParagraph($"${dto.Total:F2}");
            para.Style = "Total";
            para.Format.Alignment = ParagraphAlignment.Right;
            cell.Borders.Top.Width = 1;
            cell.Borders.Bottom.Width = 2;
        }

        private static void AddFooter(Section section)
        {
            // Create the primary footer
            var footer = section.Footers.Primary;

            var table = footer.AddTable();
            table.AddColumn(Unit.FromCentimeter(8));
            table.AddColumn(Unit.FromCentimeter(8));

            var row = table.AddRow();

            // Left side - generation date
            var para = row.Cells[0].AddParagraph();
            para.AddText("Generated on: ");
            para.Add(new DateField { Format = "yyyy-MM-dd HH:mm:ss" });
            para.Format.Font.Size = 8;
            para.Format.Alignment = ParagraphAlignment.Left;

            // Right side - page number
            para = row.Cells[1].AddParagraph();
            para.AddText("Page ");
            para.AddPageField();
            para.AddText(" of ");
            para.AddNumPagesField();
            para.Format.Font.Size = 8;
            para.Format.Alignment = ParagraphAlignment.Right;
        }
    }
}
