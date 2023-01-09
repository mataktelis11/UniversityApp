using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UniversityApp.Models;

public partial class Course
{
    [Key]
    public int CourseId { get; set; }

    [StringLength(60)]
    [Unicode(false)]
    public string Title { get; set; } = null!;

    public int Semester { get; set; }

    public int? ProfessorId { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<CourseHasStudent> CourseHasStudents { get; } = new List<CourseHasStudent>();

    [ForeignKey("ProfessorId")]
    [InverseProperty("Courses")]
    public virtual Professor? Professor { get; set; }
}
