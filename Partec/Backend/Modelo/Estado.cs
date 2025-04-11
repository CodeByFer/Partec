using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Partec.Backend.Modelo;

[Table("Estados", Schema = "GestionIncidencias")]
public partial class Estado
{
    [Key]
    [Column("id_estado")]
    public int IdEstado { get; set; }

    [Column("descripcion_estado")]
    [StringLength(50)]
    public string DescripcionEstado { get; set; } = null!;

    [InverseProperty("IdEstadoNavigation")]
    public virtual ICollection<Incidencia> Incidencia { get; set; } = new List<Incidencia>();

    [InverseProperty("IdEstadoNavigation")]
    public virtual ICollection<LogIncidencia> LogIncidencia { get; set; } = new List<LogIncidencia>();
}
