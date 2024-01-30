using System.ComponentModel.DataAnnotations;

namespace ExamSystem.Models
{
    public class Document
    {
        [Key]
        public Guid DocId { get; set; }
        [Required]
        public string DocName { get; set; }
        //[Required]
        //public Guid DocTypeId { get; set; }
        public Subject Subject { get; set; }    
        [Required]
        public float DocVersion { get; set; }
        [Required]
        public DateTime DateAdded { get; set; }
        [Required]
        public string DocPath { get; set; }
        
    }
}
