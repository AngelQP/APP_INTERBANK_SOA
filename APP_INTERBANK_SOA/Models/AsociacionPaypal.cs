using System;
using System.Collections.Generic;

namespace APP_INTERBANK_SOA.Models;

public partial class AsociacionPaypal
{
    public int IdAsociacion { get; set; }

    public string EmailPaypal { get; set; } = null!;

    public DateTime FechaAsociacion { get; set; }

    public string Estado { get; set; } = null!;

    public int IdCuenta { get; set; }

    public virtual Cuentum IdCuentaNavigation { get; set; } = null!;
}
