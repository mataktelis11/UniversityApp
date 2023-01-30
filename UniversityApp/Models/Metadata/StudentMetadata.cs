using System.ComponentModel.DataAnnotations;

namespace UniversityApp.Models.Metadata
{
    public class StudentMetadata
    {
        [Display(Name = "Registration Number")]
        public int RegistrationNumber { get; set; }
    }
}
