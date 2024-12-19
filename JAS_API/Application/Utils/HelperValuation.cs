using Domain.Entity;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public class HelperValuation
    {
        private readonly IWebHostEnvironment _env;

        public HelperValuation(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string ToHtmlFileReceipt(Valuation valuation, string? jewelryName, DateTime? recivedDate, string? productRecivedStatus, string? note, string? khoiluong)
        {
            // Sử dụng _env.WebRootPath để lấy đường dẫn đến thư mục wwwroot
            string templatePath = Path.Combine(_env.WebRootPath, "HtmlFiles", "Receipt.html");
            string templateHtml = File.ReadAllText(templatePath);

            templateHtml = templateHtml.Replace("{{Seller_name}}", valuation.Seller.FirstName + " " + valuation.Seller.LastName ?? "")
                                       .Replace("{{DiaChi_seller}}", valuation.Seller.Address ?? "")
                                       .Replace("{{CCCD}}", valuation.Seller.CitizenIdentificationCard ?? "")
                                       .Replace("{{Time_Create}}", valuation.Seller.IDIssuanceDate.ToString() ?? "")
                                       .Replace("{{Time_expire}}", valuation.Seller.IDExpirationDate.ToString() ?? "")
                                       .Replace("{{Phone_seller}}", valuation.Seller.Account.PhoneNumber ?? "")
                                       .Replace("{{JewelryName}}", jewelryName ?? "")
                                       .Replace("{{ReceivingDate}}", recivedDate?.ToString() ?? "")
                                       .Replace("{{ProductRecivedStatus}}", productRecivedStatus ?? "")
                                       .Replace("{{Note}}", note ?? "")
                                       .Replace("{{StaffName}}", valuation.Staff.FirstName + " " + valuation.Staff.LastName ?? "")
                                       .Replace("{{role}}", valuation.Staff.Account.Role.Name ?? "")
                                       .Replace("{{KhoiLuong}}", khoiluong ?? "")
                                       .Replace("{{emailStaff}}", valuation.Staff.Account.Email ?? "");               
            return templateHtml;
        }

        public string ToHtmlFileAuthorized(Valuation valuation)
        {
            string templatePath = Path.Combine(_env.WebRootPath, "HtmlFiles", "UyQuyen.html");
            string templateHtml = File.ReadAllText(templatePath);

            templateHtml = templateHtml.Replace("{{Seller_name}}", valuation.Seller.FirstName + " " + valuation.Seller.LastName)
                                       .Replace("{{DiaChi_seller}}", valuation.Seller.Address ?? "")
                                       .Replace("{{CCCD}}", valuation.Seller.CitizenIdentificationCard ?? "")
                                       .Replace("{{Time_Create}}", valuation.Seller.IDIssuanceDate.ToString() ?? "")
                                       .Replace("{{Time_expire}}", valuation.Seller.IDExpirationDate.ToString() ?? "")
                                       .Replace("{{Specific_price}}", valuation.Jewelry.SpecificPrice.ToString() ?? "");

            return templateHtml;
        }
    }
}
