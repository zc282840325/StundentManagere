using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Model
{
    public class UserRole: BaseEntity
    {
        public int Uid { get; set; }
        public int Rid { get; set; }
        [ForeignKey("Uid")]
        public virtual User Users { get; set; }
        [ForeignKey("Rid")]
        public virtual Role Roles { get; set; }
    }
}
