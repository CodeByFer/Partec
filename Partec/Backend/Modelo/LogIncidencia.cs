using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Partec.Backend.Modelo;

[Table("LogIncidencias", Schema = "GestionIncidencias")]
[Index("IdEstado", Name = "id_estado")]
[Index("IdIncidencia", Name = "id_incidencia")]
[Index("IdUsuario", Name = "id_usuario")]
public partial class LogIncidencia
{
    [Key]
    [Column("id_log")]
    public int IdLog { get; set; }

    [Column("id_incidencia")]
    public int IdIncidencia { get; set; }

    [Column("fecha_cambio", TypeName = "datetime")]
    public DateTime FechaCambio { get; set; }

    [Column("id_estado")]
    public int IdEstado { get; set; }

    [Column("id_usuario")]
    public int IdUsuario { get; set; }

    [Column("comentario", TypeName = "text")]
    public string? Comentario { get; set; }

    [ForeignKey("IdEstado")]
    [InverseProperty("LogIncidencia")]
    public virtual Estado IdEstadoNavigation { get; set; } = null!;

    [ForeignKey("IdIncidencia")]
    [InverseProperty("LogIncidencia")]
    public virtual Incidencia IdIncidenciaNavigation { get; set; } = null!;

    [ForeignKey("IdUsuario")]
    [InverseProperty("LogIncidencia")]
    public virtual Profesor IdUsuarioNavigation { get; set; } = null!;
}
