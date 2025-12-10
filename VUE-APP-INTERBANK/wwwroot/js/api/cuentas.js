const URL_BASE_CUENTAS = "http://localhost:5083/api/CuentaAhorro";

var CuentasAPI = {
    // Obtener los tipos de cuenta disponibles (Ej: Ahorro Simple, Sueldo)
    listarTipos: function () {
        return Vue.http.get(`${URL_BASE_CUENTAS}/types`);
    },

    // Ver requisitos de un tipo especifico (Ej: Monto min, documentos)
    verRequisitos: function (idTipo) {
        return Vue.http.get(`${URL_BASE_CUENTAS}/requirements/${idTipo}`);
    },

    // Crear una nueva cuenta (Recibe CrearCuentaAhorroDto)
    crear: function (dto) {
        return Vue.http.post(URL_BASE_CUENTAS, dto);
    },

    // Actualizar datos de una cuenta (Recibe GuardarDetalleProductoDTO)
    actualizar: function (idCuenta, dto) {
        return Vue.http.put(`${URL_BASE_CUENTAS}/${idCuenta}`, dto);
    },

    // Eliminar (Cerrar) cuenta
    eliminar: function (idCuenta) {
        return Vue.http.delete(`${URL_BASE_CUENTAS}/${idCuenta}`);
    }
};