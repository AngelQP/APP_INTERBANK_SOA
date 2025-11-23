using System;
using System.Collections.Generic;

namespace APP_INTERBANK_SOA.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public string TipoPago { get; set; } = null!;

    public decimal Monto { get; set; }

    public DateTime Fecha { get; set; }

    public string Estado { get; set; } = null!;

    public int IdCuenta { get; set; }

    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;
}
