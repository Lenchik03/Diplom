using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ProjectSystemAPI.DB;

public partial class ProjectSystemNewContext : DbContext
{
    public ProjectSystemNewContext()
    {
    }

    public ProjectSystemNewContext(DbContextOptions<ProjectSystemNewContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskForUser> TaskForUsers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=192.168.200.13;user=student;password=student;database=ProjectSystemNew", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.3.39-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.IdMainDep, "FK_Departments_Departments_Id");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.IdDirector)
                .HasColumnType("int(11)")
                .HasColumnName("Id_director");
            entity.Property(e => e.IdMainDep)
                .HasColumnType("int(11)")
                .HasColumnName("Id_main_dep");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasDefaultValueSql("''");

            entity.HasOne(d => d.IdMainDepNavigation).WithMany(p => p.InverseIdMainDepNavigation)
                .HasForeignKey(d => d.IdMainDep)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Departments_Departments_Id");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.CompletionDate)
                .HasColumnType("datetime")
                .HasColumnName("Completion_date");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IdCreator)
                .HasColumnType("int(11)")
                .HasColumnName("Id_creator");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_date");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasDefaultValueSql("''");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasDefaultValueSql("''");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasDefaultValueSql("''");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.IdProject, "FK_Tasks_Projects_Id");

            entity.HasIndex(e => e.IdStatus, "FK_Tasks_Statuses_Id");

            entity.HasIndex(e => e.IdExecutor, "FK_Tasks_Users_Id");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IdCreator)
                .HasColumnType("int(11)")
                .HasColumnName("Id_creator");
            entity.Property(e => e.IdExecutor)
                .HasColumnType("int(11)")
                .HasColumnName("Id_executor");
            entity.Property(e => e.IdProject)
                .HasColumnType("int(11)")
                .HasColumnName("Id_project");
            entity.Property(e => e.IdStatus)
                .HasColumnType("int(11)")
                .HasColumnName("Id_status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasDefaultValueSql("''");

            entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.IdProject)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tasks_Projects_Id");

            entity.HasOne(d => d.IdStatusNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.IdStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tasks_Statuses_Id");
        });

        modelBuilder.Entity<TaskForUser>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => e.IdTask, "FK_TaskForUsers_Tasks_Id");

            entity.HasIndex(e => e.IdUser, "FK_TaskForUsers_Users_Id");

            entity.Property(e => e.IdTask)
                .HasColumnType("int(11)")
                .HasColumnName("Id_task");
            entity.Property(e => e.IdUser)
                .HasColumnType("int(11)")
                .HasColumnName("Id_user");

            entity.HasOne(d => d.IdTaskNavigation).WithMany()
                .HasForeignKey(d => d.IdTask)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskForUsers_Tasks_Id");

            entity.HasOne(d => d.IdUserNavigation).WithMany()
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskForUsers_Users_Id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.IdDepartment, "FK_Users_Departments_Id");

            entity.HasIndex(e => e.IdRole, "FK_Users_Roles_Id");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasDefaultValueSql("''");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasDefaultValueSql("''");
            entity.Property(e => e.IdDepartment)
                .HasColumnType("int(11)")
                .HasColumnName("Id_department");
            entity.Property(e => e.IdRole)
                .HasColumnType("int(11)")
                .HasColumnName("Id_role");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(255)
                .HasDefaultValueSql("''");
            entity.Property(e => e.Phone)
                .HasMaxLength(255)
                .HasDefaultValueSql("''")
                .HasColumnName("phone");

            entity.HasOne(d => d.IdDepartmentNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdDepartment)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Departments_Id");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles_Id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
