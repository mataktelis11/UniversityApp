using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UniversityApp.Models;

[Index("Afm", Name = "UQ__Professo__C6906E62A6B81A85", IsUnique = true)]
[Index("Userid", Name = "UQ__Professo__CBA1B256BB592E42", IsUnique = true)]
public partial class Professor
{
    [Key]
    public int ProfessorId { get; set; }

    [Column("AFM")]
    public int Afm { get; set; }

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

    [InverseProperty("Professor")]
    public virtual ICollection<Course> Courses { get; } = new List<Course>();

    [ForeignKey("Userid")]
    [InverseProperty("Professor")]
    public virtual User? User { get; set; }
}
