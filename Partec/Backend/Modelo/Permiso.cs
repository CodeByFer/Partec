﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Partec.MVVM.Base;

namespace Partec.Backend.Modelo;

[Table("Permisos", Schema = "GestionIncidencias")]
public partial class Permiso : PropertyChangedDataError
{
    [Key]
    [Column("id_permiso")]
    public int IdPermiso { get; set; }

    [Column("descripcion")]
    [StringLength(255)]
    public string Descripcion { get; set; } = null!;

    [Column("NombreControl")]
    [StringLength(100)]
    public string NombreControl { get; set; } = null!;

    [ForeignKey("IdPermiso")]
    [InverseProperty("IdPermisos")]
    public virtual ICollection<Rol> IdRols { get; set; } = new List<Rol>();

    [NotMapped]
    public bool Asignado
    {
        get => _asignado;
        set { _asignado = value; } // Usas PropertyChangedDataError
    }
    private bool _asignado;
}
