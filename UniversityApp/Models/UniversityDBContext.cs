using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UniversityApp.Models;

public partial class UniversityDBContext : DbContext
{
    public UniversityDBContext()
    {
    }

    public UniversityDBContext(DbContextOptions<UniversityDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseHasStudent> CourseHasStudents { get; set; }

    public virtual DbSet<Professor> Professors { get; set; }

    public virtual DbSet<Secretary> Secretaries { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-IAVAU55\\SQLEXPRESS;Database=universitydb;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D71A732810F14");

            entity.Property(e => e.CourseId).ValueGeneratedNever();

            entity.HasOne(d => d.Professor).WithMany(p => p.Courses).HasConstraintName("FK_Courses_Professors");
        });

        modelBuilder.Entity<CourseHasStudent>(entity =>
        {
            entity.HasKey(e => e.GradeId).HasName("PK__Course_h__54F87A5753200034");

            entity.Property(e => e.GradeId).ValueGeneratedNever();

            entity.HasOne(d => d.Course).WithMany(p => p.CourseHasStudents).HasConstraintName("FK_Courses_Grade");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseHasStudents).HasConstraintName("FK_Students_Grade");
        });

        modelBuilder.Entity<Professor>(entity =>
        {
            entity.HasKey(e => e.ProfessorId).HasName("PK__Professo__9003594983A69331");

            entity.Property(e => e.ProfessorId).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithOne(p => p.Professor).HasConstraintName("FK_Professors_Users");
        });

        modelBuilder.Entity<Secretary>(entity =>
        {
            entity.HasKey(e => e.SecretaryId).HasName("PK__Secretar__5359F01393E53683");

            entity.Property(e => e.SecretaryId).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithOne(p => p.Secretary).HasConstraintName("FK_Secretariess_Users");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__4D11D63CD01599A4");

            entity.Property(e => e.StudentId).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithOne(p => p.Student).HasConstraintName("FK_Students_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("PK__Users__CBA1B25732EAE073");

            entity.Property(e => e.Userid).ValueGeneratedNever();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
