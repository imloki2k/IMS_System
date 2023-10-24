using System;
using System.Collections.Generic;

namespace IMS_System.Models.Entities;

public partial class Project
{
    public int ProjectId { get; set; }

    public string? EnglishName { get; set; }

    public string? VietnameseName { get; set; }

    public int? StatusId { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    public virtual Status? Status { get; set; }
}
