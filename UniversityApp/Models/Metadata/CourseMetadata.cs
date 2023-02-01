using System.ComponentModel.DataAnnotations;

namespace UniversityApp.Models.Metadata
{
    public class CourseMetadata
    {
        [Display(Name = "Course Title - Semester")]
        public int CourseId { get; set; }

        [Display(Name = "Professor FullName - Professor AFM")]
        public int? ProfessorId { get; set; }
    }
}
