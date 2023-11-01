using System;
using System.Collections.Generic;

namespace IMS_System.Models.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public string? Avatar { get; set; }

    public int? RoleId { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    public virtual Role? Role { get; set; }
}
