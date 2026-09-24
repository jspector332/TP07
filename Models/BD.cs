namespace TP07.Models;
using Dapper;
using Microsoft.Data.SqlClient;

public class BD
{
    private string _connectionString = @"Server=localhost;DataBase = DBRedSocial;integrated Security = True;TrustServerCertificate=True;";

    public void agregarUsuario(Usuarios usuario)
    {
        string query = "INSERT INTO Usuarios (NombreUsuario, Contraseña, Nombre, Apellido) VALUES (@pNombreUsuario, @pContraseña, @pNombre, @pApellido)";
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Execute(query, new {pNombreUsuario = usuario.NombreUsuario, pContraseña = usuario.Contraseña, pNombre = usuario.Nombre, pApellido = usuario.Apellido});
        }
    }
    public void agregarPublicacion(Publicaciones publicacion)
    {
        string query = "INSERT INTO Publicaciones (Imagen, Titulo, Descripcion, IdUsuario, FechaPublicacion) VALUES (@pImagen, @pTitulo, @pDescripcion, @pIdUsuario, @pFecha)";
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            connection.Execute(query, new {pImagen = publicacion.Imagen, pTitulo = publicacion.Titulo, pDescripcion = publicacion.Descripcion, pIdUsuario = publicacion.IdUsuario, pFecha = publicacion.FechaPublicacion});
        }
    }

    public List<Publicaciones> obtenerPublicaciones()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT Id, IdUsuario, Titulo, Descripcion, Imagen, FechaPublicacion
                FROM Publicaciones
                ORDER BY FechaPublicacion DESC, Id DESC";

            return connection.Query<Publicaciones>(query).ToList();
        }
    }

    public Dictionary<int, string> obtenerNombresUsuarios()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT Id, NombreUsuario FROM Usuarios";
            return connection.Query<(int Id, string NombreUsuario)>(query)
                .ToDictionary(x => x.Id, x => x.NombreUsuario);
        }
    }

    public Usuarios verificarUsuario(Usuarios usuario)
    {
        Usuarios usuarioExistente = null;
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            string query = "SELECT * FROM Usuarios WHERE NombreUsuario = @pNombreUsuario";
            usuarioExistente = connection.QueryFirstOrDefault<Usuarios>(query, new {pNombreUsuario = usuario.NombreUsuario});
        }
        return usuarioExistente;
    }
    public Usuarios buscarXId(int id)
    {
        Usuarios usuarioExistente = null;
        using(SqlConnection connection = new SqlConnection(_connectionString)){
            string query = "SELECT * FROM Usuarios WHERE Id = @pId";
            usuarioExistente = connection.QueryFirstOrDefault<Usuarios>(query, new {pId = id});
        }
        return usuarioExistente;
    }
}