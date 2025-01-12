using Microsoft.EntityFrameworkCore;
using StarterKit.Utils;
using System;
using System.Collections.Generic;

namespace StarterKit.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Event_Attendance> EventAttendances { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>()
                .HasIndex(p => p.UserName).IsUnique();

            modelBuilder.Entity<Admin>()
                .HasData(new Admin { AdminId = 1, Email = "admin1@example.com", UserName = "admin1", Password = EncryptionHelper.EncryptPassword("password") });
            modelBuilder.Entity<Admin>()
                .HasData(new Admin { AdminId = 2, Email = "admin2@example.com", UserName = "admin2", Password = EncryptionHelper.EncryptPassword("tooeasytooguess") });
            modelBuilder.Entity<Admin>()
                .HasData(new Admin { AdminId = 3, Email = "admin3@example.com", UserName = "admin3", Password = EncryptionHelper.EncryptPassword("helloworld") });
            modelBuilder.Entity<Admin>()
                .HasData(new Admin { AdminId = 4, Email = "admin4@example.com", UserName = "admin4", Password = EncryptionHelper.EncryptPassword("Welcome123") });
            modelBuilder.Entity<Admin>()
                .HasData(new Admin { AdminId = 5, Email = "admin5@example.com", UserName = "admin5", Password = EncryptionHelper.EncryptPassword("Whatisapassword?") });

            // Seed sample events
            modelBuilder.Entity<Event>().HasData(
                new Event 
                { 
                    EventId = 1, 
                    Title = "Team Meeting", 
                    Description = "Monthly team sync-up", 
                    EventDate = new DateTime(2023, 10, 15), 
                    StartTime = new TimeSpan(10, 0, 0), 
                    EndTime = new TimeSpan(11, 0, 0), 
                    Location = "Conference Room A", 
                    AdminApproval = true,
                    Event_Attendances = new List<Event_Attendance>() 
                },
                new Event 
                { 
                    EventId = 2, 
                    Title = "Office Party", 
                    Description = "End of year celebration", 
                    EventDate = new DateTime(2023, 12, 20), 
                    StartTime = new TimeSpan(18, 0, 0), 
                    EndTime = new TimeSpan(22, 0, 0), 
                    Location = "Main Hall", 
                    AdminApproval = true,
                    Event_Attendances = new List<Event_Attendance>() 
                },
                new Event 
                { 
                    EventId = 3, 
                    Title = "Workshop", 
                    Description = "Skill development workshop", 
                    EventDate = new DateTime(2023, 11, 5), 
                    StartTime = new TimeSpan(9, 0, 0), 
                    EndTime = new TimeSpan(12, 0, 0), 
                    Location = "Training Room", 
                    AdminApproval = true,
                    Event_Attendances = new List<Event_Attendance>() 
                }
            );
        }
    }
}
