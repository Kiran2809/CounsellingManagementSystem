using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CounsellingManagementSystem.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public string AppointmentTime { get; set; }

        [Required]
        public string Reason { get; set; }

        public string Status { get; set; } = "Pending";
        public string? Remarks { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }

        public Student? Student { get; set; }

        public int CounsellorId { get; set; }

        public Counsellor? Counsellor { get; set; }
    }
}