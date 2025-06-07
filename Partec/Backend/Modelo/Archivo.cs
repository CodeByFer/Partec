using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Partec.MVVM.Base;

namespace Partec.Backend.Modelo;

[Table("Archivos", Schema = "GestionIncidencias")]
[Index("IdIncidencia", Name = "id_incidencia")]
public partial class Archivo : PropertyChangedDataError
{
    [Key]
    [Column("id_archivo")]
    public int IdArchivo { get; set; }

    [Column("id_incidencia")]
    public int? IdIncidencia { get; set; }

    [Column("tipo_archivo", TypeName = "enum('grafico','resumen','informe','otro')")]
    public string? TipoArchivo { get; set; }

    [Column("ruta_relativa")]
    [StringLength(255)]
    public string RutaRelativa { get; set; } = null!;

    [ForeignKey("IdIncidencia")]
    [InverseProperty("Archivos")]
    public virtual Incidencia IdIncidenciaNavigation { get; set; } = null!;
}
