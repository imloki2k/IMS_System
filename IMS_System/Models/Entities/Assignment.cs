using System;
using System.Collections.Generic;

namespace IMS_System.Models.Entities;

public partial class Assignment
{
    public int AssignmentId { get; set; }

    public string? AssingmentName { get; set; }

    public int? MilestoneId { get; set; }

    public int? SubjectId { get; set; }

    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();

    public virtual Subject? Subject { get; set; }
}
