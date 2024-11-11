using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EnumStatusLot
    {
        Waiting = 1, 
        Ready = 2,
        Auctioning = 3,   
        Sold = 4 ,       
        Canceled = 5,    
        Passed = 6,
        Pause = 7
    }
}
