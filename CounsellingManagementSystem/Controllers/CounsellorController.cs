using CounsellingManagementSystem.Data;
using CounsellingManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CounsellingManagementSystem.Controllers
{
    public class CounsellorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CounsellorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var counsellor = _context.Counsellors
                .FirstOrDefault(c => c.Email == email && c.Password == password);

            if (counsellor != null)
            {
                HttpContext.Session.SetInt32("CounsellorId", counsellor.CounsellorId);
                HttpContext.Session.SetString("CounsellorName", counsellor.FullName);

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid Login";

            return View();
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("CounsellorId") == null)
                return RedirectToAction("Login");

            int counsellorId = HttpContext.Session.GetInt32("CounsellorId").Value;

            ViewBag.CounsellorName = HttpContext.Session.GetString("CounsellorName");

            ViewBag.TotalAppointments = _context.Appointments
                .Count(a => a.CounsellorId == counsellorId);

            ViewBag.Pending = _context.Appointments
                .Count(a => a.CounsellorId == counsellorId &&
                            a.Status == "Pending");

            ViewBag.Approved = _context.Appointments
                .Count(a => a.CounsellorId == counsellorId &&
                            a.Status == "Approved");

            ViewBag.Rejected = _context.Appointments
                .Count(a => a.CounsellorId == counsellorId &&
                            a.Status == "Rejected");

            var appointments = _context.Appointments
                .Include(a => a.Student)
                .Where(a => a.CounsellorId == counsellorId)
                .OrderByDescending(a => a.AppointmentId)
                .Take(5)
                .ToList();

            return View(appointments);
        }

        public IActionResult PendingAppointments()
        {
            if (HttpContext.Session.GetInt32("CounsellorId") == null)
                return RedirectToAction("Login");

            int counsellorId = HttpContext.Session.GetInt32("CounsellorId").Value;

            var appointments = _context.Appointments
                .Include(a => a.Student)
                .Where(a => a.CounsellorId == counsellorId &&
                            a.Status == "Pending")
                .ToList();

            return View(appointments);
        }

        public IActionResult ReviewedAppointments()
        {
            if (HttpContext.Session.GetInt32("CounsellorId") == null)
                return RedirectToAction("Login");

            int counsellorId = HttpContext.Session.GetInt32("CounsellorId").Value;

            var appointments = _context.Appointments
                .Include(a => a.Student)
                .Where(a => a.CounsellorId == counsellorId &&
                           (a.Status == "Approved" ||
                            a.Status == "Rejected"))
                .ToList();

            return View(appointments);
        }

        public IActionResult ApproveAppointment(int id)
        {
            var appointment = _context.Appointments.Find(id);

            if (appointment != null)
            {
                appointment.Status = "Approved";
                appointment.Remarks = "Approved by Counsellor";

                _context.SaveChanges();

                TempData["Success"] = "Appointment Approved Successfully.";
            }

            return RedirectToAction("PendingAppointments");
        }

        public IActionResult RejectAppointment(int id)
        {
            var appointment = _context.Appointments.Find(id);

            if (appointment != null)
            {
                appointment.Status = "Rejected";
                appointment.Remarks = "Rejected by Counsellor";

                _context.SaveChanges();

                TempData["Success"] = "Appointment Rejected Successfully.";
            }

            return RedirectToAction("PendingAppointments");
        }


        [HttpGet]
        public IActionResult ReviewAppointment(int id)
        {
            if (HttpContext.Session.GetInt32("CounsellorId") == null)
            {
                return RedirectToAction("Login");
            }

            var appointment = _context.Appointments
                .Include(a => a.Student)
                .FirstOrDefault(a => a.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpPost]
        public IActionResult ReviewAppointment(Appointment appointment)
        {
            var existing = _context.Appointments
                .FirstOrDefault(a => a.AppointmentId == appointment.AppointmentId);

            if (existing == null)
            {
                return NotFound();
            }

            existing.Status = appointment.Status;
            existing.Remarks = appointment.Remarks;

            _context.SaveChanges();

            TempData["Success"] = "Appointment updated successfully.";

            return RedirectToAction("ReviewedAppointments");
        }

        //public IActionResult Appointments()
        //{
        //    if (HttpContext.Session.GetInt32("CounsellorId") == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    var appointments = _context.Appointments
        //        .Include(a => a.Student)
        //        .ToList();

        //    return View(appointments);
        //}

        public IActionResult Profile()
        {
            int? counsellorId = HttpContext.Session.GetInt32("CounsellorId");

            if (counsellorId == null)
                return RedirectToAction("Login");

            var counsellor = _context.Counsellors
                .FirstOrDefault(c => c.CounsellorId == counsellorId);

            if (counsellor == null)
                return NotFound();

            return View(counsellor);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetInt32("CounsellorId") == null)
                return RedirectToAction("Login");

            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            int? counsellorId = HttpContext.Session.GetInt32("CounsellorId");

            if (counsellorId == null)
                return RedirectToAction("Login");

            var counsellor = _context.Counsellors
                .FirstOrDefault(c => c.CounsellorId == counsellorId);

            if (counsellor == null)
                return NotFound();

            if (counsellor.Password != currentPassword)
            {
                ViewBag.Error = "Current password is incorrect.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "New passwords do not match.";
                return View();
            }

            counsellor.Password = newPassword;

            _context.SaveChanges();

            TempData["Success"] = "Password changed successfully.";

            return RedirectToAction("Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}