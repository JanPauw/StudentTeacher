using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace StudentTeacher.Models
{
    public partial class XISD_POEContext : DbContext
    {
        public XISD_POEContext()
        {
        }

        public XISD_POEContext(DbContextOptions<XISD_POEContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Campus> Campuses { get; set; } = null!;
        public virtual DbSet<Commentary> Commentaries { get; set; } = null!;
        public virtual DbSet<Execution> Executions { get; set; } = null!;
        public virtual DbSet<Grading> Gradings { get; set; } = null!;
        public virtual DbSet<Lecturer> Lecturers { get; set; } = null!;
        public virtual DbSet<Overall> Overalls { get; set; } = null!;
        public virtual DbSet<Planning> Plannings { get; set; } = null!;
        public virtual DbSet<School> Schools { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<StudentSchool> StudentSchools { get; set; } = null!;
        public virtual DbSet<Teacher> Teachers { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=tcp:xisd.database.windows.net,1433;Initial Catalog=XISD_POE;Persist Security Info=False;User ID=user;Password=,,PassnowSQL1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Campus>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK__Campus__A25C5AA65752F7FC");

                entity.ToTable("Campus");

                entity.Property(e => e.Code)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Province)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Commentary>(entity =>
            {
                entity.ToTable("Commentary");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Criteria)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.GradingNumberNavigation)
                    .WithMany(p => p.Commentaries)
                    .HasForeignKey(d => d.GradingNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Commentar__Gradi__078C1F06");
            });

            modelBuilder.Entity<Execution>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Executio__78A1A19CEE9860BE");

                entity.ToTable("Execution");

                entity.HasOne(d => d.GradingNumberNavigation)
                    .WithMany(p => p.Executions)
                    .HasForeignKey(d => d.GradingNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Execution__Gradi__01D345B0");
            });

            modelBuilder.Entity<Grading>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Gradings__78A1A19CE0AE4B12");

                entity.Property(e => e.Student)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Teacher)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.HasOne(d => d.StudentNavigation)
                    .WithMany(p => p.Gradings)
                    .HasForeignKey(d => d.Student)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Gradings__Studen__793DFFAF");

                entity.HasOne(d => d.TeacherNavigation)
                    .WithMany(p => p.Gradings)
                    .HasForeignKey(d => d.Teacher)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Gradings__Teache__7849DB76");
            });

            modelBuilder.Entity<Lecturer>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Lecturer__78A1A19CE9EDC00E");

                entity.Property(e => e.Number)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Campus)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.HasOne(d => d.CampusNavigation)
                    .WithMany(p => p.Lecturers)
                    .HasForeignKey(d => d.Campus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Lecturers__Campu__6FB49575");

                entity.HasOne(d => d.EmailNavigation)
                    .WithMany(p => p.Lecturers)
                    .HasForeignKey(d => d.Email)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Lecturers__Email__6EC0713C");
            });

            modelBuilder.Entity<Overall>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Overall__78A1A19CB23B3F75");

                entity.ToTable("Overall");

                entity.HasOne(d => d.GradingNumberNavigation)
                    .WithMany(p => p.Overalls)
                    .HasForeignKey(d => d.GradingNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Overall__Grading__04AFB25B");
            });

            modelBuilder.Entity<Planning>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Planning__78A1A19C848452E0");

                entity.ToTable("Planning");

                entity.HasOne(d => d.GradingNumberNavigation)
                    .WithMany(p => p.Plannings)
                    .HasForeignKey(d => d.GradingNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Planning__Gradin__7EF6D905");
            });

            modelBuilder.Entity<School>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK__Schools__A25C5AA6A63B78BD");

                entity.Property(e => e.Code)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Campus)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Quintile)
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.HasOne(d => d.CampusNavigation)
                    .WithMany(p => p.Schools)
                    .HasForeignKey(d => d.Campus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Schools__Campus__681373AD");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Students__78A1A19CA60239E2");

                entity.Property(e => e.Number)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.Qualification)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<StudentSchool>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.School)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Student)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.SchoolNavigation)
                    .WithMany(p => p.StudentSchools)
                    .HasForeignKey(d => d.School)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StudentSc__Schoo__756D6ECB");

                entity.HasOne(d => d.StudentNavigation)
                    .WithMany(p => p.StudentSchools)
                    .HasForeignKey(d => d.Student)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StudentSc__Stude__74794A92");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Teachers__78A1A19CE16A286C");

                entity.Property(e => e.Number)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.School)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.HasOne(d => d.EmailNavigation)
                    .WithMany(p => p.Teachers)
                    .HasForeignKey(d => d.Email)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Teachers__Email__6AEFE058");

                entity.HasOne(d => d.SchoolNavigation)
                    .WithMany(p => p.Teachers)
                    .HasForeignKey(d => d.School)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Teachers__School__6BE40491");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Email)
                    .HasName("PK__Users__A9D10535A81E2A46");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Password).IsUnicode(false);

                entity.Property(e => e.Role)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Type).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
