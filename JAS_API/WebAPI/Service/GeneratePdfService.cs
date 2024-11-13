﻿using Application.Interfaces;
using Application.Utils;
using DinkToPdf;
using DinkToPdf.Contracts;
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
        private readonly IConverter _converter;
        private readonly HelperValuation _helperValuation;
        public GeneratePdfService(IConverter converter, HelperValuation helperValuation)
        {
            _converter = converter;
            _helperValuation = helperValuation;
        }
        public byte[] CreateReceiptPDF(Valuation valuation, DateTime? recivedDate, string? productRecivedStatus)
        {
            string exportsPath = Path.Combine(Directory.GetCurrentDirectory(), "Exports");
            // Tạo thư mục "Exports" nếu chưa tồn tại
            if (!Directory.Exists(exportsPath))
            {
                Directory.CreateDirectory(exportsPath);
            }

            string fileName = $"BienBanXacNhanNhanHang_{valuation.Id}.pdf";
            string outputPath = Path.Combine(exportsPath, fileName);

            var glb = new GlobalSettings
            {
                DocumentTitle = fileName,
                Out = outputPath
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = _helperValuation.ToHtmlFileReceipt(valuation, recivedDate, productRecivedStatus),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = null }
            };

            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = glb,
                Objects = { objectSettings }
            };

            _converter.Convert(pdf);

            // Đọc tệp PDF đã tạo thành mảng byte
            byte[] pdfBytes = File.ReadAllBytes(outputPath);

            // Xóa tệp tạm sau khi đọc nếu cần
            File.Delete(outputPath);

            return pdfBytes;
        }


        public byte[] CreateAuthorizedPDF(Valuation valuation)
        {
            string exportsPath = Path.Combine(Directory.GetCurrentDirectory(), "Exports");
            // Tạo thư mục "Exports" nếu chưa tồn tại
            if (!Directory.Exists(exportsPath))
            {
                Directory.CreateDirectory(exportsPath);
            }

            string fileName = $"BienBanUyQuyen_{valuation.Id}.pdf";
            string outputPath = Path.Combine(exportsPath, fileName);

            var glb = new GlobalSettings
            {
                DocumentTitle = fileName,
                Out = outputPath
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = _helperValuation.ToHtmlFileAuthorized(valuation),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = null }
            };

            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = glb,
                Objects = { objectSettings }
            };

            _converter.Convert(pdf);

            // Đọc tệp PDF đã tạo thành mảng byte
            byte[] pdfBytes = File.ReadAllBytes(outputPath);

            // Xóa tệp tạm sau khi đọc nếu cần
            File.Delete(outputPath);

            return pdfBytes;
        }

    }
}