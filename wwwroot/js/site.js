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

function toggleLike(idPublicacion) {
    fetch('/Home/ToggleLike', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ idPublicacion: idPublicacion })
    })
    .then(response => response.json())
    .then(data => {
        if (!data.ok) {
            alert(data.error || 'No se pudo procesar el Me Gusta.');
            return;
        }

        const likeButton = document.getElementById('btn-like-' + idPublicacion);
        const likesCount = document.getElementById('likes-count-' + idPublicacion);

        if (likeButton) {
            likeButton.innerText = data.liked ? 'Ya no me gusta' : 'Me gusta';
        }

        if (likesCount) {
            likesCount.innerText = data.likesCount + ' Me gusta';
        }
    })
    .catch((error) => {
        console.error('Error:', error);
        alert('Error de conexión al actualizar Me Gusta.');
    });
}

function getComentarios(id, btn){
    const container = document.getElementById("comentario-" + id);
    if (!container) return;

    // Si ya tiene contenido, hacer toggle (cerrar)
    if (container.innerHTML && container.innerHTML.trim().length > 0) {
        container.innerHTML = "";
        if (btn) btn.innerText = "Ver Comentarios";
        return;
    }

    fetch( '/Home/VerComentarios?id=' + id, {method: 'GET',
        headers: { 'Content-Type': 'application/json' },
    })
    .then(response => response.json())
    .then(data => {
        let body="";
        if (Array.isArray(data) && data.length > 0) {
            data.forEach(item => {
                const nombre = item.nombreUsuario || 'Usuario desconocido';
                const texto = item.texto || '';
                body += `<div class="comentario-item"><span class="comentario-usuario">${escapeHtml(nombre)}:</span> <span class="comentario-texto">${escapeHtml(texto)}</span></div>`;
            });
        } else {
            body = `<div class="no-comments">No hay comentarios.</div>`;
        }

        container.innerHTML = body;
        if (btn) btn.innerText = "Ocultar Comentarios";
    })
    .catch((error) => {
        console.error('Error:', error);
    });
}

let desdePublicaciones = 0;

document.addEventListener('DOMContentLoaded', function () {
    const container = document.getElementById('publicaciones-container');
    if (!container) {
        return;
    }

    const cantidadInicial = parseInt(container.getAttribute('data-cantidad-inicial') || '0');
    desdePublicaciones = isNaN(cantidadInicial) ? 0 : cantidadInicial;
});

function cargarMasPublicaciones() {
    const botonVerMas = document.getElementById('btn-ver-mas');
    const container = document.getElementById('publicaciones-container');

    if (!botonVerMas || !container) {
        return;
    }

    botonVerMas.disabled = true;

    fetch('/Publicacion/ObtenerMas?desde=' + desdePublicaciones, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(data => {
        if (!data.ok) {
            alert(data.error || 'No se pudieron cargar más publicaciones.');
            botonVerMas.disabled = false;
            return;
        }

        if (Array.isArray(data.publicaciones)) {
            data.publicaciones.forEach(publicacion => {
                container.insertAdjacentHTML('beforeend', crearHtmlPublicacion(publicacion));
            });
        }

        desdePublicaciones = data.siguienteDesde ?? desdePublicaciones;

        if (!data.hayMas) {
            botonVerMas.style.display = 'none';
        } else {
            botonVerMas.disabled = false;
        }
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error de conexión al cargar publicaciones.');
        botonVerMas.disabled = false;
    });
}

function crearHtmlPublicacion(publicacion) {
    const id = publicacion.id;
    const nombreUsuario = escapeHtml(publicacion.nombreUsuario || 'Usuario desconocido');
    const titulo = escapeHtml(publicacion.titulo || '');
    const descripcion = escapeHtml(publicacion.descripcion || '');
    const fecha = formatearFecha(publicacion.fechaPublicacion);
    const likes = Number.isInteger(publicacion.likesCount) ? publicacion.likesCount : 0;
    const textoBotonLike = publicacion.usuarioDioLike ? 'Ya no me gusta' : 'Me gusta';
    const imagen = publicacion.imagen;

    let imagenHtml = '';
    if (imagen && imagen.trim().length > 0) {
        imagenHtml = `
            <div class="post-image-wrap">
                <img class="post-image" src="${escapeAttribute(imagen)}" alt="${titulo}" />
            </div>`;
    }

    return `
        <article class="post-card">
            <div class="post-body">
                <div class="post-username">${nombreUsuario}</div>
                <div class="post-title">${titulo}</div>
                <div class="post-date">${fecha}</div>
            </div>
            ${imagenHtml}
            <div class="post-description">${descripcion}</div>
            <div class="post-actions">
                <button id="btn-like-${id}" onclick="toggleLike(${id})" class="btn btn-like">${textoBotonLike}</button>
                <span id="likes-count-${id}">${likes} Me gusta</span>
                <div class="comentario-form">
                    <button type="button" onclick="comentarAjax(${id}, this)" class="btn btn-comment">Comentar</button>
                    <input id="comentario-input-${id}" type="text" name="comentario" placeholder="Escribe un comentario..." required />
                </div>
            </div>
            <div class="comentarios">
                <button onclick="getComentarios(${id}, this)" class="btn btn-comment">Ver Comentarios</button>
                <div id="comentario-${id}" class="comentarios-list"></div>
            </div>
        </article>`;
}

function formatearFecha(fechaIso) {
    const fecha = new Date(fechaIso);
    if (Number.isNaN(fecha.getTime())) {
        return '';
    }

    const dia = String(fecha.getDate()).padStart(2, '0');
    const mes = String(fecha.getMonth() + 1).padStart(2, '0');
    const anio = fecha.getFullYear();
    const horas = String(fecha.getHours()).padStart(2, '0');
    const minutos = String(fecha.getMinutes()).padStart(2, '0');
    return `${dia}/${mes}/${anio} ${horas}:${minutos}`;
}

function escapeHtml(valor) {
    return String(valor)
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#39;');
}

function escapeAttribute(valor) {
    return String(valor)
        .replaceAll('&', '&amp;')
        .replaceAll('"', '&quot;')
        .replaceAll("'", '&#39;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;');
}

function comentarAjax(id, btn){
    const input = document.getElementById('comentario-input-' + id);
    if (!input) return;

    const texto = input.value.trim();
    if (!texto) { alert('Escribe un comentario.'); return; }

    btn.disabled = true;

    fetch('/Home/ComentarAjax', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ idPublicacion: id, comentario: texto })
    })
    .then(response => response.json())
    .then(data => {
        if (!data.ok) {
            alert(data.error || 'No se pudo publicar el comentario.');
            return;
        }

        const article = btn.closest('article');
        const cont = document.getElementById('comentario-' + id) || (article ? article.querySelector('.comentarios-list') : null);

        const nombre = data.comentario.nombreUsuario || 'Usuario desconocido';
        const textoC = data.comentario.texto || '';
        const html = `<div class="comentario-item"><span class="comentario-usuario">${escapeHtml(nombre)}:</span> <span class="comentario-texto">${escapeHtml(textoC)}</span></div>`;

        if (cont) {
            // If previously showed "No hay comentarios.", remove that placeholder
            const placeholder = cont.querySelector('.no-comments');
            if (placeholder) placeholder.remove();
            cont.insertAdjacentHTML('beforeend', html);
        }

        // Asegurar que el botón "Ver Comentarios" muestre que están visibles
        if (article) {
            const verBtn = article.querySelector('.comentarios button');
            if (verBtn && verBtn.innerText.trim() === 'Ver Comentarios') {
                verBtn.innerText = 'Ocultar Comentarios';
            }
        }

        input.value = '';
    })
    .catch(err => {
        console.error(err);
        alert('Error de conexión al enviar comentario.');
    })
    .finally(() => { btn.disabled = false; });
}