using CounsellingManagementSystem.Data;
using CounsellingManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ClosedXML.Excel;
using System.IO;

namespace CounsellingManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var admin = _context.Admins
                .FirstOrDefault(a => a.Username == username && a.Password == password);

            if (admin != null)
            {
                HttpContext.Session.SetInt32("AdminId", admin.AdminId);
                HttpContext.Session.SetString("AdminName", admin.Username);

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid Username or Password";
            return View();
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.TotalStudents = _context.Students.Count();
            ViewBag.TotalAppointments = _context.Appointments.Count();
            ViewBag.TotalCounsellors = _context.Counsellors.Count();
            ViewBag.Pending = _context.Appointments.Count(a => a.Status == "Pending");
            ViewBag.Approved = _context.Appointments.Count(a => a.Status == "Approved");
            ViewBag.Rejected = _context.Appointments.Count(a => a.Status == "Rejected");

            var recentAppointments = _context.Appointments
                .Include(a => a.Student)
                .OrderByDescending(a => a.AppointmentId)
                .Take(5)
                .ToList();

            return View(recentAppointments);
        }

        public IActionResult ExportToPdf()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var appointments = _context.Appointments
                .Include(a => a.Student)
                .Include(a => a.Counsellor)
                .ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 20, 20, 20, 20);

                PdfWriter.GetInstance(document, ms);

                document.Open();

                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);

                document.Add(new Paragraph("Counselling Management System", titleFont));
                document.Add(new Paragraph("Appointment Report"));
                document.Add(new Paragraph("Generated On: " + DateTime.Now));
                document.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(6);

                table.WidthPercentage = 100;

                table.AddCell("Student");
                table.AddCell("Counsellor");
                table.AddCell("Date");
                table.AddCell("Time");
                table.AddCell("Status");
                table.AddCell("Remarks");

                foreach (var item in appointments)
                {
                    table.AddCell(item.Student?.FullName ?? "");
                    table.AddCell(item.Counsellor?.FullName ?? "");
                    table.AddCell(item.AppointmentDate.ToShortDateString());
                    table.AddCell(item.AppointmentTime);
                    table.AddCell(item.Status);
                    table.AddCell(item.Remarks ?? "");
                }

                document.Add(table);

                document.Close();

                return File(
                    ms.ToArray(),
                    "application/pdf",
                    "AppointmentsReport.pdf");
            }
        }

        //public IActionResult ViewStudents()
        //{
        //    if (HttpContext.Session.GetInt32("AdminId") == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    var students = _context.Students.ToList();

        //    return View(students);
        //}

        public IActionResult ExportStudentsPdf()
        {
            var students = _context.Students.ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4);

                PdfWriter.GetInstance(document, ms);

                document.Open();

                document.Add(new Paragraph("Students Report"));
                document.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(5);

                table.WidthPercentage = 100;

                table.AddCell("Name");
                table.AddCell("Email");
                table.AddCell("Phone");
                table.AddCell("Department");
                table.AddCell("Year");

                foreach (var s in students)
                {
                    table.AddCell(s.FullName);
                    table.AddCell(s.Email);
                    table.AddCell(s.Phone);
                    table.AddCell(s.Department);
                    table.AddCell(s.YearOfStudy.ToString());
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(),
                    "application/pdf",
                    "StudentsReport.pdf");
            }
        }

        public IActionResult ExportStudentsExcel()
        {
            var students = _context.Students.ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Students");

                ws.Cell(1, 1).Value = "Name";
                ws.Cell(1, 2).Value = "Email";
                ws.Cell(1, 3).Value = "Phone";
                ws.Cell(1, 4).Value = "Department";
                ws.Cell(1, 5).Value = "Year";

                int row = 2;

                foreach (var s in students)
                {
                    ws.Cell(row, 1).Value = s.FullName;
                    ws.Cell(row, 2).Value = s.Email;
                    ws.Cell(row, 3).Value = s.Phone;
                    ws.Cell(row, 4).Value = s.Department;
                    ws.Cell(row, 5).Value = s.YearOfStudy;

                    row++;
                }

                ws.Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "StudentsReport.xlsx");
                }
            }
        }

        public IActionResult ExportCounsellorsPdf()
        {
            var counsellors = _context.Counsellors.ToList();

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4);

                PdfWriter.GetInstance(document, ms);

                document.Open();

                document.Add(new Paragraph("Counsellors Report"));
                document.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(3);

                table.WidthPercentage = 100;

                table.AddCell("Name");
                table.AddCell("Email");
                table.AddCell("Department");

                foreach (var c in counsellors)
                {
                    table.AddCell(c.FullName);
                    table.AddCell(c.Email);
                    table.AddCell(c.Department);
                }

                document.Add(table);

                document.Close();

                return File(ms.ToArray(),
                    "application/pdf",
                    "CounsellorsReport.pdf");
            }
        }

        public IActionResult ExportCounsellorsExcel()
        {
            var counsellors = _context.Counsellors.ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Counsellors");

                ws.Cell(1, 1).Value = "Name";
                ws.Cell(1, 2).Value = "Email";
                ws.Cell(1, 3).Value = "Department";

                int row = 2;

                foreach (var c in counsellors)
                {
                    ws.Cell(row, 1).Value = c.FullName;
                    ws.Cell(row, 2).Value = c.Email;
                    ws.Cell(row, 3).Value = c.Department;

                    row++;
                }

                ws.Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "CounsellorsReport.xlsx");
                }
            }
        }

        public IActionResult ExportToExcel()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var appointments = _context.Appointments
                .Include(a => a.Student)
                .Include(a => a.Counsellor)
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Appointments");

                worksheet.Cell(1, 1).Value = "Student";
                worksheet.Cell(1, 2).Value = "Counsellor";
                worksheet.Cell(1, 3).Value = "Date";
                worksheet.Cell(1, 4).Value = "Time";
                worksheet.Cell(1, 5).Value = "Reason";
                worksheet.Cell(1, 6).Value = "Status";
                worksheet.Cell(1, 7).Value = "Remarks";

                int row = 2;

                foreach (var item in appointments)
                {
                    worksheet.Cell(row, 1).Value = item.Student?.FullName;
                    worksheet.Cell(row, 2).Value = item.Counsellor?.FullName;
                    worksheet.Cell(row, 3).Value = item.AppointmentDate.ToShortDateString();
                    worksheet.Cell(row, 4).Value = item.AppointmentTime;
                    worksheet.Cell(row, 5).Value = item.Reason;
                    worksheet.Cell(row, 6).Value = item.Status;
                    worksheet.Cell(row, 7).Value = item.Remarks;

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "AppointmentsReport.xlsx");
                }
            }
        }


        public IActionResult ViewAppointments(string status)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var appointments = _context.Appointments
                .Include(a => a.Student)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                appointments = appointments.Where(a => a.Status == status);
            }

            ViewBag.Status = status;

            return View(appointments.ToList());
        }


        public IActionResult DeleteCounsellor(int id)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var counsellor = _context.Counsellors.Find(id);

            if (counsellor == null)
            {
                return NotFound();
            }

            // Delete all appointments assigned to this counsellor
            var appointments = _context.Appointments
                                       .Where(a => a.CounsellorId == id)
                                       .ToList();

            _context.Appointments.RemoveRange(appointments);

            // Delete counsellor
            _context.Counsellors.Remove(counsellor);

            _context.SaveChanges();

            TempData["Success"] = "Counsellor deleted successfully.";

            return RedirectToAction("ViewCounsellors");
        }

        public IActionResult ViewStudents(string search)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var students = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                students = students.Where(s =>
                    s.FullName.Contains(search) ||
                    s.Email.Contains(search));
            }

            ViewBag.Search = search;

            return View(students.ToList());
        }

        public IActionResult DeleteStudent(int id)
        {
            var student = _context.Students.Find(id);

            if (student == null)
            {
                return NotFound();
            }

            var appointments = _context.Appointments
                                       .Where(a => a.StudentId == id)
                                       .ToList();

            TempData["Success"] = $"Found {appointments.Count} appointments for StudentId = {id}";

            _context.Appointments.RemoveRange(appointments);
            _context.Students.Remove(student);

            _context.SaveChanges();

            return RedirectToAction("ViewStudents");
        }

        public IActionResult StudentDetails(int id)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var student = _context.Students
                                  .FirstOrDefault(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        public IActionResult ViewCounsellors(string search)
        {
            var counsellors = _context.Counsellors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                counsellors = counsellors.Where(c =>
                    c.FullName.Contains(search) ||
                    c.Email.Contains(search) ||
                    c.Department.Contains(search));
            }

            ViewBag.Search = search;

            return View(counsellors.ToList());
        }

        [HttpGet]
        public IActionResult AddCounsellor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCounsellor(Counsellor counsellor)
        {
            if (ModelState.IsValid)
            {
                var exists = _context.Counsellors
                    .FirstOrDefault(c => c.Email == counsellor.Email);

                if (exists != null)
                {
                    ViewBag.Error = "Email already exists.";
                    return View(counsellor);
                }

                _context.Counsellors.Add(counsellor);
                _context.SaveChanges();

                TempData["Success"] = "Counsellor added successfully.";

                return RedirectToAction("ViewCounsellors");
            }

            return View(counsellor);
        }

        public IActionResult DeleteAppointment(int id)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            var appointment = _context.Appointments.Find(id);

            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            TempData["Success"] = "Appointment deleted successfully.";

            return RedirectToAction("ViewAppointments");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}