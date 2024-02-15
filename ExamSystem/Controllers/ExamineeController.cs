using ExamSystem.Models;
using ExamSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ExamSystem.Controllers
{
    [Authorize(Roles ="User")]
    public class ExamineeController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        //private readonly ProductRepository productRepository;
        private readonly ExamContext examContext;
        private readonly IExamineeRepository userRepository;
        private readonly UserManager<User> _userManager;

        public ExamineeController(ExamContext examContext, IWebHostEnvironment webHostEnvironment,IExamineeRepository userRepo, UserManager<User> _userManager)
        {
            this.examContext = examContext;
            this.webHostEnvironment = webHostEnvironment;
            //this.productRepository = productRepository;
            userRepository = userRepo;
            this._userManager = _userManager;

        }
        [HttpGet]
        [Route("/Examinee/")]
        public IActionResult User_Home()
        {
            //UserRepository ur = new UserRepository(examContext);
            List<Exam> exams = userRepository.GetExams();

            return View(exams);
        }
        [HttpPost]
        public IActionResult User_Home(string query) { 
        List<Exam> exams=userRepository.FilterExams(query);
            if (exams == null)
            {
                return View();
            }
            else {
                return PartialView("_FilteredDataPartialView",exams);
            }

        }


        [HttpGet]
        public IActionResult Documents()
        {
            //UserRepository ur = new UserRepository(examContext);
            DocumentViewModel dvm = userRepository.GetDocuments();
            // Console.WriteLine(docs[0].Subject.SubjectName);
            return View(dvm);
        }

        [HttpPost]
        public IActionResult Documents(string DocName,string Subject,string Version) { 
            List<Document> docs=userRepository.FilterDocs(DocName, Subject, Version);
            DocumentViewModel dvm = userRepository.GetDocuments();
            dvm.Documents = docs;
            return View(dvm);
        }

        [HttpGet]
        public IActionResult ExamPage(Guid id)
        {
            //Guid id = new Guid("3B462801-95A3-4818-B6DE-DB08080EE9DE");
            //UserRepository ur = new UserRepository(examContext);
            Exam ex = userRepository.GetExam(id);
            //Console.WriteLine("Exam is " + ex.ExamName);
            return View(ex);
        }
        [HttpGet]
        public async Task<IActionResult> Results() {
            //UserRepository ur = new UserRepository(examContext);
            User usr=await _userManager.GetUserAsync(User);
            List<Result> results =userRepository.GetResults(usr);
            ViewBag.ExamNames = userRepository.GetExamNames();
            ViewBag.SubjectNames = userRepository.GetSubjectNames();
            //Console.WriteLine(results.Count());
            return View(results);
        }

        [HttpPost]
        public IActionResult Results(string ExamName,string SubjectName,string date,string score)
        {
            //UserRepository ur = new UserRepository(examContext);
            List<Result> results = userRepository.GetFilteredResults(ExamName,SubjectName,score,date);
            ViewBag.ExamNames = userRepository.GetExamNames();
            ViewBag.SubjectNames = userRepository.GetSubjectNames();
            Console.WriteLine(results.Count());
            return View(results);
          
        }
        [HttpGet]
        public ActionResult GetExamDetail(Guid id) {
            //UserRepository ur = new UserRepository(examContext);
            Exam exam = userRepository.GetExamDetail(id);
            return Json(exam);
            }

        [HttpGet]
        public ActionResult checkAnswer(Guid id)
        {
            try
            {
                return Json(userRepository.checkAnswer(id));
            }
            catch (Exception ex) {
     
                return RedirectToAction("User_Home");
            }

        }
        public async Task<ActionResult> saveScore(int score, Guid eid) {
            try
            {
                string id = "";
                User user = await _userManager.GetUserAsync(User);
                if (user != null)
                    id = user.Id;
               // Result e = await examContext.Results.Include(r => r.Exam).Include(e => e.User).Where(r => (r.User == user) && (r.Exam.ExamId == eid)).FirstOrDefaultAsync();
                //if (e == null)
                //{
                Result res = new Result();
                res.RowScore = score;
                //res.Exam = examContext.Exams.Where(e => e.ExamId == eid).First();
                res.Exam = userRepository.GetExam(eid);
             //    numberOfQuestions = examContext.Questions.Include(e => e.Exam).Where(q => q.Exam.ExamId == res.Exam.ExamId).Count();
               int numberOfQuestions = userRepository.GetNumberofQuestions(res.Exam.ExamId);
                res.outOf100 = (float)res.RowScore / (float)numberOfQuestions;
                res.outOf100 *= 100;
                res.User = user;
                res.DateTaken = DateTime.Now;
                userRepository.addScore(res);
                return RedirectToAction("Results");
            }
                //else { 
                //e.RowScore = score;
                //    int numberOfQuestions = examContext.Questions.Include(e => e.Exam).Where(q => q.Exam.ExamId == e.Exam.ExamId).Count();
                //    e.outOf100 = (float)e.RowScore / (float)numberOfQuestions;
                //    e.outOf100 *= 100;
                //    e.DateTaken = DateTime.Now;
                //   await examContext.SaveChangesAsync();
      
            
            catch (Exception e) {
                return RedirectToAction("User_Home");
            }
        }

        public ActionResult ResultDetail(Guid id)
        {
            ResultDetail res=userRepository.GetResultDetail(id);
            return Json(res);
        }
        //public ActionResult ComputeScore(Guid id) {
        //    float score = userRepository.ComputeScore(id);

        //    var jsonObject = new { scr= score};
        //    return Json(jsonObject);
        //}


    }
}
