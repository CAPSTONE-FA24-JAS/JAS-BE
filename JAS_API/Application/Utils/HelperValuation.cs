using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public class HelperValuation
    {
        public static string ToHtmlFileReceipt(Valuation valuation, DateTime? recivedDate, string? productRecivedStatus)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(),  "HtmlFiles", "Receipt.html");
            string templateHtml = File.ReadAllText(templatePath);
            

            templateHtml = templateHtml.Replace("{{Seller_name}}", valuation.Seller.FirstName + valuation.Seller.LastName)
                                       .Replace("{{DiaChi_seller}}", valuation.Seller.Address)
                                       .Replace("{{CCCD}}", valuation.Seller.CitizenIdentificationCard)
                                       .Replace("{{Time_Create}}", valuation.Seller.IDIssuanceDate.ToString())
                                       .Replace("{{Time_expire}}", valuation.Seller.IDExpirationDate.ToString())
                                       .Replace("{{Phone_seller}}", valuation.Seller.Account.PhoneNumber)
                                       .Replace("{{JewelryName}}", valuation.Name)
                                       .Replace("{{ReceivingDate}}", recivedDate.ToString())
                                       .Replace("{{ProductRecivedStatus}}", productRecivedStatus);






            return templateHtml;
        }


        public static string ToHtmlFileAuthorized(Valuation valuation)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "HtmlFiles", "UyQuyen.html");
            string templateHtml = File.ReadAllText(templatePath);


            templateHtml = templateHtml.Replace("{{Seller_name}}", valuation.Seller.FirstName + valuation.Seller.LastName)
                                       .Replace("{{DiaChi_seller}}", valuation.Seller.Address)
                                       .Replace("{{CCCD}}", valuation.Seller.CitizenIdentificationCard)
                                       .Replace("{{Time_Create}}", valuation.Seller.IDIssuanceDate.ToString())
                                       .Replace("{{Time_expire}}", valuation.Seller.IDExpirationDate.ToString());
            return templateHtml;
        }
    }
}
