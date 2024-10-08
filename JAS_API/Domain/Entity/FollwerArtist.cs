using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class FollwerArtist : BaseEntity
    {
        public int? CustomerId { get; set; }
        public int? ArtistId { get; set; }

        //
        public virtual Artist? Artist { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}
