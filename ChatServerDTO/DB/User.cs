using System;
using System.Collections.Generic;

namespace ChatServerDTO.DB;

public partial class User
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string Patronymic { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int IdRole { get; set; }

    public int IdDepartment { get; set; }

    public string Phone { get; set; } = null!;

    public DateTime? Birthday { get; set; }

    public string Post { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public virtual ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();

    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual Department IdDepartmentNavigation { get; set; } = null!;

    public virtual Role IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<TaskForUser> TaskForUsers { get; set; } = new List<TaskForUser>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
