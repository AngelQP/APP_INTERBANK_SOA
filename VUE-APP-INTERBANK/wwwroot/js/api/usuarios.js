
// URL_BASE para usuarios
const URL_BASE_USUARIOS = "http://localhost:5083/api/Usuarios";

// Consumo de api para usuarios

var UsuariosAPI = {

    registrar(data) {
        var url = `${URL_BASE_USUARIOS}/registrarCliente`; 
        return Vue.http.post(url, data);
    },

    login(data) {
        var url = `${URL_BASE_USUARIOS}/login`;
        return Vue.http.post(url, data);
    },

    listar() {
        var url = `${URL_BASE_USUARIOS}/listarClientes`;
        return Vue.http.get(url);
    },

    actualizar(id, data) {
        var url = `${URL_BASE_USUARIOS}/actualizarCliente/${id}`;
        return Vue.http.put(url, data);
    },

    eliminar(id) {
        console.log(`URL: ${URL_BASE_USUARIOS}/eliminarCliente/${id}`);
        var url = `${URL_BASE_USUARIOS}/eliminarCliente/${id}`;
        return Vue.http.delete(url);
    }
}