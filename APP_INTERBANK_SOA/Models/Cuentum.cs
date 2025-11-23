using System;
using System.Collections.Generic;

namespace APP_INTERBANK_SOA.Models;

public partial class Cuentum
{
    public int IdCuenta { get; set; }

    public string NumeroCuenta { get; set; } = null!;

    public string TipoCuenta { get; set; } = null!;

    public decimal SaldoDisponible { get; set; }

    public string Moneda { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaCierre { get; set; }

    public string? MotivoCierre { get; set; }

    public string Estado { get; set; } = null!;

    public int IdUsuario { get; set; }

    public int? IdCuentaPadre { get; set; }

    public virtual ICollection<AsociacionPaypal> AsociacionPaypals { get; set; } = new List<AsociacionPaypal>();

    public virtual ICollection<DepositoPlazo> DepositoPlazos { get; set; } = new List<DepositoPlazo>();

    public virtual Cuentum? IdCuentaPadreNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Cuentum> InverseIdCuentaPadreNavigation { get; set; } = new List<Cuentum>();

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<Transferencium> TransferenciumIdCuentaDestinoNavigations { get; set; } = new List<Transferencium>();

    public virtual ICollection<Transferencium> TransferenciumIdCuentaOrigenNavigations { get; set; } = new List<Transferencium>();
}
