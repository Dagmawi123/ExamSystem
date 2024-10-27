using ExamSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ExamSystem.Models.ViewModels;

namespace ExamSystem.Controllers
{
    public class ExaminerController : Controller
    {
        private readonly ExamContext examContext;
       private readonly IWebHostEnvironment webHostEnvironment;
        public ExaminerController(IWebHostEnvironment webHostEnvironment, ExamContext context)
        {
            this.webHostEnvironment = webHostEnvironment;
            examContext = context;
        }


        public IActionResult Index()
        {
            ViewBag.Subjects = examContext.Subjects.ToList();
            List<Exam> examList = examContext.Exams.ToList();
            return View("Exam",examList);
        }

        [HttpPost]
        public async Task<IActionResult> SaveExam()
        {
            string requestBody;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            JObject json = JObject.Parse(requestBody);

            Subject sbj = examContext.Subjects.Where(s => s.SubjectName == (string)json["Subject"]!).First();
            Exam ex = new Exam()
            {
                ExamName = (string)json["Exam"]!,
                ExamDifficulty = "Proffessional",
                DateCreated = DateTime.Now,
                PassingMark = (int)json["Pass"]!,
                TimeAllocated = (string)json["Time"]!,
                QuestionWeight = (int)json["Value"]!,
                Subject = sbj,
            };

          await examContext.Exams.AddAsync(ex);
          

            JArray questionAnswerArray = (JArray)json["questions"]!;

            foreach (JObject questionAnswerObj in questionAnswerArray)
            {
                string question = (string)questionAnswerObj["quest"]!;
                Question qst = new Question() { 
                Query=question,
                Exam=ex
                };
                await examContext.Questions.AddAsync(qst);
              
                JArray answerArray = (JArray)questionAnswerObj["answer"]!;

                foreach (JArray answer in answerArray)
                {
                    bool answerValue = (bool)answer[0];
                    string answerText = (string)answer[1]!;

                    Answer asr = new Answer() { 
                    AnswerText=answerText,
                    isCorrect=answerValue,
                    Question=qst
                    };
                  await examContext.Answers.AddAsync(asr);
                
                }

            }
            try {
                await examContext.SaveChangesAsync();
            }
            catch (Exception esx) {
                Console.WriteLine(esx.Message);
            }



            return Ok(); // Return an appropriate response
           
        }
        public async Task<IActionResult>  SaveEdit() {
            string requestBody;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            JObject json = JObject.Parse(requestBody);

            Subject sbj = examContext.Subjects.Where(s => s.SubjectName == (string)json["Subject"]!).First();
            Exam ex = examContext.Exams.Where(e => e.ExamId == (Guid)json["Id"]!).First();
            ex.Subject = sbj;
            ex.ExamName = (string)json["Exam"]!;
            // ex.ExamDifficulty = "Proffessional",
            ex.DateCreated = DateTime.Now;
            ex.PassingMark = (int)json["Pass"]!;
            ex.TimeAllocated = (string)json["Time"]!;
            ex.QuestionWeight = (int)json["Value"]!;
            await examContext.SaveChangesAsync();

            JArray questionAnswerArray = (JArray)json["questions"]!;
            foreach (JObject questionAnswerObj in questionAnswerArray)
            {
                string question = (string)questionAnswerObj["quest"]!;
                Question qst = examContext.Questions.Where(q => q.QuesionId == (Guid)questionAnswerObj["QId"]!).First();
                qst.Query = question;
                await examContext.SaveChangesAsync();
                JArray answerArray = (JArray)questionAnswerObj["answer"]!;

                foreach (JArray answer in answerArray)
                {
                    bool answerValue = (bool)answer[1];
                    string answerText = (string)answer[2]!;

                    Answer asr = examContext.Answers.Where(e => e.AnswerId == (Guid)answer[0]).First();
                    asr.AnswerText = answerText;
                    asr.isCorrect=answerValue;                  

                    await examContext.SaveChangesAsync();
                }

            }

            return Ok();
        }





        public IActionResult EditExam(Guid id)
        {
            Exam ex = examContext.Exams.Include(e => e.Subject).Include(e => e.Questions).ThenInclude(q => q.Answers).Where(e => e.ExamId == id).First();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 64 // Set the maximum depth as needed
            };

            string json = JsonSerializer.Serialize(ex, options);
            return Content(json, "application/json");
        }

        [HttpGet]
        public IActionResult Reference()
        {
            ViewBag.Subjects = examContext.Subjects.Select(e => e.SubjectName).ToList();
            ViewBag.Docs = examContext.Documents.Include(d=>d.Subject).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Reference(ReferenceViewModel reference)
        {
            string FileName = "";
            string uniqueCFileName = "";
            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "docs");
            if (ModelState.IsValid) { 
            }
            if (reference.DocPath != null)
            {
                 FileName = Guid.NewGuid().ToString() + "_" + reference.DocPath.FileName;
                uniqueCFileName = Path.Combine(uploadFolder, FileName);
                reference.DocPath.CopyTo(new FileStream(uniqueCFileName, FileMode.Create));
            }
                try
                {
                    Subject sbje = examContext.Subjects.Where(s => s.SubjectName == reference.Subject).First();
                    Models.Document doc = new Models.Document()
                    {
                        DateAdded = DateTime.Now,
                        DocName = reference.DocName,
                        Subject = sbje,
                        DocVersion = reference.DocVersion,
                        DocPath = "/docs/" + FileName

                    };
                    examContext.Documents.Add(doc);
                    await examContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Something went wrong{ex.Message}");
                }
            ModelState.AddModelError(string.Empty, $"Something went wrong invalid model");
            return View();
        }


        [HttpGet]
        public IActionResult Subject()
        {
            ViewBag.Subjects = examContext.Subjects.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Subject(SubjectViewModel subject)
        {
            string FileName = "";
            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "docs");

            if (subject.ImageUrl != null)
            {
                 FileName = Guid.NewGuid().ToString() + "_" + subject.ImageUrl.FileName;
                string uniqueCFileName = Path.Combine(uploadFolder, FileName);
                subject.ImageUrl.CopyTo(new FileStream(uniqueCFileName, FileMode.Create));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    Subject sbj = new Subject() { 
                    SubjectName = subject.SubjectName,
                    Description = subject.Description,
                    ImageUrl="/docs/"+ FileName
                    };
                    examContext.Add(sbj);
                    await examContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Something went wrong{ex.Message}");
                 
                }
            }
            ModelState.AddModelError(string.Empty, $"Something went wrong invalid model");
            return View();
        }

        public async Task<IActionResult> DeleteExam(Guid id)
        {
            Exam examToDelete=examContext.Exams.Include(e => e.Questions)
                .ThenInclude(q => q.Answers)
            .First(e => e.ExamId == id); 
           
         
                if (examToDelete != null)
                {

                    // Delete the associated answers for each question
                    foreach (Question question in examToDelete.Questions)
                    {
                        examContext.Answers.RemoveRange(question.Answers);
                    }

                    // Delete the questions associated with the exam
                    examContext.Questions.RemoveRange(examToDelete.Questions);

                    // Delete the exam entity
                    examContext.Exams.Remove(examToDelete);

                    // Save changes to the database
                  await  examContext.SaveChangesAsync();
                }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Overview()
        {
         
              List<Exam> examList = examContext.Exams.ToList();
                //fro the pass and fail from result table through exam id relation
                return View(examList);
            
        }
        [HttpGet]
        public IActionResult Result()
        {


            return View();
        }

        public async Task<ActionResult> DeleteDoc(Guid id)
        {
            Models.Document? doc = await examContext.Documents.FindAsync(id);
            if (doc != null)
            {
                examContext.Documents.Remove(doc);
                await examContext.SaveChangesAsync();
            }
            return RedirectToAction("Reference");

        }
        public async Task<ActionResult> DeleteSub(Guid id)
        {
            Models.Subject? Sub = await examContext.Subjects.FindAsync(id);
            if (Sub != null)
            {
                examContext.Subjects.Remove(Sub);
                await examContext.SaveChangesAsync();
            }
            return RedirectToAction("Subject");

        }

        public IActionResult Dash()
        {

            ViewBag.Subjects = examContext.Subjects.Count();
            ViewBag.Exams = examContext.Exams.Count();
            List<Exam> examList = examContext.Exams.ToList();
            ViewBag.Docs = examContext.Documents.Include(d => d.Subject).ToList().Count();

            return View("Dashboard");
        }
        [HttpPost]
        public IActionResult Index(int a)
        {
            return View();
        }


    }
}

