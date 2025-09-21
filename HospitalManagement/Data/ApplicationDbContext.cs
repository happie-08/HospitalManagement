using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<ReferenceDoctor> ReferenceDoctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Master> Masters { get; set; }
        public DbSet<OPD> OPDs { get; set; }
        public DbSet<Department> Departments { get; set; }

        // ✅ New for many-to-many
        public DbSet<OPDDiagnosis> OPDDiagnoses { get; set; }
        public DbSet<OPDSymptom> OPDSymptoms { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ OPDDiagnosis mapping
            modelBuilder.Entity<OPDDiagnosis>()
                .HasKey(x => new { x.OPDId, x.DiagnosisId });

            modelBuilder.Entity<OPDDiagnosis>()
                .HasOne(x => x.OPD)
                .WithMany(o => o.OPDDiagnoses)
                .HasForeignKey(x => x.OPDId);

            modelBuilder.Entity<OPDDiagnosis>()
                .HasOne(x => x.Diagnosis)
                .WithMany()
                .HasForeignKey(x => x.DiagnosisId);

            // ✅ OPDSymptom mapping
            modelBuilder.Entity<OPDSymptom>()
                .HasKey(x => new { x.OPDId, x.SymptomId });

            modelBuilder.Entity<OPDSymptom>()
                .HasOne(x => x.OPD)
                .WithMany(o => o.OPDSymptoms)
                .HasForeignKey(x => x.OPDId);

            modelBuilder.Entity<OPDSymptom>()
                .HasOne(x => x.Symptom)
                .WithMany()
                .HasForeignKey(x => x.SymptomId);
        }
    }
}
