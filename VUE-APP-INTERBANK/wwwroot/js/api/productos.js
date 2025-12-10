const URL_BASE_PRODUCTOS = "http://localhost:5083/api/ProductosFinancieros";

var ProductosAPI = {
    // Listar productos personalizados para el usuario
    listarPersonalizados: function (idUsuario) {
        return Vue.http.get(`${URL_BASE_PRODUCTOS}/personalized/${idUsuario}`);
    },

    // Obtener info de un producto especifico
    obtener: function (idProducto) {
        return Vue.http.get(`${URL_BASE_PRODUCTOS}/${idProducto}`);
    },

    // Agregar a favoritos (Recibe CrearProductoFavoritoDto)
    agregarFavorito: function (dto) {
        return Vue.http.post(`${URL_BASE_PRODUCTOS}/favorite`, dto);
    },

    // Eliminar de favoritos (Query Params)
    eliminarFavorito: function (userId, productId) {
        return Vue.http.delete(`${URL_BASE_PRODUCTOS}/favorite?userId=${userId}&productId=${productId}`);
    },

    // Actualizar producto (Admin)
    actualizar: function (idProducto, dto) {
        return Vue.http.put(`${URL_BASE_PRODUCTOS}/${idProducto}`, dto);
    }
};