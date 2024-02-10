using ExamSystem.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Web.WebPages;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExamSystem.Models
{
    //[Route("api/[controller]")]
    //[ApiController]

    public class ExamineeRepository : IExamineeRepository
    {
        private readonly UserManager<User> usrMgr;
        private readonly ExamContext examContext;

        public ExamineeRepository(ExamContext examContext, UserManager<User> usrMgr)
        {
            this.examContext = examContext;
            this.usrMgr = usrMgr;
            //this.webHostEnvironment = webHostEnvironment;
            //this.productRepository = productRepository;


        }
        public DocumentViewModel GetDocuments()
        {
            DocumentViewModel documentViewModel = new DocumentViewModel();
            documentViewModel.Documents = examContext.Documents.Include(p => p.Subject).ToList();
            documentViewModel.Subjects = examContext.Subjects.ToList();
            return documentViewModel;
        }

        public Exam GetExam(Guid id) {
            Exam? evm = new Exam();
            //evm = examContext.Exams.Include(e => e.Questions.Select(q => q.Answers)).Where(e => e.ExamId == id).FirstOrDefault();
            evm = examContext.Exams.Include(e => e.Questions).ThenInclude(q => q.Answers).Where(e => e.ExamId == id).FirstOrDefault();
            //evm.Questions=examContext.Questions.Include(e=>e.Answers).Where(e=>e.)
            return evm;
        }
        public List<Result> GetResults(User usr)
        { //string id = "85FE0EE1-22CC-42D4-8816-9DE99438188C";
            //User user = await usrMgr.GetUserAsync();
            List<Result> rs = examContext.Results.Include(e => e.User).Include(e => e.Exam).Where(r => r.User==usr).ToList();
            //var userResults = examContext.Results.Include(e=>e.User).Where(r => r.User.UserId == id).Include(e=>e.Exam).ToList();
            Console.WriteLine(rs.Count);
            return rs;
        }


        public List<Exam> GetExams() 
        {
            List<Exam> exams= examContext.Exams.Include(e => e.Subject).ToList();
            return exams;
        }

        public Exam GetExamDetail(Guid id) {
            Exam exam = examContext.Exams.Where(e => e.ExamId == id).First();
            exam.Subject = new Subject()
            {
                SubjectName = examContext.Exams.Where(e => e.ExamId == id).Select(e => e.Subject.SubjectName).First()
            };
            return exam;

        }
        public List<string> GetExamNames() {
            List<string> names = examContext.Exams.Select(e => e.ExamName).ToList();
            return names;
        }
        public List<string> GetSubjectNames() {
            List<string> names = examContext.Subjects.Select(e => e.SubjectName).ToList();
            return names;
        }

        public List<Result> GetFilteredResults(string exam,string subject,string score,string date) {
            int realScore = score == "75" ? 75 : score == "50" ? 50 : 0;
            DateTime dates = DateTime.Now;
            if (!string.IsNullOrEmpty(date))
                 dates = DateTime.Parse(date);
            string id = "85FE0EE1-22CC-42D4-8816-9DE99438188C";
            List<Result> res = examContext.Results
    .Include(e => e.User)
    .Include(e => e.Exam)
        .ThenInclude(e => e.Subject)
    .Where(r => ( r.User.Id == id &&exam==null||r.Exam.ExamName == exam) && 
    (subject==null||r.Exam.Subject.SubjectName == subject)&&
    ((date==null||r.DateTaken.Date == dates.Date))&&
    (score==null||r.RowScore>=realScore&&r.RowScore<=realScore+(realScore==0?50:25)))
    .ToList();
           
            return res;
        }
        public void addScore(Result res) {
             examContext.Results.AddAsync(res);
            examContext.SaveChanges();

        }

        public ResultDetail GetResultDetail(Guid resId)
        {
            
                Result res =
                        examContext.Results.Where(r => r.ResultId == resId).Include(r=>r.User).Include(r=>r.Exam).ThenInclude(r=>r.Questions).First();
                ResultDetail rd = new ResultDetail() {
                ExamName = res.Exam.ExamName,
                ExamineeName = res.User.FirstName +" "+ res.User.LastName,
                ExamScore = res.outOf100,
                ScoreToPass = res.Exam.PassingMark,
                CorrectAnswers=res.RowScore+"/"+ res.Exam.Questions.Count,
                DateTaken=res.DateTaken.ToString("MMM dd ,yyyy"),
                HasPassed= res.outOf100>=res.Exam.PassingMark
            };
            return rd;
            //return res;
        }
        public List<Exam> FilterExams(string name) { 
        List<Exam> exams = examContext.Exams.Where(e=>e.ExamName.Contains(name)).ToList();
            return exams;
        }

        public List<Document> FilterDocs(string DocName, string Subject, string version)
        {
            List<Document> docs = examContext.Documents.Include(e => e.Subject)
                .Where(e => (DocName == null || e.DocName.Contains(DocName)) &&
                            (Subject == null || e.Subject.SubjectName == Subject) &&
                            (version == null || e.DocVersion.ToString() == version))
                .ToList();

                return docs;
        }


        //public float ComputeScore(Guid id) {
        //    //Result result = examContext.Results.Include(e=>e.Exam).Where(r => r.ResultId == id).First();
        //    //int rawScore = result.Score;
        //    //int numberOfQuestions = examContext.Questions.Include(q=>q.Exam).Where(q => q.Exam.ExamId == result.Exam.ExamId).Count();
        //    //float score = (float)rawScore / (float)numberOfQuestions;
        //    //score *= 100;
        //    //return score;
        //}














        //// GET: api/<UserRepository>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<UserRepository>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<UserRepository>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<UserRepository>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<UserRepository>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
