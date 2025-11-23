using System;
using System.Collections.Generic;

namespace APP_INTERBANK_SOA.Models;

public partial class RequisitoCuentaAhorro
{
    public int IdRequisito { get; set; }

    public int IdTipoCuenta { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual TipoCuentaAhorro IdTipoCuentaNavigation { get; set; } = null!;
}
