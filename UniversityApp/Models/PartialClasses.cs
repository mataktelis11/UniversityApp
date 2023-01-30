using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UniversityApp.Models.Metadata;

namespace UniversityApp.Models
{
    [ModelMetadataType(typeof(StudentMetadata))]
    public partial class Student
    {

    }

    [ModelMetadataType(typeof(ProfessorMetadata))]
    public partial class Professor
    {
        public string Fullname
        {
            get
            {
                return Name + " " + Surname; 
            }
        }

        public string FullnameAFM
        {
            get
            {
                return Name + " " + Surname + " - " + Afm;
            }
        }
    }

    [ModelMetadataType(typeof(CourseMetadata))]
    public partial class Course
    {
        public string TitleSemester
        {
            get
            {
                return Title + " - " + Semester;
            }
        }
    }
}
