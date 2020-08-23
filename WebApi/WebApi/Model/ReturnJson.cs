using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Model
{
    public class ReturnJson
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public DateTime Data { get; set; }
    }
}
