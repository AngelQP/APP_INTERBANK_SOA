using System;
using System.Collections.Generic;

namespace APP_INTERBANK_SOA.Models;

public partial class Movimiento
{
    public int IdMovimiento { get; set; }

    public DateTime Fecha { get; set; }

    public string Tipo { get; set; } = null!;

    public decimal Monto { get; set; }

    public string? Descripcion { get; set; }

    public int IdCuenta { get; set; }

    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;
}
