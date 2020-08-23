using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dto
{
    public class BaseEntityDto
    {
        public int Id { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsDelete { get; set; } = false;
    }
}
