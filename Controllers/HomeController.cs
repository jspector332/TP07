using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TP07.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace TP07.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _environment;
    private const int CantidadPublicacionesPorPagina = 10;

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

        BD bd = new BD();
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Verificar(Usuarios usuario)
    {
        if(bd.verificarUsuario(usuario) == null)
        {
            bd.agregarUsuario(usuario);
        }
        else{
            ViewBag.Error = "Este usuario ya existe. Prueba otro.";
            return View("Registro");
        }
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public IActionResult VerificarLogin(Usuarios usuario)
    {
        Usuarios usuarioExistente = bd.verificarUsuario(usuario);
        if (usuarioExistente != null && usuarioExistente.Contraseña == usuario.Contraseña)
        {
            HttpContext.Session.SetString("Usuario", usuarioExistente.NombreUsuario);
            HttpContext.Session.SetString("ID", usuarioExistente.Id.ToString());
            Console.WriteLine("ID: " + usuarioExistente.Id.ToString());
            return RedirectToAction("Home");
        }
        else
        {
            ViewBag.Error= "Nombre de usuario o contraseña incorrectos.";
            return View("Login");
        }
    }

    public IActionResult Home()
    {   
        if(HttpContext.Session.GetString("Usuario") != null)
        {
            ViewBag.UsuarioNombre = HttpContext.Session.GetString("Usuario");
            int idUsuario = int.Parse(HttpContext.Session.GetString("ID"));
            Usuarios usuarioAView = bd.buscarXId(idUsuario);
            
            if(usuarioAView != null)
            { ViewBag.Usuario = usuarioAView; }

            var publicaciones = bd.obtenerPublicacionesPaginadas(0, CantidadPublicacionesPorPagina);
            int totalPublicaciones = bd.obtenerCantidadPublicaciones();
            ViewBag.NombresUsuarios = bd.obtenerNombresUsuarios();
            ViewBag.LikesPorPublicacion = bd.obtenerCantidadLikesPorPublicacion();
            ViewBag.PublicacionesConLikeDelUsuario = bd.obtenerIdsPublicacionesConLikeDeUsuario(idUsuario);
            ViewBag.CantidadInicialPublicaciones = publicaciones.Count;
            ViewBag.HayMasPublicaciones = totalPublicaciones > publicaciones.Count;
            return View(publicaciones);
        }
        else
        {
            return View("Index");
        }
    }

    [HttpGet("/Publicacion/ObtenerMas")]
    public JsonResult ObtenerMas(int desde = 0)
    {
        string idUsuarioSession = HttpContext.Session.GetString("ID");
        if (string.IsNullOrEmpty(idUsuarioSession))
        {
            Response.StatusCode = 401;
            return Json(new { ok = false, error = "Usuario no autenticado." });
        }

        if (desde < 0)
        {
            return Json(new { ok = false, error = "Parámetro inválido." });
        }

        int idUsuario = int.Parse(idUsuarioSession);
        var publicaciones = bd.obtenerPublicacionesPaginadas(desde, CantidadPublicacionesPorPagina);
        int totalPublicaciones = bd.obtenerCantidadPublicaciones();
        var nombresUsuarios = bd.obtenerNombresUsuarios();
        var likesPorPublicacion = bd.obtenerCantidadLikesPorPublicacion();
        var publicacionesConLikeDelUsuario = bd.obtenerIdsPublicacionesConLikeDeUsuario(idUsuario);

        var publicacionesRespuesta = publicaciones.Select(publicacion => new
        {
            id = publicacion.Id,
            titulo = publicacion.Titulo,
            descripcion = publicacion.Descripcion,
            fechaPublicacion = publicacion.FechaPublicacion,
            imagen = NormalizarRutaImagen(publicacion.Imagen),
            nombreUsuario = nombresUsuarios != null && nombresUsuarios.ContainsKey(publicacion.IdUsuario)
                ? nombresUsuarios[publicacion.IdUsuario]
                : "Usuario desconocido",
            likesCount = likesPorPublicacion != null && likesPorPublicacion.ContainsKey(publicacion.Id)
                ? likesPorPublicacion[publicacion.Id]
                : 0,
            usuarioDioLike = publicacionesConLikeDelUsuario != null && publicacionesConLikeDelUsuario.Contains(publicacion.Id)
        });

        bool hayMas = (desde + publicaciones.Count) < totalPublicaciones;

        return Json(new
        {
            ok = true,
            publicaciones = publicacionesRespuesta,
            hayMas = hayMas,
            siguienteDesde = desde + publicaciones.Count
        });
    }

    [HttpPost]
    public JsonResult ToggleLike([FromBody] ToggleLikeRequest request)
    {
        string idUsuarioSession = HttpContext.Session.GetString("ID");
        if (string.IsNullOrEmpty(idUsuarioSession))
        {
            return Json(new { ok = false, error = "Usuario no autenticado." });
        }

        if (request == null || request.IdPublicacion <= 0)
        {
            return Json(new { ok = false, error = "Publicación inválida." });
        }

        int idUsuario = int.Parse(idUsuarioSession);

        if (!bd.publicacionExiste(request.IdPublicacion))
        {
            return Json(new { ok = false, error = "La publicación no existe." });
        }

        bool yaDioLike = bd.usuarioDioLike(request.IdPublicacion, idUsuario);

        if (yaDioLike)
        {
            bd.quitarLike(request.IdPublicacion, idUsuario);
        }
        else
        {
            bd.agregarLike(request.IdPublicacion, idUsuario);
        }

        int cantidadLikes = bd.obtenerCantidadLikes(request.IdPublicacion);

        return Json(new
        {
            ok = true,
            liked = !yaDioLike,
            likesCount = cantidadLikes,
            idPublicacion = request.IdPublicacion
        });
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
    public IActionResult Publicar()
    {
        ViewBag.UsuarioNombre = HttpContext.Session.GetString("Usuario");
        ViewBag.FechaPublicacion = DateTime.Now;
        return View();
    }
    [HttpPost]
    public IActionResult AgregarPublicacion(Publicaciones publicacion, IFormFile Imagen)
    {
        if (Imagen != null && Imagen.Length > 0)
        {
            string carpetaImagenes = Path.Combine(_environment.WebRootPath, "imagenes");
            Directory.CreateDirectory(carpetaImagenes);

            string extension = Path.GetExtension(Imagen.FileName);
            string nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string rutaCompleta = Path.Combine(carpetaImagenes, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                Imagen.CopyTo(stream);
            }

            publicacion.Imagen = $"/imagenes/{nombreArchivo}";
        }

        publicacion.FechaPublicacion = DateTime.Now;
        bd.agregarPublicacion(publicacion);
        return RedirectToAction("Home");
    }

    public List<Comentarios> VerComentarios(int id)
    {
        return bd.GetComentarios(id);
    }

    public IActionResult Comentar(int idPublicacion, string comentario)
    {
        if (HttpContext.Session.GetString("Usuario") != null)
        {
            Comentarios nuevoComentario = new Comentarios
            {
                IdPublicacion = idPublicacion,
                IdUsuarioComenta = int.Parse(HttpContext.Session.GetString("ID")),
                Texto = comentario,
                FechaComentario = DateTime.Now
            };

            bd.agregarComentario(nuevoComentario);
            return RedirectToAction("Home");
        }
        else
        {
            return RedirectToAction("Index");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public class ToggleLikeRequest
    {
        public int IdPublicacion { get; set; }
    }

    private string NormalizarRutaImagen(string imagen)
    {
        if (string.IsNullOrWhiteSpace(imagen))
        {
            return null;
        }

        return imagen.StartsWith("/") ? imagen : $"/imagenes/{imagen}";
    }
}
