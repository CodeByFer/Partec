using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Partec.Backend.Modelo;

public partial class GestionincidenciasContext : DbContext
{
    public GestionincidenciasContext()
    {
    }

    public GestionincidenciasContext(DbContextOptions<GestionincidenciasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Archivo> Archivos { get; set; }

    public virtual DbSet<Departamento> Departamentos { get; set; }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Incidencia> Incidencias { get; set; }

    public virtual DbSet<LogIncidencia> LogIncidencias { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Profesor> Profesores { get; set; }

    public virtual DbSet<Rol> Roles { get; set; }

    public virtual DbSet<TipoHardware> TipoHardwares { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseLazyLoadingProxies().UseMySQL("server=127.0.0.1;port=3306;database=gestionincidencias;user=root;password=mysql");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Archivo>(entity =>
        {
            entity.HasKey(e => e.IdArchivo).HasName("PRIMARY");

            entity.HasOne(d => d.IdIncidenciaNavigation).WithMany(p => p.Archivos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("archivos_ibfk_1");
        });

        modelBuilder.Entity<Departamento>(entity =>
        {
            entity.HasKey(e => e.IdDepartamento).HasName("PRIMARY");
        });

        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.IdEstado).HasName("PRIMARY");
        });

        modelBuilder.Entity<Incidencia>(entity =>
        {
            entity.HasKey(e => e.IdIncidencia).HasName("PRIMARY");

            entity.Property(e => e.FechaIntroduccion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.Incidencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("incidencias_ibfk_2");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Incidencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("incidencias_ibfk_4");

            entity.HasOne(d => d.IdProfesorNavigation).WithMany(p => p.IncidenciaIdProfesorNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("incidencias_ibfk_1");

            entity.HasOne(d => d.IdResponsableNavigation).WithMany(p => p.IncidenciaIdResponsableNavigations).HasConstraintName("incidencias_ibfk_5");

            entity.HasOne(d => d.IdTipoHwNavigation).WithMany(p => p.Incidencia).HasConstraintName("incidencias_ibfk_3");
        });

        modelBuilder.Entity<LogIncidencia>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("PRIMARY");

            entity.Property(e => e.FechaCambio).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.LogIncidencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("logincidencias_ibfk_2");

            entity.HasOne(d => d.IdIncidenciaNavigation).WithMany(p => p.LogIncidencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("logincidencias_ibfk_1");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.LogIncidencia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("logincidencias_ibfk_3");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("PRIMARY");
        });

        modelBuilder.Entity<Profesor>(entity =>
        {
            entity.HasKey(e => e.IdProfesor).HasName("PRIMARY");

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.Profesores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("profesores_ibfk_1");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Profesores)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("profesores_ibfk_2");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PRIMARY");

            entity.HasMany(d => d.IdPermisos).WithMany(p => p.IdRols)
                .UsingEntity<Dictionary<string, object>>(
                    "RolPermiso",
                    r => r.HasOne<Permiso>().WithMany()
                        .HasForeignKey("IdPermiso")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("rolpermisos_ibfk_2"),
                    l => l.HasOne<Rol>().WithMany()
                        .HasForeignKey("IdRol")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("rolpermisos_ibfk_1"),
                    j =>
                    {
                        j.HasKey("IdRol", "IdPermiso").HasName("PRIMARY");
                        j.ToTable("RolPermisos", "GestionIncidencias");
                        j.HasIndex(new[] { "IdPermiso" }, "id_permiso");
                        j.IndexerProperty<int>("IdRol").HasColumnName("id_rol");
                        j.IndexerProperty<int>("IdPermiso").HasColumnName("id_permiso");
                    });
        });

        modelBuilder.Entity<TipoHardware>(entity =>
        {
            entity.HasKey(e => e.IdTipoHw).HasName("PRIMARY");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
