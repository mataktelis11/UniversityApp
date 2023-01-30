using System.ComponentModel.DataAnnotations;

namespace UniversityApp.Models.Metadata
{
    public class ProfessorMetadata
    {
        [Display(Name = "FullName - AFM")]
        public int ProfessorId { get; set; }
    }
}
