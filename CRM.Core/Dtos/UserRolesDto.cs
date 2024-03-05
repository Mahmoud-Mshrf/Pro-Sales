using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRM.Core.Dtos
{
    public class UserRolesDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public IEnumerable<RoleModel> Roles { get; set; }
    }
}
