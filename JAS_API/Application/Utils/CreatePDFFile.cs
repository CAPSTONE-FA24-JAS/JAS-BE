using Application.ViewModels.ValuationDTOs;
using Domain.Entity;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public class CreatePDFFile
    {
        public static byte[] CreatePDF(Valuation valuation, Account seller)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter pdfWriter = PdfWriter.GetInstance(document, stream);

                // Open document to start writing content
                document.Open();

                // Add header
                Font headerFont = FontFactory.GetFont("Times New Roman", 14, Font.BOLD);
                Paragraph header = new Paragraph("Cộng hòa xã hội chủ nghĩa Việt Nam\nĐộc lập - Tự do - Hạnh phúc", headerFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(header);

                // Add title
                Font titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
                Paragraph title = new Paragraph("BIÊN BAN GIAO NHAN", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                // Add location and date
                Font contentFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
                document.Add(new Paragraph($"Tp Hồ Chí Minh, ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}", contentFont));
                document.Add(new Paragraph(" ", contentFont)); // Add a blank line

                // Section 1: Bên A
                document.Add(new Paragraph("1. Bên A:", contentFont));
                document.Add(new Paragraph("Công ty: JAS company", contentFont));
                document.Add(new Paragraph("Địa chỉ: S10.05, đường Nguyễn Xiển, phường Long Bình, tp Thủ Đức, Hồ Chí Minh", contentFont));
                document.Add(new Paragraph("SĐT: 0961545926", contentFont));
                document.Add(new Paragraph(" ", contentFont)); // Add a blank line

                // Section 2: Bên B
                document.Add(new Paragraph("2. Bên B:", contentFont));
                document.Add(new Paragraph($"Họ tên: {seller.LastName + seller.FirstName}", contentFont));
                document.Add(new Paragraph($"Địa chỉ: {seller.Address}", contentFont));
                document.Add(new Paragraph($"Số CMND: {seller.CitizenIdentificationCard} cấp ngày: {seller.IDIssuanceDate}   Ngày hết hạn: {seller.IDExpirationDate}", contentFont));
                document.Add(new Paragraph($"Email: {seller.Email}", contentFont));
                document.Add(new Paragraph(" ", contentFont)); // Add a blank line

                // Section 3: Nội dung
                document.Add(new Paragraph("3. Nội dung:", contentFont));
                document.Add(new Paragraph($"Công ty chúng tôi đã nhận được hàng của quý khách hàng (Bên B) như sau:", contentFont));
                document.Add(new Paragraph($"- \"{valuation.Name}\"  với trạng thái: \"{valuation.ActualStatusOfJewelry}\"  vào ngày \"{valuation.DeliveryDate:dd/MM/yyyy}\"", contentFont));

                // Add final confirmation
                document.Add(new Paragraph(" ", contentFont)); // Add a blank line

                // Close the document
                document.Close();

                // Return the PDF file's byte array
                return stream.ToArray();
            }
        }
    }
}
