using CounsellingManagementSystem.Data;
using CounsellingManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace CounsellingManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Register(Student student)
        {
            if (ModelState.IsValid)
            {
                var existingStudent = _context.Students
                    .FirstOrDefault(s => s.Email == student.Email);

                if (existingStudent != null)
                {
                    ViewBag.Error = "Email already registered.";
                    return View(student);
                }

                _context.Students.Add(student);
                _context.SaveChanges();

                TempData["Success"] = "Registration Successful";

                return RedirectToAction("Login");
            }

            return View(student);
        }


        // GET: Student/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Student/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var student = _context.Students
                .FirstOrDefault(s => s.Email == email && s.Password == password);

            if (student != null)
            {
                HttpContext.Session.SetInt32("StudentId", student.StudentId);
                HttpContext.Session.SetString("StudentName", student.FullName);
                HttpContext.Session.SetString("StudentEmail", student.Email);

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View();
        }

        public IActionResult Dashboard()
        {
            int? studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
                return RedirectToAction("Login");

            var student = _context.Students.Find(studentId);

            ViewBag.StudentName = student.FullName;

            ViewBag.TotalAppointments = _context.Appointments
                .Count(a => a.StudentId == studentId);

            ViewBag.PendingAppointments = _context.Appointments
                .Count(a => a.StudentId == studentId &&
                            a.Status == "Pending");

            var appointments = _context.Appointments
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.AppointmentId)
                .Take(5)
                .ToList();

            return View(appointments);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email,
                                            string newPassword,
                                            string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            var student = _context.Students
                                  .FirstOrDefault(s => s.Email == email);

            if (student == null)
            {
                ViewBag.Error = "Email not found.";
                return View();
            }

            student.Password = newPassword;

            _context.SaveChanges();

            TempData["Success"] = "Password updated successfully. Please login.";

            return RedirectToAction("Login");
        }


        // GET: Student/BookAppointment
        [HttpGet]
        public IActionResult BookAppointment()
        {
            if (HttpContext.Session.GetInt32("StudentId") == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Counsellors = _context.Counsellors.ToList();

            return View();
        }

        // POST: Student/BookAppointment
        [HttpPost]
        public IActionResult BookAppointment(Appointment appointment)
        {
            if (HttpContext.Session.GetInt32("StudentId") == null)
            {
                return RedirectToAction("Login");
            }

            appointment.StudentId = HttpContext.Session.GetInt32("StudentId").Value;

            // This is the selected counsellor from the dropdown.
            // It will be saved automatically because of model binding.

            appointment.Status = "Pending";

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            TempData["Success"] = "Appointment Booked Successfully.";

            return RedirectToAction("BookAppointment");
        }


        public IActionResult ViewAppointments()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
            {
                return RedirectToAction("Login");
            }

            var appointments = _context.Appointments
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            return View(appointments);
        }

        public IActionResult Profile()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
                return RedirectToAction("Login");

            var student = _context.Students.Find(studentId);

            return View(student);
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetInt32("StudentId") == null)
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword,
                                            string newPassword,
                                            string confirmPassword)
        {
            int? studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
                return RedirectToAction("Login");

            var student = _context.Students.Find(studentId);

            if (student == null)
                return RedirectToAction("Login");

            if (student.Password != currentPassword)
            {
                ViewBag.Error = "Current password is incorrect.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "New password and Confirm password do not match.";
                return View();
            }

            student.Password = newPassword;

            _context.SaveChanges();

            TempData["Success"] = "Password changed successfully.";

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            int? studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
                return RedirectToAction("Login");

            var student = _context.Students.Find(studentId);

            return View(student);
        }

        [HttpPost]
        public IActionResult EditProfile(Student model)
        {
            int? studentId = HttpContext.Session.GetInt32("StudentId");

            if (studentId == null)
                return RedirectToAction("Login");

            var student = _context.Students.Find(studentId);

            if (student == null)
                return RedirectToAction("Login");

            student.FullName = model.FullName;
            student.Email = model.Email;
            student.Phone = model.Phone;
            student.Department = model.Department;
            student.YearOfStudy = model.YearOfStudy;

            _context.SaveChanges();

            TempData["Success"] = "Profile updated successfully.";

            return RedirectToAction("Profile");
        }
    }
}