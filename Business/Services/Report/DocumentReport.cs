using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Report
{
    public class DocumentReport : IDocumentReport
    {
        public DocumentReport()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]?> Report(DataAccess.Entities.Document document)
        {
            var documentPdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.ContentFromLeftToRight();
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    // Simple color palette that works with alpha version
                    var primaryColor = QuestPDF.Infrastructure.Color.FromHex("#014A99");
                    var secondaryColor = QuestPDF.Infrastructure.Color.FromHex("#6C757D");
                    var borderColor = QuestPDF.Infrastructure.Color.FromHex("#DEE2E6");

                    page.Header().ShowOnce().Column(headerCol =>
                    {
                        headerCol.Item().PaddingBottom(10).Row(row =>
                        {
                            row.RelativeColumn().Column(col =>
                            {
                                col.Item()
                                    .Text(document.DocumentName)
                                    .AlignStart()
                                    .Bold()
                                    .FontSize(18)
                                    .FontColor(primaryColor);

                                col.Item()
                                    .PaddingTop(5)
                                    .Text($"Date: {DateTime.Now:MMMM dd, yyyy}")
                                    .AlignStart()
                                    .FontSize(10)
                                    .FontColor(secondaryColor);
                            });

                            row.RelativeColumn().Column(col =>
                            {
                                col.Item()
                                    .Text($"Ref: {document.RefNumber}")
                                    .AlignEnd()
                                    .Bold()
                                    .FontSize(11)
                                    .FontColor(secondaryColor);

                                // Simplified QR code container - removed BorderRadius and complex styling
                                col.Item()
                                    .PaddingTop(10)
                                    .AlignRight()
                                    .Width(80)
                                    .Height(80)
                                    .Svg(document.QR)
                                    .FitArea();
                            });
                        });

                        // Simple separator line
                        headerCol.Item().PaddingTop(10).BorderBottom(1);
                    });

                    page.Content().Column(contentCol =>
                    {
                        // Student Information Section
                        contentCol.Item().PaddingTop(15).Column(section =>
                        {
                            // Section header
                            section.Item().Background(QuestPDF.Infrastructure.Color.FromHex("#E7F1FF")).Padding(5).Row(row =>
                            {
                                row.RelativeItem().Text("STUDENT INFORMATION").Bold().FontSize(11).FontColor(primaryColor);
                            });

                            // Student row
                            section.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item()
                                        .Text("Student Name")
                                        .AlignStart()
                                        .FontSize(11)
                                        .FontColor(secondaryColor);
                                });
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item()
                                        .Text(document.Student?.Name ?? "N/A")
                                        .AlignEnd()
                                        .Bold()
                                        .FontSize(12)
                                        .FontColor(primaryColor);
                                });
                            });
                            section.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item()
                                        .Text("Student Major")
                                        .AlignStart()
                                        .FontSize(11)
                                        .FontColor(secondaryColor);
                                });
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item()
                                        .Text(document.Student?.Major ?? "N/A")
                                        .AlignEnd()
                                        .Bold()
                                        .FontSize(12)
                                        .FontColor(primaryColor);
                                });
                            });
                        });

                        // Document Properties Section
                        if (document.DocumentProperties != null && document.DocumentProperties.Any())
                        {
                            contentCol.Item().PaddingTop(15).Column(section =>
                            {
                                // Section header
                                section.Item().Background(QuestPDF.Infrastructure.Color.FromHex("#E7F1FF")).Padding(5).Row(row =>
                                {
                                    row.RelativeItem().Text("DOCUMENT DETAILS").Bold().FontSize(11).FontColor(primaryColor);
                                });

                                int counter = 0;
                                foreach (var prop in document.DocumentProperties)
                                {
                                    // Simple alternating colors without complex background
                                    section.Item().PaddingTop(8).Row(row =>
                                    {
                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item()
                                                .Text(prop.PropertyName)
                                                .AlignStart()
                                                .FontSize(11)
                                                .FontColor(secondaryColor);
                                        });
                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item()
                                                .Text(prop.PropertyValue ?? "N/A")
                                                .AlignEnd()
                                                .FontSize(12)
                                                .FontColor(primaryColor);
                                        });
                                    });

                                    // Light separator between rows
                                    if (counter < document.DocumentProperties.Count - 1)
                                    {
                                        section.Item().PaddingTop(5).BorderBottom(0.5f);
                                    }
                                    counter++;
                                }
                            });
                        }

                      
                    });

                    // Simple page footer
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ").FontSize(8).FontColor(secondaryColor);
                        text.CurrentPageNumber().FontSize(8).FontColor(primaryColor);
                        text.Span(" of ").FontSize(8).FontColor(secondaryColor);
                        text.TotalPages().FontSize(8).FontColor(primaryColor);
                    });
                });
            });

           // documentPdf.ShowInCompanion();
            var pdfBytes = documentPdf.GeneratePdf();
            return pdfBytes;
        }
    }
}