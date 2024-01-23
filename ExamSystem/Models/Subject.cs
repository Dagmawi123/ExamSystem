using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Models
{
    public class Subject
    {
        [Key]
    public Guid SubjectId { get; set; }
public string SubjectName { get; set; }
    public List<Exam> Exams { get; set; }
        public List<Document> Documents { get; set; }
    }

}
