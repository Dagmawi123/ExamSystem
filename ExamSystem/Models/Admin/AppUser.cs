
using Microsoft.AspNetCore.Identity;
namespace ExamSystem.Models.Admin
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
