using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace NetHome.Data.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfRegistration { get; set; }

        public ICollection<Device> Devices { get; set; }
    }
}
