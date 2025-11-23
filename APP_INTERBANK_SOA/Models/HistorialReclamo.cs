using System;
using System.Collections.Generic;

namespace APP_INTERBANK_SOA.Models;

public partial class HistorialReclamo
{
    public int IdHistorial { get; set; }

    public int IdReclamo { get; set; }

    public DateTime FechaCambio { get; set; }

    public string EstadoAnterior { get; set; } = null!;

    public string EstadoNuevo { get; set; } = null!;

    public string? Comentario { get; set; }

    public int? IdUsuarioModificador { get; set; }

    public virtual Reclamo IdReclamoNavigation { get; set; } = null!;

    public virtual Usuario? IdUsuarioModificadorNavigation { get; set; }
}
