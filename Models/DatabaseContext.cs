using Microsoft.EntityFrameworkCore;
using StarterKit.Utils;

namespace StarterKit.Models
{
    public class DatabaseContext : DbContext
    {
        // The admin table will be used in both cases
        public DbSet<Admin> Admins { get; set; }

        // You can comment out or remove the case you are not going to use.

        // Tables for the Theatre ticket case

        // public DbSet<Customer> Customer { get; set; }
        // public DbSet<Reservation> Reservation { get; set; }
        // public DbSet<TheatreShowDate> TheatreShowDate { get; set; }
        // public DbSet<TheatreShow> TheatreShow { get; set; }
        // public DbSet<Venue> Venue { get; set; }

        // Tables for the event calendar case

        public DbSet<User> Users { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Event_Attendance> EventAttendances { get; set; }
        public DbSet<Event> Events { get; set; }
        //public DbSet<Attendance> OfficeAttendances { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Existing admin configuration
            modelBuilder.Entity<Admin>()
                .HasIndex(p => p.UserName)
                .IsUnique();

            // Add seed data for admin
            modelBuilder.Entity<Admin>().HasData(
                new Admin { AdminId = 1, Email = "admin1@example.com", UserName = "admin1", Password = EncryptionHelper.HashPassword("password") }
                // ... other admin seeds
            );

            // Add configurations for other entities
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Event>()
                .Property(e => e.AdminApproval)
                .HasDefaultValue(false);

            modelBuilder.Entity<Event_Attendance>()
                .HasOne(ea => ea.Event)
                .WithMany(e => e.EventAttendances)
                .HasForeignKey(ea => ea.EventId);

            modelBuilder.Entity<Event_Attendance>()
                .HasOne(ea => ea.User)
                .WithMany(u => u.EventAttendances)
                .HasForeignKey(ea => ea.UserId);
        }

    }

}