﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Notification : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? Is_Read { get; set; }
        public int? NotifiableId { get; set; }
        public string? Notifi_Type { get; set; }
        public int? AccountId { get; set; }
        public string? ImageLink { get; set; }
        public string? StatusOfValuation { get; set; }

        //
        public virtual Account? Account { get; set; }
    }
}
