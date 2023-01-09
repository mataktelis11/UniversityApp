using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UniversityApp.Models;

[Table("Course_has_students")]
[Index("CourseId", "StudentId", Name = "Course_has_students_Uq", IsUnique = true)]
public partial class CourseHasStudent
{
    [Key]
    public int GradeId { get; set; }

    public int? CourseId { get; set; }

    public int? StudentId { get; set; }

    public int? Grade { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("CourseHasStudents")]
    public virtual Course? Course { get; set; }

    [ForeignKey("StudentId")]
    [InverseProperty("CourseHasStudents")]
    public virtual Student? Student { get; set; }
}
