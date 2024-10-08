using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Artist : BaseEntity
    {
        public string? Name { get; set; }
        //
        public virtual IEnumerable<Jewelry>? Jewelries { get; set; } 
        public virtual IEnumerable<FollwerArtist>? FollwerArtists { get; set; } 
    }
}
