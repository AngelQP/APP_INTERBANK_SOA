const URL_BASE_MOVIMIENTOS = "http://localhost:5083/api/DetalleMovimiento";

var MovimientosAPI = {
    // Listar todos los movimientos de una cuenta
    listar: function (idCuenta) {
        return Vue.http.get(`${URL_BASE_MOVIMIENTOS}/${idCuenta}`);
    },

    // Listar movimientos por rango de fechas (Query Params)
    // Formato de fechas sugerido: YYYY-MM-DD
    listarPorRango: function (idCuenta, desde, hasta) {
        return Vue.http.get(`${URL_BASE_MOVIMIENTOS}/${idCuenta}/range?desde=${desde}&hasta=${hasta}`);
    }
};