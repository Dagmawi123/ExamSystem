using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Models.ViewModels
{
    public class SubjectViewModel
    {
        public SubjectViewModel() { }
        // [Display(Name = "SubjectName")]
        public string SubjectName { get; set; }
        // [Display(Name = "Description")]
        public string Description { get; set; }
        // [Display(Name = "ImageUrl")]
        public IFormFile ImageUrl { get; set; } //to static file && database [picture]

    }
}