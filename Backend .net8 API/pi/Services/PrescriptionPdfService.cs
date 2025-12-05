using System.IO;
using Fylde.Prescriptions.Api.Models;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;

namespace Fylde.Prescriptions.Api.Services;

public class PrescriptionPdfService
{
    public byte[] GeneratePrescriptionPdf(Prescription prescription)
    {
        using var ms = new MemoryStream();

        var writer = new PdfWriter(ms);
        var pdfDoc = new PdfDocument(writer);
        var doc = new Document(pdfDoc);

        // Clinic header
        doc.Add(new Paragraph(prescription.ClinicName)
            .SetFontSize(18)
            .SetBold());

        doc.Add(new Paragraph(prescription.ClinicAddress)
            .SetFontSize(10));

        doc.Add(new Paragraph($"Date: {prescription.DateIssued:yyyy-MM-dd}")
            .SetMarginTop(10));

        doc.Add(new Paragraph($"Patient: {prescription.PatientName}")
            .SetMarginTop(5)
            .SetFontSize(12)
            .SetBold());

        // Prescription table
        doc.Add(new Paragraph("Prescription")
            .SetFontSize(14)
            .SetBold()
            .SetMarginTop(15));

        var table = new Table(new float[] { 3, 2, 4, 1 }).UseAllAvailableWidth();
        table.AddHeaderCell("Drug");
        table.AddHeaderCell("Strength");
        table.AddHeaderCell("Directions");
        table.AddHeaderCell("Qty");

        foreach (var item in prescription.Items)
        {
            table.AddCell(item.DrugName);
            table.AddCell(item.Strength);
            table.AddCell(item.Directions);
            table.AddCell(item.Quantity.ToString());
        }

        doc.Add(table);

        doc.Add(new Paragraph($"Prescriber: {prescription.PrescriberName}")
            .SetMarginTop(25));

        doc.Add(new Paragraph("Digitally signed by the prescriber. Any modification after signing will invalidate this document.")
            .SetFontSize(9)
            .SetItalic()
            .SetFontColor(ColorConstants.GRAY)
            .SetMarginTop(10));

        doc.Close();
        return ms.ToArray();
    }
}
