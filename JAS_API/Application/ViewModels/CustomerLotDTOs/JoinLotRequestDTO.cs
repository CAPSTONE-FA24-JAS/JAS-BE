using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.CustomerLotDTOs
{
    public class JoinLotRequestDTO
    {
        public int AccountId { get; set; }
        public int LotId { get; set; }

        public string ConnectionId { get; set; }
    }
}
