using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Partec.Backend.Modelo;

[Table("Profesores", Schema = "GestionIncidencias")]
[Index("Dni", Name = "dni", IsUnique = true)]
[Index("Email", Name = "email", IsUnique = true)]
[Index("IdDepartamento", Name = "id_departamento")]
[Index("IdRol", Name = "id_rol")]
public partial class Profesor
{
    [Key]
    [Column("id_profesor")]
    public int IdProfesor { get; set; }

    [Column("dni")]
    [StringLength(15)]
    public string Dni { get; set; } = null!;

    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("apellidos")]
    [StringLength(100)]
    public string Apellidos { get; set; } = null!;

    [Column("id_departamento")]
    public int IdDepartamento { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("password")]
    [StringLength(255)]
    public string Password { get; set; } = null!;

    [Column("id_rol")]
    public int IdRol { get; set; }

    [ForeignKey("IdDepartamento")]
    [InverseProperty("Profesores")]
    public virtual Departamento IdDepartamentoNavigation { get; set; } = null!;

    [ForeignKey("IdRol")]
    [InverseProperty("Profesores")]
    public virtual Rol IdRolNavigation { get; set; } = null!;

    [InverseProperty("IdProfesorNavigation")]
    public virtual ICollection<Incidencia> IncidenciaIdProfesorNavigations { get; set; } = new List<Incidencia>();

    [InverseProperty("IdResponsableNavigation")]
    public virtual ICollection<Incidencia> IncidenciaIdResponsableNavigations { get; set; } = new List<Incidencia>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<LogIncidencia> LogIncidencia { get; set; } = new List<LogIncidencia>();
}
