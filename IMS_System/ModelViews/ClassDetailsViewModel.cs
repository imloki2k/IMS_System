using IMS_System.Models.Entities;

namespace IMS_System.ModelViews
{
    public class ClassDetailsViewModel
    {
        public Class Class { get; set; }
        public IEnumerable<ClassStudent> Student { get; set; }
        public IEnumerable<Milestone> Milestone { get; set; }
    }
}
