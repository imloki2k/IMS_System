using System;
using System.Collections.Generic;

namespace IMS_System.Models.Entities;

public partial class ClassStudent
{
    public int Id { get; set; }

    public int? ClassId { get; set; }

    public int? StudentId { get; set; }

    public virtual Class? Class { get; set; }

    public virtual User? Student { get; set; }
}
