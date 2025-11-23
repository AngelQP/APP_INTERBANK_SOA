using System;
using System.Collections.Generic;

namespace APP_INTERBANK_SOA.Models;

public partial class PreferenciaProducto
{
    public int IdPreferencia { get; set; }

    public int IdUsuario { get; set; }

    public string Categoria { get; set; } = null!;

    public string Valor { get; set; } = null!;

    public DateTime FechaActualizacion { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
