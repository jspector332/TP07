// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const fbnombre = document.getElementById("fb-nombre");
const fbapellido = document.getElementById("fb-apellido");
const fbnombreUsuario = document.getElementById("fb-nombreUsuario");
const fbcontrasenia = document.getElementById("fb-contrasenia");
const fbnombreUsuarioLogin = document.getElementById("fb-nombreusuariologin");
const fbcontrasenialogin = document.getElementById("fb-contlogin");
const nombre = document.getElementById("nombre");
const apellido = document.getElementById("apellido");
const nombreUsuario = document.getElementById("nombreUsuario");
const nombreUsuarioLogin = document.getElementById("nombreUsuarioLogin");
const contrasenia = document.getElementById("contrasenia");
const contraseniaLogin = document.getElementById("contraseniaLogin");

const errores = [];
function enviarFormulario(){
    errores.length = 0;
    
    const nombreV = nombre.value;
    const apellidoV = apellido.value;
    const nombreUsuarioV = nombreUsuario.value;
    const contraseniaV = contrasenia.value;

    limpiarFeedbacks();

    if (!nombreV || nombreV.trim().length === 0) {
        errores.push("El nombre esta vacio/es invalido.");
        mostrarError("fb-nombre", "Campo obligatorio.");
    }
    else{
        mostrarOK("fb-nombre", "OK");
    }

    if (!apellidoV || apellidoV.trim().length === 0) {
        errores.push("El apellido esta vacio/es invalido.");
        mostrarError("fb-apellido", "Campo obligatorio.");
    }
    else{
        mostrarOK("fb-apellido", "OK");
    }

    if (!nombreUsuarioV || nombreUsuarioV.trim().length === 0) {
        errores.push("El nombre de usuario esta vacio/es invalido.");
        mostrarError("fb-nombreUsuario", "Campo obligatorio.");
    }
    else{
        mostrarOK("fb-nombreUsuario", "OK");
    }

    if (!contraseniaV || contraseniaV.trim().length === 0 || contraseniaV.length < 8) {
        errores.push("La contraseña es obligatoria y debe tener al menos 8 caracteres.");
        mostrarError("fb-contrasenia", "Campo obligatorio.");
    }
    else{
        mostrarOK("fb-contrasenia", "OK");
    }

    if (errores.length > 0){
        divResultado.style.color  = "red";
        divResultado.style.border = "1px solid red";
        divResultado.style.padding = "2%";
        divResultado.innerHTML = "<strong>No se pudo enviar:</strong><br>" + errores.join("<br>");
    }
    else{
        divResultado.style.color  = "green";
        divResultado.style.border = "1px solid green";
        divResultado.style.padding = "2%";
        divResultado.innerHTML = "<strong>¡Registro exitoso!</strong><br>";
    }
}

function mostrarError(id, msg) {
    const el = document.getElementById(id);
    el.innerHTML = msg;
    el.style.color = "red";
}

function mostrarOK(id, msg){
    const el = document.getElementById(id);
    el.innerHTML = msg;
    el.style.color = "green";
}

function limpiarFeedbacks() {
    fbnombre.innerHTML = "";
    fbapellido.innerHTML = "";
    fbnombreUsuario.innerHTML = "";
    fbcontrasenia.innerHTML = "";
    divResultado.innerHTML = "";
}

function limpiarFeedbacksLogin() {
    fbnombreUsuarioLogin.innerHTML = "";
    fbcontrasenialogin.innerHTML = "";
    divResultadoLogin.innerHTML = "";
}

const erroresLogin = [];
function enviarFormLogin(){
    erroresLogin.length = 0;
    const nombreUsuarioV = nombreUsuarioLogin.value;
    const contraseniaV = contraseniaLogin.value;

    limpiarFeedbacksLogin();

    if (!nombreUsuarioV || nombreUsuarioV.trim().length === 0) {
        erroresLogin.push("El nombre de usuario esta vacio/no es valido.");
        mostrarError("fb-nombreusuariologin", "Campo obligatorio.");
    }
    else{
        mostrarOK("fb-nombreusuariologin", "OK");
    }

    if (!contraseniaV || contraseniaV.trim().length === 0 || contraseniaV.length < 5) {
        erroresLogin.push("La contraseña es incorrecta o esta vacia.");
        mostrarError("fb-contlogin", "Campo obligatorio.");
    }
    else{
        mostrarOK("fb-contlogin", "OK");
    }

    if (erroresLogin.length > 0){
        divResultadoLogin.style.color  = "red";
        divResultadoLogin.style.border = "1px solid red";
        divResultadoLogin.style.padding = "2%";
        divResultadoLogin.innerHTML = "<strong>No se pudo enviar:</strong><br>" + erroresLogin.join("<br>");
    }
    else{
        divResultadoLogin.style.color  = "green";
        divResultadoLogin.style.border = "1px solid green";
        divResultadoLogin.style.padding = "2%";
        divResultadoLogin.innerHTML = "<strong>¡Login exitoso!</strong><br>";
    }
}

function validarFormulario() {
    if (errores.length > 0) {
        return false;
    }
    else{
        return true;
    }
}

function validarFormularioLogin() {
    if (erroresLogin.length > 0) {
        return false;
    }
    else{
        return true;
    }
}