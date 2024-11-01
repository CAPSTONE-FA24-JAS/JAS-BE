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
        public static string ToHtmlFile(Valuation valuation)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "HtmlFiles", "Receipt");
            string templateHtml = File.ReadAllText(templatePath);
            StringBuilder stringData = new StringBuilder(String.Empty);
            return stringData.ToString();
        }
    }
}
