using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Partec.Backend.Modelo;

[Table("TipoHardware", Schema = "GestionIncidencias")]
public partial class TipoHardware
{
    [Key]
    [Column("id_tipo_hw")]
    public int IdTipoHw { get; set; }

    [Column("descripcion")]
    [StringLength(50)]
    public string Descripcion { get; set; } = null!;

    [InverseProperty("IdTipoHwNavigation")]
    public virtual ICollection<Incidencia> Incidencia { get; set; } = new List<Incidencia>();
}
