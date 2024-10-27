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
        private readonly ExamContext examContext;
        private readonly IExamineeRepository userRepository;
        private readonly UserManager<User> _userManager;

        public ExamineeController(ExamContext examContext, IWebHostEnvironment webHostEnvironment,IExamineeRepository userRepo, UserManager<User> _userManager)
        {
            this.examContext = examContext;
            this.webHostEnvironment = webHostEnvironment;
            userRepository = userRepo;
            this._userManager = _userManager;

        }
        [HttpGet]
        [Route("/Examinee/")]
        public IActionResult User_Home()
        {
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
            DocumentViewModel dvm = userRepository.GetDocuments();
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
            Exam ex = userRepository.GetExam(id);
            return View(ex);
        }
        [HttpGet]
        public async Task<IActionResult> Results() {
            User? usr=await _userManager.GetUserAsync(User);
            List<Result> results =userRepository.GetResults(usr!);
            ViewBag.ExamNames = userRepository.GetExamNames();
            ViewBag.SubjectNames = userRepository.GetSubjectNames();
            return View(results);
        }

        [HttpPost]
        public IActionResult Results(string ExamName,string SubjectName,string date,string score)
        {   string userId=_userManager.GetUserId(User)!;
            List<Result> results = userRepository.GetFilteredResults(ExamName,SubjectName,score,date,userId);
            ViewBag.ExamNames = userRepository.GetExamNames();
            ViewBag.SubjectNames = userRepository.GetSubjectNames();
           // Console.WriteLine(results.Count());
            return View(results);
          
        }
        [HttpGet]
        public ActionResult GetExamDetail(Guid id) {
            Exam exam = userRepository.GetExamDetail(id);
            return Json(exam);
            }

        [HttpGet]
        public async Task<ActionResult> checkAnswer(Guid id)
        {
            try
            {
                return  Json(await userRepository.checkAnswer(id));
            }
            catch (Exception ex) {
     
                return RedirectToAction("User_Home");
            }

        }
        [HttpGet]
        public async Task<ActionResult> SampleAction()
        {
            await Task.CompletedTask;
            return RedirectToAction("User_Home");
        }
        [HttpPost]
        public async Task<ActionResult> SaveScore(int score, Guid eid)
        {
               Console.ForegroundColor=ConsoleColor.Red;
            Console.WriteLine("Called Action method");
              Console.ForegroundColor=ConsoleColor.White;
            try
             {
                 string id = "";
                 User? user = await _userManager.GetUserAsync(User);
                 if (user != null)
                     id = user.Id;
                 Result? e = await examContext.Results.Include(r => r.Exam)
                     .Include(e => e.User).Where(r => (r.User == user) && (r.Exam!=null)&&(r.Exam.ExamId == eid)).
                     FirstOrDefaultAsync();
                 if (e == null)
                 {
                     Result res = new Result();
                     res.RowScore = score;
                     res.Exam = userRepository.GetExam(eid);
                      int numberOfQuestions = userRepository.GetNumberofQuestions(res.Exam.ExamId);
                     res.outOf100 = (float)res.RowScore / (float)numberOfQuestions;
                     res.outOf100 *= 100;
                     res.User = user;
                     res.DateTaken = DateTime.Now;
                     userRepository.addScore(res);
                     return RedirectToAction("Results");
                 }
                 else
                 {
                     e.RowScore = score;
                     int numberOfQuestions = examContext.Questions.Include(e => e.Exam).Where(q => q.Exam.ExamId == e.Exam.ExamId).Count();
                     e.outOf100 = (float)e.RowScore / (float)numberOfQuestions;
                     e.outOf100 *= 100;
                     e.DateTaken = DateTime.Now;
                     string sql = "UPDATE Results SET outOf100=" +e.outOf100+",RowScore="+e.RowScore+",DateTaken='"+DateTime.Now+ "' WHERE ResultId='"+e.ResultId+"'";
                     int r=examContext.Database.ExecuteSqlRaw(sql);
                     return RedirectToAction("Results");
                     //await examContext.SaveChangesAsync();
                 }
             }
             catch (Exception e)
             {
                 return RedirectToAction("User_Home");
             }
         
        //     await Task.CompletedTask;
        //   return RedirectToAction("User_Home");
        }
        public ActionResult ResultDetail(Guid id)
        {
            ResultDetail res=userRepository.GetResultDetail(id);
            return Json(res);
        }
    }
}