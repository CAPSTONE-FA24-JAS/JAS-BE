using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class KeyCharacteristicDetail : BaseEntity
    {
        public string? Description { get; set; }
        public int? JewelryId { get; set; }
        public int? KeyCharacteristicId { get; set; }
        //
        public virtual Jewelry? Jewelry { get; set; }
        public virtual KeyCharacteristic? KeyCharacteristic { get; set; }

    }
}
