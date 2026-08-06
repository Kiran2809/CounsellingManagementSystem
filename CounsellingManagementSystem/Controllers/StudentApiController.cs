using CounsellingManagementSystem.Data;
using CounsellingManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CounsellingManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudentApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Students.ToListAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return Ok(student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, Student updatedStudent)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            student.FullName = updatedStudent.FullName;
            student.Email = updatedStudent.Email;
            student.Password = updatedStudent.Password;
            student.Phone = updatedStudent.Phone;
            student.Department = updatedStudent.Department;
            student.YearOfStudy = updatedStudent.YearOfStudy;

            await _context.SaveChangesAsync();

            return Ok(student);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            // Delete appointments first
            var appointments = _context.Appointments
                                       .Where(a => a.StudentId == id)
                                       .ToList();

            _context.Appointments.RemoveRange(appointments);

            // Delete student
            _context.Students.Remove(student);

            await _context.SaveChangesAsync();

            return Ok("Student and related appointments deleted successfully.");
        }
    }
}