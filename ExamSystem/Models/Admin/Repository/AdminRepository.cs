//using ExamSystem.Models;
//using ExamSystem.Models;
namespace ExamSystem.Models.Admin.Repository
{
    public class AdminRepository
    {
        private readonly ExamContext dbcont;
        private readonly IWebHostEnvironment _environment;

        public AdminRepository(ExamContext  dbcontext, IWebHostEnvironment webHostEnvironment)
        {
            _environment = webHostEnvironment;
            dbcont = dbcontext;
        }

        


    }
}
