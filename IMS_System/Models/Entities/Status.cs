using System;
using System.Collections.Generic;

namespace IMS_System.Models.Entities;

public partial class Status
{
    public int StatusId { get; set; }

    public string? StatusName { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Issue> Issues { get; set; } = new List<Issue>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
