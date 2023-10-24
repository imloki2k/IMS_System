using System;
using System.Collections.Generic;

namespace IMS_System.Models.Entities;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string? SubjectName { get; set; }

    public string? SubjectCode { get; set; }

    public int? StatusId { get; set; }

    public string? Description { get; set; }

    public int? ClassId { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual Class? Class { get; set; }
}
