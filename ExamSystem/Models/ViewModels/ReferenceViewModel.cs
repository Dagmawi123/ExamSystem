using ExamSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Models.ViewModels
{
    public class ReferenceViewModel
    {
        public ReferenceViewModel() { }

        public string DocName { get; set; }

        public string Subject { get; set; } //dropdown      

        public float DocVersion { get; set; } // 

        public DateTime DateAdded { get; set; }

        public IFormFile DocPath { get; set; } //url path wwwroot

    }
}