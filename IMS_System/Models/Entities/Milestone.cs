using System;
using System.Collections.Generic;

namespace IMS_System.Models.Entities;

public partial class Milestone
{
    public int MilestoneId { get; set; }

    public int? ClassId { get; set; }

    public int? ProjectId { get; set; }

    public int? IssueId { get; set; }

    public int? AssignmentId { get; set; }
    public int? SubjectId { get; set; }

    public DateTime? Milestone1 { get; set; }

    public string? MilestoneDescription { get; set; }

    public virtual Assignment? Assignment { get; set; }

    public virtual Class? Class { get; set; }

    public virtual Issue? Issue { get; set; }

    public virtual Project? Project { get; set; }

    public virtual Subject? Subject { get; set; }
}
