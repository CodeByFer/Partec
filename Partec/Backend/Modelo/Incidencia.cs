using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Partec.Backend.Modelo;

[Table("Incidencias", Schema = "GestionIncidencias")]
[Index("IdDepartamento", Name = "id_departamento")]
[Index("IdEstado", Name = "id_estado")]
[Index("IdProfesor", Name = "id_profesor")]
[Index("IdResponsable", Name = "id_responsable")]
[Index("IdTipoHw", Name = "id_tipo_hw")]
public partial class Incidencia
{
    [Key]
    [Column("id_incidencia")]
    public int IdIncidencia { get; set; }

    [Column("tipo_incidencia", TypeName = "enum('HW','SW')")]
    public string TipoIncidencia { get; set; } = null!;

    [Column("fecha_incidencia", TypeName = "datetime")]
    public DateTime FechaIncidencia { get; set; }

    [Column("fecha_introduccion", TypeName = "datetime")]
    public DateTime FechaIntroduccion { get; set; }

    [Column("id_profesor")]
    public int IdProfesor { get; set; }

    [Column("id_departamento")]
    public int IdDepartamento { get; set; }

    [Column("ubicacion")]
    [StringLength(100)]
    public string Ubicacion { get; set; } = null!;

    [Column("id_tipo_hw")]
    public int? IdTipoHw { get; set; }

    [Column("modelo")]
    [StringLength(50)]
    public string? Modelo { get; set; }

    [Column("numero_serie")]
    [StringLength(100)]
    public string? NumeroSerie { get; set; }

    [Column("descripcion", TypeName = "text")]
    public string Descripcion { get; set; } = null!;

    [Column("observaciones", TypeName = "text")]
    public string? Observaciones { get; set; }

    [Column("id_estado")]
    public int IdEstado { get; set; }

    [Column("id_responsable")]
    public int? IdResponsable { get; set; }

    [Column("fecha_resolucion", TypeName = "datetime")]
    public DateTime? FechaResolucion { get; set; }

    [Column("tiempo_invertido")]
    public int? TiempoInvertido { get; set; }

    [InverseProperty("IdIncidenciaNavigation")]
    public virtual ICollection<Archivo> Archivos { get; set; } = new List<Archivo>();

    [ForeignKey("IdDepartamento")]
    [InverseProperty("Incidencia")]
    public virtual Departamento IdDepartamentoNavigation { get; set; } = null!;

    [ForeignKey("IdEstado")]
    [InverseProperty("Incidencia")]
    public virtual Estado IdEstadoNavigation { get; set; } = null!;

    [ForeignKey("IdProfesor")]
    [InverseProperty("IncidenciaIdProfesorNavigations")]
    public virtual Profesor IdProfesorNavigation { get; set; } = null!;

    [ForeignKey("IdResponsable")]
    [InverseProperty("IncidenciaIdResponsableNavigations")]
    public virtual Profesor? IdResponsableNavigation { get; set; }

    [ForeignKey("IdTipoHw")]
    [InverseProperty("Incidencia")]
    public virtual TipoHardware? IdTipoHwNavigation { get; set; }

    [InverseProperty("IdIncidenciaNavigation")]
    public virtual ICollection<LogIncidencia> LogIncidencia { get; set; } = new List<LogIncidencia>();
}
