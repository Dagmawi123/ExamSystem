using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Models
{
    public class Result
    {
        [Key]
        public Guid ResultId { get; set; }
        //public Guid UserId { get; set; }
        public User User { get; set; }  
        public int RowScore { get; set; }
        public float outOf100 { get; set; }
        public Exam? Exam { get; set; }
        public DateTime DateTaken { get; set; }
    }
}
