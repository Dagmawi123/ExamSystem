namespace ExamSystem.Models
{
    public class ResultDetail
    {
        public ResultDetail() { }
        public string ExamName { get; set; }
        public string ExamineeName { get; set; }   
        public float ExamScore { get; set; }
        public int ScoreToPass { get; set; } 
        public string CorrectAnswers { get; set; }
        public String DateTaken { get; set; }
        public bool HasPassed { get; set; }

    }
}
