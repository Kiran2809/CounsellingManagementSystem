using System.ComponentModel.DataAnnotations;

namespace CounsellingManagementSystem.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Phone { get; set; }

        public string Department { get; set; }

        public int YearOfStudy { get; set; }
    }
}