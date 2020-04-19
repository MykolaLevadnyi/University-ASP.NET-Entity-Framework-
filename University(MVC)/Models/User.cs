using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace University_MVC_.Models
{
    public class User : IdentityUser
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        

    }
}
