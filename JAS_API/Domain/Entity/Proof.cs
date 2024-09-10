using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Proof : BaseEntity
    {
        public string? Name { get; set; }
        public int? ProofTypeId { get; set; }
        public int? JewelryId { get; set; }
        //
        public virtual ProofType? ProofType { get; set; }
        public virtual Jewelry? Jewelry { get; set; }
    }
}
