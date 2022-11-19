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
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<Teacher> Teachers { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=xisd.database.windows.net;Initial Catalog=XISD_POE;User ID=user;Password=,,PassnowSQL1;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Campus>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK__Campus__A25C5AA6994B5B5D");

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

                entity.Property(e => e.Comment).IsUnicode(false);

                entity.Property(e => e.Criteria)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.GradingNumberNavigation)
                    .WithMany(p => p.Commentaries)
                    .HasForeignKey(d => d.GradingNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Commentar__Gradi__6D6238AF");
            });

            modelBuilder.Entity<Execution>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Executio__78A1A19C4BF56572");

                entity.ToTable("Execution");

                entity.HasOne(d => d.GradingNumberNavigation)
                    .WithMany(p => p.Executions)
                    .HasForeignKey(d => d.GradingNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Execution__Gradi__67A95F59");
            });

            modelBuilder.Entity<Grading>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Gradings__78A1A19C2034FEB8");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Student)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Subject)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Teacher)
                    .HasMaxLength(6)
                    .IsUnicode(false);

                entity.Property(e => e.Topic)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.StudentNavigation)
                    .WithMany(p => p.Gradings)
                    .HasForeignKey(d => d.Student)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Gradings__Studen__61F08603");

                entity.HasOne(d => d.TeacherNavigation)
                    .WithMany(p => p.Gradings)
                    .HasForeignKey(d => d.Teacher)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Gradings__Teache__60FC61CA");
            });

            modelBuilder.Entity<Lecturer>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Lecturer__78A1A19CB81626F9");

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
                    .HasConstraintName("FK__Lecturers__Campu__5772F790");

                entity.HasOne(d => d.EmailNavigation)
                    .WithMany(p => p.Lecturers)
                    .HasForeignKey(d => d.Email)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Lecturers__Email__567ED357");
            });

            modelBuilder.Entity<Overall>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Overall__78A1A19C7330EDCF");

                entity.ToTable("Overall");

                entity.HasOne(d => d.GradingNumberNavigation)
                    .WithMany(p => p.Overalls)
                    .HasForeignKey(d => d.GradingNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Overall__Grading__6A85CC04");
            });

            modelBuilder.Entity<Planning>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Planning__78A1A19CB29DEDCD");

                entity.ToTable("Planning");

                entity.HasOne(d => d.GradingNumberNavigation)
                    .WithMany(p => p.Plannings)
                    .HasForeignKey(d => d.GradingNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Planning__Gradin__64CCF2AE");
            });

            modelBuilder.Entity<School>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK__Schools__A25C5AA6DB081373");

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
                    .HasConstraintName("FK__Schools__Campus__4FD1D5C8");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Students__78A1A19C70BAFF75");

                entity.Property(e => e.Number)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Campus)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName).IsUnicode(false);

                entity.Property(e => e.LastName).IsUnicode(false);

                entity.Property(e => e.Qualification)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.CampusNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.Campus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Students__Campus__5A4F643B");
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
                    .HasConstraintName("FK__StudentSc__Schoo__5E1FF51F");

                entity.HasOne(d => d.StudentNavigation)
                    .WithMany(p => p.StudentSchools)
                    .HasForeignKey(d => d.Student)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__StudentSc__Stude__5D2BD0E6");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AmountOfClasses).HasColumnName("amountOfClasses");

                entity.Property(e => e.Subject1)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("subject");

                entity.Property(e => e.YearOfStudy)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("yearOfStudy");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Teachers__78A1A19C1D7B53C3");

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
                    .HasConstraintName("FK__Teachers__Email__52AE4273");

                entity.HasOne(d => d.SchoolNavigation)
                    .WithMany(p => p.Teachers)
                    .HasForeignKey(d => d.School)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Teachers__School__53A266AC");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Email)
                    .HasName("PK__Users__A9D105351688E9F9");

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
