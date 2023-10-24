using System;
using System.Collections.Generic;

namespace IMS_System.Models.Entities;

public partial class Semeter
{
    public int SemeterId { get; set; }

    public string? SemeterName { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
