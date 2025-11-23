using System;
using System.Collections.Generic;

namespace APP_INTERBANK_SOA.Models;

public partial class ProductoFinanciero
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public decimal? TasaInteres { get; set; }

    public int? Plazo { get; set; }

    public string Estado { get; set; } = null!;

    public int IdUsuario { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<ProductoFavorito> ProductoFavoritos { get; set; } = new List<ProductoFavorito>();
}
