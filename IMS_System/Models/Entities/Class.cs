using System;
using System.Collections.Generic;

namespace IMS_System.Models.Entities;

public partial class Class
{
    public int ClassId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? ClassName { get; set; }

    public int? StatusId { get; set; }

    public int? SemeterId { get; set; }

    public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();

    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();

    public virtual Semeter? Semeter { get; set; }

    public virtual Status? Status { get; set; }

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
