using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public List<Result> Results { get; set; } 
        

        public UserType UserType { get; set; }
    }

}
