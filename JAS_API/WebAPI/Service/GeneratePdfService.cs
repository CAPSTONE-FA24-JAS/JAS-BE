using Application.Interfaces;
using Application.Utils;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class GeneratePdfService : IGeneratePDFService
    {
        
        private readonly HelperValuation _helperValuation;
        public GeneratePdfService( HelperValuation helperValuation, IConfiguration configuration)
        {
            License.LicenseKey = configuration["IronPdf:LicenseKey"];
            _helperValuation = helperValuation;
        }
        public byte[] CreateReceiptPDF(Valuation valuation, string? jewelryName, DateTime? recivedDate, string? productRecivedStatus, string? note, string? khoiluong)
        {
            // Tạo nội dung HTML từ HelperValuation
            string htmlContent = _helperValuation.ToHtmlFileReceipt(valuation, jewelryName, recivedDate, productRecivedStatus, note, khoiluong);

            // Khởi tạo trình render PDF của IronPdf
            var renderer = new ChromePdfRenderer();

            // Cấu hình để đảm bảo hỗ trợ đầy đủ
            renderer.RenderingOptions.MarginTop = 10; // margin trên
            renderer.RenderingOptions.MarginBottom = 10; // margin dưới
            renderer.RenderingOptions.MarginLeft = 10; // margin trái
            renderer.RenderingOptions.MarginRight = 10; // margin phải
            renderer.RenderingOptions.PrintHtmlBackgrounds = true;

            // Render nội dung HTML thành PDF
            var pdf = renderer.RenderHtmlAsPdf(htmlContent);

            // Trả về mảng byte PDF
            return pdf.BinaryData;
        }


        public byte[] CreateAuthorizedPDF(Valuation valuation)
        {
            // Tạo nội dung HTML từ HelperValuation
            string htmlContent = _helperValuation.ToHtmlFileAuthorized(valuation);

            // Khởi tạo trình render PDF của IronPdf
            var renderer = new ChromePdfRenderer();

            // Cấu hình để đảm bảo hỗ trợ đầy đủ
            renderer.RenderingOptions.MarginTop = 10; // margin trên
            renderer.RenderingOptions.MarginBottom = 10; // margin dưới
            renderer.RenderingOptions.MarginLeft = 10; // margin trái
            renderer.RenderingOptions.MarginRight = 10; // margin phải
            renderer.RenderingOptions.PrintHtmlBackgrounds = true;

            // Render nội dung HTML thành PDF
            var pdf = renderer.RenderHtmlAsPdf(htmlContent);

            // Trả về mảng byte PDF
            return pdf.BinaryData;
        }

    }
}
