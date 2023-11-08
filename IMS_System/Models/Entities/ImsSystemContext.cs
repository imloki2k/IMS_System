using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IMS_System.Models.Entities;

public partial class ImsSystemContext : DbContext
{
    public ImsSystemContext()
    {
    }

    public ImsSystemContext(DbContextOptions<ImsSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassStudent> ClassStudents { get; set; }

    public virtual DbSet<Issue> Issues { get; set; }

    public virtual DbSet<Milestone> Milestones { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectMember> ProjectMembers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Semeter> Semeters { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            optionsBuilder.UseSqlServer(config.GetConnectionString("dbIMSsystem"));
        }
    }
        
        //=> optionsBuilder.UseSqlServer("Data Source=DESKTOP-R36K6QN;Initial Catalog=IMS_System;User ID=sa;Password=123456;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("Assignment");

            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.AssingmentName)
                .HasMaxLength(300)
                .HasColumnName("assingment_name");
            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Subject).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_Assignment_Subjects");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.ClassName)
                .HasMaxLength(200)
                .HasColumnName("class_name");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("end_date");
            entity.Property(e => e.SemeterId).HasColumnName("semeter_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("start_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.HasOne(d => d.Semeter).WithMany(p => p.Classes)
                .HasForeignKey(d => d.SemeterId)
                .HasConstraintName("FK_Classes_Semeters");

            entity.HasOne(d => d.Status).WithMany(p => p.Classes)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Classes_Status");
        });

        modelBuilder.Entity<ClassStudent>(entity =>
        {
            entity.ToTable("Class_student");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassStudents)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_Class_student_Classes");

            entity.HasOne(d => d.Student).WithMany(p => p.ClassStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_Class_student_Users");
        });

        modelBuilder.Entity<Issue>(entity =>
        {
            entity.Property(e => e.IssueId)
                
                .HasColumnName("issue_id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.IssueDescription)
                .HasColumnType("text")
                .HasColumnName("issue_description");
            entity.Property(e => e.IssueName)
                .HasMaxLength(200)
                .HasColumnName("issue_name");
            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Status).WithMany(p => p.Issues)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Issues_Status");
        });

        modelBuilder.Entity<Milestone>(entity =>
        {
            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.IssueId).HasColumnName("issue_id");
            entity.Property(e => e.Milestone1)
                .HasColumnType("datetime")
                .HasColumnName("milestone");
            entity.Property(e => e.MilestoneDescription)
                .HasColumnType("text")
                .HasColumnName("milestone_description");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Assignment).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.AssignmentId)
                .HasConstraintName("FK_Milestones_Assignment");

            entity.HasOne(d => d.Class).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_Milestones_Classes");

            entity.HasOne(d => d.Issue).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.IssueId)
                .HasConstraintName("FK_Milestones_Issues");

            entity.HasOne(d => d.Project).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_Milestones_Projects");

            entity.HasOne(d => d.Subject).WithMany(p => p.Milestones)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_Milestones_Subjects");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.EnglishName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("english_name");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.VietnameseName)
                .HasMaxLength(200)
                .HasColumnName("vietnamese_name");

            entity.HasOne(d => d.Status).WithMany(p => p.Projects)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Projects_Status");
        });

        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.ToTable("Project_member");

            entity.Property(e => e.Id)
               
                .HasColumnName("id");
            entity.Property(e => e.IsLeader).HasColumnName("is_leader");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_Project_member_Projects");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Project_member_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(150)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Semeter>(entity =>
        {
            entity.Property(e => e.SemeterId).HasColumnName("semeter_id");
            entity.Property(e => e.SemeterName)
                .HasMaxLength(150)
                .HasColumnName("semeter_name");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("status_id");
            entity.Property(e => e.StatusName)
                .HasMaxLength(200)
                .HasColumnName("status_name");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.SubjectCode)
                .HasMaxLength(200)
                .HasColumnName("subject_code");
            entity.Property(e => e.SubjectName)
                .HasMaxLength(200)
                .HasColumnName("subject_name");

            entity.HasOne(d => d.Class).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK_Subjects_Classes");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Avatar)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsFixedLength()
                .HasColumnName("email");
            entity.Property(e => e.Mobile).HasColumnName("mobile");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
