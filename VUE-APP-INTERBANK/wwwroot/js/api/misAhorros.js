const URL_BASE_MIS_AHORROS = "http://localhost:5083/api/DetalleAhorro";

var MisAhorrosAPI = {
    // Listar todas las cuentas de un usuario especifico
    listarPorUsuario: function (idUsuario) {
        return Vue.http.get(`${URL_BASE_MIS_AHORROS}/user/${idUsuario}`);
    },

    // Ver el detalle completo de un deposito especifico
    verDetalle: function (idDeposito) {
        return Vue.http.get(`${URL_BASE_MIS_AHORROS}/${idDeposito}`);
    }
};