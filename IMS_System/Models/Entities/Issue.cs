using System;
using System.Collections.Generic;

namespace IMS_System.Models.Entities;

public partial class Issue
{
    public int IssueId { get; set; }

    public string? IssueName { get; set; }

    public int? ProjectId { get; set; }

    public int? MilestoneId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? IssueDescription { get; set; }

    public int? StatusId { get; set; }

    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();

    public virtual Status? Status { get; set; }
}
