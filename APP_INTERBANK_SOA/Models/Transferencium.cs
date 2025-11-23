using System;
using System.Collections.Generic;

namespace APP_INTERBANK_SOA.Models;

public partial class Transferencium
{
    public int IdTransferencia { get; set; }

    public decimal Monto { get; set; }

    public DateTime Fecha { get; set; }

    public string TipoTransferencia { get; set; } = null!;

    public int IdCuentaOrigen { get; set; }

    public int? IdCuentaDestino { get; set; }

    public string? CuentaDestinoExterna { get; set; }

    public string? BancoDestino { get; set; }

    public string? NombreDestinatario { get; set; }

    public string? Concepto { get; set; }

    public decimal Comision { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Cuentum? IdCuentaDestinoNavigation { get; set; }

    public virtual Cuentum IdCuentaOrigenNavigation { get; set; } = null!;
}
