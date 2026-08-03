using System.ComponentModel.DataAnnotations;

namespace CounsellingManagementSystem.Models
{
    public class Counsellor
    {
        [Key]
        public int CounsellorId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Department { get; set; }
    }
}