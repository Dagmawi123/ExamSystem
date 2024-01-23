using ExamSystem.Models;
using ExamSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        public IActionResult User_Home()
        {
            //UserRepository ur = new UserRepository(examContext);
            List<Exam> exams = userRepository.GetExams();

            return View(exams);
        }

        [HttpGet]
        public IActionResult Documents()
        {
            //UserRepository ur = new UserRepository(examContext);
            DocumentViewModel dvm = userRepository.GetDocuments();
            // Console.WriteLine(docs[0].Subject.SubjectName);
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
                Guid id = new Guid("696384b7-0e20-4671-b28b-1f8676d892e6");
                Result res = new Result();
                res.Score = score;
                res.Exam = examContext.Exams.Where(e => e.ExamId == eid).First();
                res.User = examContext.Users.Where(e => e.UserId == id).First();
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


    }
}
