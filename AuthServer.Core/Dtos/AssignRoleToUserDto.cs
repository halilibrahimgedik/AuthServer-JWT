using AuthServer.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Dtos
{
    public class AssignRoleToUserDto
    {
        public string UserId { get; set; }

        public string RoleName { get; set; }
    }
}
