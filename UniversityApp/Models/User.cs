using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UniversityApp.Models;

[Index("Username", Name = "UQ__Users__536C85E4CAFE4A72", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("userid")]
    public int Userid { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [StringLength(70)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string Role { get; set; } = null!;

    [InverseProperty("User")]
    public virtual Professor? Professor { get; set; }

    [InverseProperty("User")]
    public virtual Secretary? Secretary { get; set; }

    [InverseProperty("User")]
    public virtual Student? Student { get; set; }
}
