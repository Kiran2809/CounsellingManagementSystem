using Microsoft.EntityFrameworkCore;
using CounsellingManagementSystem.Models;

namespace CounsellingManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<Counsellor> Counsellors { get; set; }
    }
}