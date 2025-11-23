using System;
using System.Collections.Generic;

namespace APP_INTERBANK_SOA.Models;

public partial class DepositoPlazo
{
    public int IdDeposito { get; set; }

    public decimal Monto { get; set; }

    public int PlazoDias { get; set; }

    public decimal TasaAnual { get; set; }

    public DateTime FechaApertura { get; set; }

    public string Estado { get; set; } = null!;

    public bool RenovacionAutomatica { get; set; }

    public int IdCuenta { get; set; }

    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;
}
