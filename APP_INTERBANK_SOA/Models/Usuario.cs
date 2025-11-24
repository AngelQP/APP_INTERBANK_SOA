using System;
using System.Collections.Generic;

namespace APP_INTERBANK_SOA.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string ContrasenaHash { get; set; } = null!;

    public string TipoDocumento { get; set; } = null!;

    public string NumeroDocumento { get; set; } = null!;

    public string? Telefono { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public DateTime? UltimoLogin { get; set; }

    public int IntentosFallidos { get; set; }

    public DateTime? BloqueadoHasta { get; set; }

    public int IdRol { get; set; }

    public virtual ICollection<Beneficio> Beneficios { get; set; } = new List<Beneficio>();

    public virtual ICollection<Cuentum> Cuenta { get; set; } = new List<Cuentum>();

    public virtual ICollection<HistorialReclamo> HistorialReclamos { get; set; } = new List<HistorialReclamo>();

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<PreferenciaProducto> PreferenciaProductos { get; set; } = new List<PreferenciaProducto>();

    public virtual ICollection<ProductoFavorito> ProductoFavoritos { get; set; } = new List<ProductoFavorito>();

    public virtual ICollection<ProductoFinanciero> ProductoFinancieros { get; set; } = new List<ProductoFinanciero>();

    public virtual ICollection<Reclamo> Reclamos { get; set; } = new List<Reclamo>();
}
