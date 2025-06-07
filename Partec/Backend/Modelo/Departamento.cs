using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Partec.MVVM.Base;

namespace Partec.Backend.Modelo;

[Table("Departamentos", Schema = "GestionIncidencias")]
public partial class Departamento : PropertyChangedDataError
{
    [Key]
    [Column("id_departamento")]
    public int IdDepartamento { get; set; }

    [Column("nombre")]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("IdDepartamentoNavigation")]
    public virtual ICollection<Incidencia> Incidencia { get; set; } = new List<Incidencia>();

    [InverseProperty("IdDepartamentoNavigation")]
    public virtual ICollection<Profesor> Profesores { get; set; } = new List<Profesor>();
}
