using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TP07.Models;
using Microsoft.AspNetCore.Http;

namespace TP07.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment _environment;

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
            Usuarios usuarioAView = bd.buscarXId(int.Parse(HttpContext.Session.GetString("ID")));
            
            if(usuarioAView != null)
            { ViewBag.Usuario = usuarioAView; }
            return View();
        }
        else
        {
            return View("Index");
        }
    }
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
    public IActionResult Publicar()
    {
        return View();
    }
    public IActionResult AgregarPublicacion()
    {
        
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
}
