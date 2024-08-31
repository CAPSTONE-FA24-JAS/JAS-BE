using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceReponse
{
    public class APIResponseModel
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public object? Data { get; set; }
    }
}
