using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class KeyCharacteristic : BaseEntity
    {
        public string? Name { get; set; }

        //
        public virtual IEnumerable<KeyCharacteristicDetail> KeyCharacteristicDetails { get; set; }
    }
}
