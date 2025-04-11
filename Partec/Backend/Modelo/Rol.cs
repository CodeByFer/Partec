using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Partec.Backend.Modelo;

[Table("Roles", Schema = "GestionIncidencias")]
public partial class Rol
{
    [Key]
    [Column("id_rol")]
    public int IdRol { get; set; }

    [Column("nombre_rol")]
    [StringLength(50)]
    public string NombreRol { get; set; } = null!;

    [InverseProperty("IdRolNavigation")]
    public virtual ICollection<Profesor> Profesores { get; set; } = new List<Profesor>();

    [ForeignKey("IdRol")]
    [InverseProperty("IdRols")]
    public virtual ICollection<Permiso> IdPermisos { get; set; } = new List<Permiso>();
}
