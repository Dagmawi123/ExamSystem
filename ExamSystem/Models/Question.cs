using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Models
{
    public class Question
    {
        [Key]
        public Guid QuesionId { get; set; }
        //public Guid ExamId { get; set; }
       public string Query { get; set; }
        public Exam Exam { get; set; }
        public List<Answer> Answers { get; set; }
        
    }
}
