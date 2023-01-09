using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UniversityApp.Models;

[Index("RegistrationNumber", Name = "UQ__Students__69FF2C9CCBADABAA", IsUnique = true)]
[Index("Userid", Name = "UQ__Students__CBA1B256A0E713AC", IsUnique = true)]
public partial class Student
{
    [Key]
    [Column("studentId")]
    public int StudentId { get; set; }

    [Column("Registration_number")]
    public int RegistrationNumber { get; set; }

    [StringLength(45)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(45)]
    [Unicode(false)]
    public string Surname { get; set; } = null!;

    [StringLength(45)]
    [Unicode(false)]
    public string Department { get; set; } = null!;

    [Column("userid")]
    public int? Userid { get; set; }

    [InverseProperty("Student")]
    public virtual ICollection<CourseHasStudent> CourseHasStudents { get; } = new List<CourseHasStudent>();

    [ForeignKey("Userid")]
    [InverseProperty("Student")]
    public virtual User? User { get; set; }
}
