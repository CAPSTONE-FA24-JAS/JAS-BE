using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Utils
{
    public static class EnumHelper
    {
        public class EnumItem
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
        public static List<EnumItem> GetEnums<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .Select(e => new EnumItem { Name = e.ToString(), Value = Convert.ToInt32(e)})
                       .ToList();
        }
    }
}
