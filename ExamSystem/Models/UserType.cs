using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Models
{
    public class UserType
    {
        [Key]
        public Guid UserTypeId { get; set; }
        public string User_Type { get; set; }
        public List<User> users { get; set;}
    }
}
