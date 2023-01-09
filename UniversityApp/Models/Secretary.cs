using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UniversityApp.Models;

[Index("Userid", Name = "UQ__Secretar__CBA1B25631CD262A", IsUnique = true)]
public partial class Secretary
{
    [Key]
    public int SecretaryId { get; set; }

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

    [ForeignKey("Userid")]
    [InverseProperty("Secretary")]
    public virtual User? User { get; set; }
}
