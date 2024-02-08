using ExamSystem.Models;
using ExamSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Nodes;
//using Microsoft.AspNetCore.Mvc;
//using System.Web.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using ExamSystem.Models.User;

namespace ExamSystem.Controllers
{
    public class ExamineeController : Controller
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        //private readonly ProductRepository productRepository;
        private readonly ExamContext examContext;
        private readonly IExamineeRepository userRepository;

        public ExamineeController(ExamContext examContext, IWebHostEnvironment webHostEnvironment,IExamineeRepository userRepo)
        {
            this.examContext = examContext;
            this.webHostEnvironment = webHostEnvironment;
            //this.productRepository = productRepository;
            userRepository = userRepo;


        }
        [HttpGet]
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
        public IActionResult Results() {
            //UserRepository ur = new UserRepository(examContext);
            List<Result> results = userRepository.GetResults();
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
                bool value = examContext.Answers.Where(a => a.AnswerId== id).Select(a => a.isCorrect).First();
                return Json(value);
            }
            catch (Exception ex) {
     
                return RedirectToAction("User_Home");
            }

        }
        public async Task<ActionResult> saveScore(int score,Guid eid) {
            try
            {
                string id = "85FE0EE1-22CC-42D4-8816-9DE99438188C";
                Result res = new Result();
                res.RowScore = score;
                res.Exam = examContext.Exams.Where(e => e.ExamId == eid).First();
                int numberOfQuestions = examContext.Questions.Include(e => e.Exam).Where(q => q.Exam.ExamId == res.Exam.ExamId).Count();
                res.outOf100=(float)res.RowScore / (float)numberOfQuestions;
                res.outOf100 *= 100;
                res.User =  examContext.Users.Where(e => e.Id == id).First();
                res.DateTaken= DateTime.Now;

                //UserRepository ur = new UserRepository(examContext);
                userRepository.addScore(res);

                return RedirectToAction("User_Home");
            }
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
