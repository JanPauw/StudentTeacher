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
        public virtual DbSet<Lecturer> Lecturers { get; set; } = null!;
        public virtual DbSet<School> Schools { get; set; } = null!;
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
                    .HasName("PK__Campus__A25C5AA66A1FBF1D");

                entity.ToTable("Campus");

                entity.Property(e => e.Code)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Province)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Lecturer>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Lecturer__78A1A19C929224BA");

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
                    .HasConstraintName("FK__Lecturers__Campu__245D67DE");

                entity.HasOne(d => d.EmailNavigation)
                    .WithMany(p => p.Lecturers)
                    .HasForeignKey(d => d.Email)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Lecturers__Email__236943A5");
            });

            modelBuilder.Entity<School>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("PK__Schools__A25C5AA61164245E");

                entity.Property(e => e.Code)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.Campus)
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

                entity.HasOne(d => d.CampusNavigation)
                    .WithMany(p => p.Schools)
                    .HasForeignKey(d => d.Campus)
                    .HasConstraintName("FK__Schools__Campus__2739D489");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(e => e.Number)
                    .HasName("PK__Teachers__78A1A19CB330B6BF");

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
                    .HasConstraintName("FK__Teachers__Email__2A164134");

                entity.HasOne(d => d.SchoolNavigation)
                    .WithMany(p => p.Teachers)
                    .HasForeignKey(d => d.School)
                    .HasConstraintName("FK__Teachers__School__2B0A656D");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Email)
                    .HasName("PK__Users__A9D105354C0C3C5B");

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
