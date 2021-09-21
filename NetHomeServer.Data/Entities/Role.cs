using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHomeServer.Data.Entities
{
    public class Role : IdentityRole
    {
        public int AccessLevel { get; set; }
    }
}
