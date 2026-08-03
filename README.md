# 🎓 Counselling Management System

A complete **ASP.NET Core MVC-based Counselling Management System** developed to simplify and manage student counselling appointments in educational institutions.

This system provides separate modules for **Admin**, **Student**, and **Counsellor**, allowing efficient appointment booking, approval, counselling management, and report generation.

---

# 📌 Project Overview

The Counselling Management System is designed to digitize the counselling process within colleges and universities.

Students can register and request counselling appointments, counsellors can review and manage appointments, and administrators can oversee the entire system through a centralized dashboard.

---

# ✨ Features

## 👨‍🎓 Student Module

- Student Registration
- Student Login
- Secure Session Management
- Book Counselling Appointment
- View Appointment History
- View Appointment Status
- Logout

---

## 👨‍🏫 Counsellor Module

- Counsellor Login
- Personalized Dashboard
- View Assigned Appointments
- Approve Appointments
- Reject Appointments
- Add Counselling Remarks
- View Pending Appointments
- View Reviewed Appointments
- Logout

---

## 👨‍💼 Admin Module

- Admin Login
- Dashboard with Statistics
- Manage Students
- Manage Counsellors
- Delete Students
- Delete Counsellors
- Delete Appointments
- Export Students to PDF
- Export Students to Excel
- Export Counsellors to PDF
- Export Counsellors to Excel
- Export Appointments to PDF
- Export Appointments to Excel
- Search Students
- Search Counsellors
- Search Appointments
- View Recent Appointments

---

# 📊 Dashboard Statistics

The Admin Dashboard displays:

- Total Students
- Total Counsellors
- Total Appointments
- Pending Appointments
- Approved Appointments
- Rejected Appointments
- Recent Appointments

---

# 🛠 Technologies Used

### Backend

- ASP.NET Core MVC
- C#
- Entity Framework Core

### Frontend

- HTML5
- CSS3
- Bootstrap 5
- JavaScript
- Razor Views (.cshtml)

### Database

- Microsoft SQL Server
- Entity Framework Core Migrations

### Libraries

- ClosedXML (Excel Export)
- iTextSharp (PDF Export)

### IDE

- Microsoft Visual Studio 2026

---

# 🗄 Database Tables

- Admins
- Students
- Counsellors
- Appointments

---

# 🔄 System Workflow

Student
↓

Registers/Login
↓

Books Appointment
↓

Counsellor Reviews
↓

Approve / Reject
↓

Student Views Status
↓

Admin Monitors Everything

---

# 📁 Project Structure

```
CounsellingManagementSystem
│
├── Controllers
│
├── Models
│
├── Views
│
├── Data
│
├── wwwroot
│
├── Migrations
│
├── Program.cs
│
└── appsettings.json
```

---

# 🚀 Installation

## Clone Repository

```bash
git clone https://github.com/Kiran2809/CounsellingManagementSystem.git
```

Open the project in **Visual Studio 2026**.

Restore NuGet Packages.

Update the SQL Server connection string in **appsettings.json**.

Run Entity Framework migrations:

```powershell
Update-Database
```

Run the project.

---

# 🔐 Default Login Credentials

## Admin

```
Username : admin
Password : admin123
```

## Counsellor

```
Email : ramesh@gmail.com
Password : Ramesh@123
```

```
Email : rahul1234@gmail.com
Password : Rahul@123
```

## Student

Create a new account from the Registration page.

---

# 🎯 Future Enhancements

- Forgot Password using Email OTP
- Email Notifications
- SMS Notifications
- Online Video Counselling
- Calendar Integration
- Role-Based Authorization
- Appointment Reminder Emails
- Student Feedback System
- Counselling Reports
- Analytics Dashboard

---

# 👨‍💻 Developed By

**Kiran Kumar**

B.Tech – Electronics and Communication Engineering (ECE)

Application Developer

---

# 📜 License

This project is developed for educational and learning purposes.

---

# ⭐ Support

If you found this project useful, consider giving it a ⭐ on GitHub.
