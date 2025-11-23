using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APP_INTERBANK_SOA.Models;

public partial class InterbankContext : DbContext
{
    public InterbankContext()
    {
    }

    public InterbankContext(DbContextOptions<InterbankContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AsociacionPaypal> AsociacionPaypals { get; set; }

    public virtual DbSet<Beneficio> Beneficios { get; set; }

    public virtual DbSet<Cuentum> Cuenta { get; set; }

    public virtual DbSet<DepositoPlazo> DepositoPlazos { get; set; }

    public virtual DbSet<HistorialReclamo> HistorialReclamos { get; set; }

    public virtual DbSet<Movimiento> Movimientos { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<PreferenciaProducto> PreferenciaProductos { get; set; }

    public virtual DbSet<ProductoFavorito> ProductoFavoritos { get; set; }

    public virtual DbSet<ProductoFinanciero> ProductoFinancieros { get; set; }

    public virtual DbSet<Reclamo> Reclamos { get; set; }

    public virtual DbSet<RequisitoCuentaAhorro> RequisitoCuentaAhorros { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<TipoCuentaAhorro> TipoCuentaAhorros { get; set; }

    public virtual DbSet<Transferencium> Transferencia { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:interbank.database.windows.net,1433;Database=INTERBANK;User ID=Admin123;Password=123Admin;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AsociacionPaypal>(entity =>
        {
            entity.HasKey(e => e.IdAsociacion);

            entity.ToTable("AsociacionPaypal");

            entity.Property(e => e.IdAsociacion).HasColumnName("idAsociacion");
            entity.Property(e => e.EmailPaypal)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("emailPaypal");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaAsociacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaAsociacion");
            entity.Property(e => e.IdCuenta).HasColumnName("idCuenta");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.AsociacionPaypals)
                .HasForeignKey(d => d.IdCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AsociacionPaypal_Cuenta");
        });

        modelBuilder.Entity<Beneficio>(entity =>
        {
            entity.HasKey(e => e.IdBeneficio);

            entity.ToTable("Beneficio");

            entity.Property(e => e.IdBeneficio).HasColumnName("idBeneficio");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Puntos)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("puntos");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Beneficios)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Beneficio_Usuario");
        });

        modelBuilder.Entity<Cuentum>(entity =>
        {
            entity.HasKey(e => e.IdCuenta);

            entity.HasIndex(e => e.IdCuentaPadre, "IX_Cuenta_CuentaPadre").HasFilter("([idCuentaPadre] IS NOT NULL)");

            entity.HasIndex(e => e.NumeroCuenta, "UK_Cuenta_Numero").IsUnique();

            entity.Property(e => e.IdCuenta).HasColumnName("idCuenta");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCierre)
                .HasColumnType("datetime")
                .HasColumnName("fechaCierre");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.IdCuentaPadre).HasColumnName("idCuentaPadre");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Moneda)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("moneda");
            entity.Property(e => e.MotivoCierre)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("motivoCierre");
            entity.Property(e => e.NumeroCuenta)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("numeroCuenta");
            entity.Property(e => e.SaldoDisponible)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("saldoDisponible");
            entity.Property(e => e.TipoCuenta)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipoCuenta");

            entity.HasOne(d => d.IdCuentaPadreNavigation).WithMany(p => p.InverseIdCuentaPadreNavigation)
                .HasForeignKey(d => d.IdCuentaPadre)
                .HasConstraintName("FK_Cuenta_CuentaPadre");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Cuenta)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Usuario");
        });

        modelBuilder.Entity<DepositoPlazo>(entity =>
        {
            entity.HasKey(e => e.IdDeposito);

            entity.ToTable("DepositoPlazo");

            entity.HasIndex(e => e.IdCuenta, "IX_DepositoPlazo_Cuenta");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FechaApertura)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RenovacionAutomatica).HasDefaultValue(true);
            entity.Property(e => e.TasaAnual).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.DepositoPlazos)
                .HasForeignKey(d => d.IdCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DepositoPlazo_Cuenta");
        });

        modelBuilder.Entity<HistorialReclamo>(entity =>
        {
            entity.HasKey(e => e.IdHistorial);

            entity.ToTable("HistorialReclamo");

            entity.HasIndex(e => new { e.IdReclamo, e.FechaCambio }, "IX_HistorialReclamo_Reclamo").IsDescending(false, true);

            entity.Property(e => e.IdHistorial).HasColumnName("idHistorial");
            entity.Property(e => e.Comentario)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("comentario");
            entity.Property(e => e.EstadoAnterior)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estadoAnterior");
            entity.Property(e => e.EstadoNuevo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estadoNuevo");
            entity.Property(e => e.FechaCambio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCambio");
            entity.Property(e => e.IdReclamo).HasColumnName("idReclamo");
            entity.Property(e => e.IdUsuarioModificador).HasColumnName("idUsuarioModificador");

            entity.HasOne(d => d.IdReclamoNavigation).WithMany(p => p.HistorialReclamos)
                .HasForeignKey(d => d.IdReclamo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistorialReclamo_Reclamo");

            entity.HasOne(d => d.IdUsuarioModificadorNavigation).WithMany(p => p.HistorialReclamos)
                .HasForeignKey(d => d.IdUsuarioModificador)
                .HasConstraintName("FK_HistorialReclamo_Usuario");
        });

        modelBuilder.Entity<Movimiento>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento);

            entity.ToTable("Movimiento");

            entity.Property(e => e.IdMovimiento).HasColumnName("idMovimiento");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdCuenta).HasColumnName("idCuenta");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.Movimientos)
                .HasForeignKey(d => d.IdCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Movimiento_Cuenta");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago);

            entity.ToTable("Pago");

            entity.Property(e => e.IdPago).HasColumnName("idPago");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdCuenta).HasColumnName("idCuenta");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.TipoPago)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipoPago");

            entity.HasOne(d => d.IdCuentaNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pago_Cuenta");
        });

        modelBuilder.Entity<PreferenciaProducto>(entity =>
        {
            entity.HasKey(e => e.IdPreferencia);

            entity.ToTable("PreferenciaProducto");

            entity.HasIndex(e => e.IdUsuario, "IX_PreferenciaProducto_Usuario");

            entity.Property(e => e.IdPreferencia).HasColumnName("idPreferencia");
            entity.Property(e => e.Categoria)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("categoria");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaActualizacion");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Valor)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("valor");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.PreferenciaProductos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PreferenciaProducto_Usuario");
        });

        modelBuilder.Entity<ProductoFavorito>(entity =>
        {
            entity.HasKey(e => e.IdFavorito);

            entity.ToTable("ProductoFavorito");

            entity.HasIndex(e => e.IdUsuario, "IX_ProductoFavorito_Usuario");

            entity.HasIndex(e => new { e.IdUsuario, e.IdProducto }, "UK_ProductoFavorito").IsUnique();

            entity.Property(e => e.IdFavorito).HasColumnName("idFavorito");
            entity.Property(e => e.FechaAgregado)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaAgregado");
            entity.Property(e => e.IdProducto).HasColumnName("idProducto");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductoFavoritos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductoFavorito_Producto");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ProductoFavoritos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductoFavorito_Usuario");
        });

        modelBuilder.Entity<ProductoFinanciero>(entity =>
        {
            entity.HasKey(e => e.IdProducto);

            entity.ToTable("ProductoFinanciero");

            entity.Property(e => e.IdProducto).HasColumnName("idProducto");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Plazo).HasColumnName("plazo");
            entity.Property(e => e.TasaInteres)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("tasaInteres");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipo");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ProductoFinancieros)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductoFinanciero_Usuario");
        });

        modelBuilder.Entity<Reclamo>(entity =>
        {
            entity.HasKey(e => e.IdReclamo);

            entity.ToTable("Reclamo");

            entity.Property(e => e.IdReclamo).HasColumnName("idReclamo");
            entity.Property(e => e.Descripcion)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.FechaRespuesta)
                .HasColumnType("datetime")
                .HasColumnName("fechaRespuesta");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Respuesta)
                .IsUnicode(false)
                .HasColumnName("respuesta");
            entity.Property(e => e.TipoReclamo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipoReclamo");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Reclamos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reclamo_Usuario");
        });

        modelBuilder.Entity<RequisitoCuentaAhorro>(entity =>
        {
            entity.HasKey(e => e.IdRequisito);

            entity.ToTable("RequisitoCuentaAhorro");

            entity.HasIndex(e => e.IdTipoCuenta, "IX_RequisitoCuentaAhorro_TipoCuenta");

            entity.Property(e => e.IdRequisito).HasColumnName("idRequisito");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdTipoCuenta).HasColumnName("idTipoCuenta");

            entity.HasOne(d => d.IdTipoCuentaNavigation).WithMany(p => p.RequisitoCuentaAhorros)
                .HasForeignKey(d => d.IdTipoCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RequisitoCuentaAhorro");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol);

            entity.ToTable("Rol");

            entity.HasIndex(e => e.Nombre, "UK_Rol_Nombre").IsUnique();

            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("ACTIVO")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<TipoCuentaAhorro>(entity =>
        {
            entity.HasKey(e => e.IdTipoCuenta);

            entity.ToTable("TipoCuentaAhorro");

            entity.HasIndex(e => e.Nombre, "UK_TipoCuentaAhorro_Nombre").IsUnique();

            entity.Property(e => e.IdTipoCuenta).HasColumnName("idTipoCuenta");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("ACTIVO")
                .HasColumnName("estado");
            entity.Property(e => e.Moneda)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("moneda");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.TasaInteres)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("tasaInteres");
        });

        modelBuilder.Entity<Transferencium>(entity =>
        {
            entity.HasKey(e => e.IdTransferencia);

            entity.HasIndex(e => new { e.IdCuentaOrigen, e.Fecha }, "IX_Transferencia_CuentaOrigen").IsDescending(false, true);

            entity.Property(e => e.IdTransferencia).HasColumnName("idTransferencia");
            entity.Property(e => e.BancoDestino)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("bancoDestino");
            entity.Property(e => e.Comision)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("comision");
            entity.Property(e => e.Concepto)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("concepto");
            entity.Property(e => e.CuentaDestinoExterna)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cuentaDestinoExterna");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdCuentaDestino).HasColumnName("idCuentaDestino");
            entity.Property(e => e.IdCuentaOrigen).HasColumnName("idCuentaOrigen");
            entity.Property(e => e.Monto)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("monto");
            entity.Property(e => e.NombreDestinatario)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nombreDestinatario");
            entity.Property(e => e.TipoTransferencia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tipoTransferencia");

            entity.HasOne(d => d.IdCuentaDestinoNavigation).WithMany(p => p.TransferenciumIdCuentaDestinoNavigations)
                .HasForeignKey(d => d.IdCuentaDestino)
                .HasConstraintName("FK_Transferencia_CuentaDestino");

            entity.HasOne(d => d.IdCuentaOrigenNavigation).WithMany(p => p.TransferenciumIdCuentaOrigenNavigations)
                .HasForeignKey(d => d.IdCuentaOrigen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transferencia_CuentaOrigen");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.Correo, "UK_Usuario_Correo").IsUnique();

            entity.HasIndex(e => new { e.TipoDocumento, e.NumeroDocumento }, "UK_Usuario_Documento").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.BloqueadoHasta)
                .HasColumnType("datetime")
                .HasColumnName("bloqueadoHasta");
            entity.Property(e => e.ContraseñaHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("contraseñaHash");
            entity.Property(e => e.Correo)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.IntentosFallidos).HasColumnName("intentosFallidos");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NumeroDocumento)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("numeroDocumento");
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("telefono");
            entity.Property(e => e.TipoDocumento)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("tipoDocumento");
            entity.Property(e => e.UltimoLogin)
                .HasColumnType("datetime")
                .HasColumnName("ultimoLogin");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
