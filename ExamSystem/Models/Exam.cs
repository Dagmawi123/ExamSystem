using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Models
{
    public class Exam
    {
        [Key]
        public Guid ExamId { get; set; }
        public string ExamName { get; set; }
        public Subject Subject { get; set; }
        //public string ExamImagePath { get; set; }
        public string ExamDifficulty { get; set; }
        public DateTime DateCreated { get; set; }
        public string TimeAllocated { get; set; }
        public int QuestionWeight { get; set; } 
        public int PassingMark {  get; set; } 
        public List<Question> Questions { get; set; }
        
    } 
}
