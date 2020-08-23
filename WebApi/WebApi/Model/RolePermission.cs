using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Model
{
    public class RolePermission:BaseEntity
    {
        public int Rid { get; set; }
        public int Pid { get; set; }

        [ForeignKey("Rid")]
        public virtual Role Roles { get; set; }
        [ForeignKey("Pid")]
        public virtual Permission Permissions { get; set; }
    }
}
