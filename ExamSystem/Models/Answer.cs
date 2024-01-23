using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Models
{
    public class Answer
    {
        [Key]
        public Guid AnswerId { get; set; }
        [Required]
        public string AnswerText { get; set; }
        //[Required]
        //public Guid QuestionId { get; set; }
        public Question Question { get; set; }  
        [Required]
        public bool isCorrect { get; set; }
    }
}
