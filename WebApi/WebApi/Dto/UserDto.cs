using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Dto
{
    public class UserDto: BaseEntityDto
    {
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public string Address { get; set; }
    }
}
