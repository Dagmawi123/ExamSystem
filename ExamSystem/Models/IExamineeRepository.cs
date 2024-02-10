using ExamSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExamSystem.Models
{

    public interface IExamineeRepository
    {
        public DocumentViewModel GetDocuments();
        public Exam GetExam(Guid id);
        public List<Result> GetResults(User usr);
        public List<Exam> GetExams();
        public Exam GetExamDetail(Guid id);
        public void addScore(Result res);
        public List<string> GetExamNames();
        public List<string> GetSubjectNames();
        public List<Result> GetFilteredResults(string exam, string subject, string score, string date);
        public ResultDetail GetResultDetail(Guid resId);
        public List<Exam> FilterExams(string name);
        public List<Document> FilterDocs(string DocName, string Subject, string version);
        //public float ComputeScore(Guid id);
    }
}
