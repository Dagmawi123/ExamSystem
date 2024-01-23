namespace ExamSystem.Models.ViewModels
{
    public class ExamTakeViewModel
    {
        public ExamTakeViewModel() { }
        public Exam Exam { get; set; }
        public User User { get; set; }
        public List<Question> Questions { get; set; }
    }
}
