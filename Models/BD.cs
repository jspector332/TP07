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

    public List<Publicaciones> obtenerPublicacionesPaginadas(int desde, int cantidad)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT Id, IdUsuario, Titulo, Descripcion, Imagen, FechaPublicacion
                FROM Publicaciones
                ORDER BY FechaPublicacion DESC, Id DESC
                OFFSET @pDesde ROWS FETCH NEXT @pCantidad ROWS ONLY";

            return connection.Query<Publicaciones>(query, new { pDesde = desde, pCantidad = cantidad }).ToList();
        }
    }

    public int obtenerCantidadPublicaciones()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(1) FROM Publicaciones";
            return connection.ExecuteScalar<int>(query);
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

    public List<Comentarios> GetComentarios(int idPublicacion)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM Comentarios WHERE IdPublicacion = @pIdPublicacion ORDER BY FechaComentario ASC";
            return connection.Query<Comentarios>(query, new { pIdPublicacion = idPublicacion }).ToList();
        }
    }

    public void agregarComentario(Comentarios comentario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "INSERT INTO Comentarios (IdPublicacion, IdUsuarioComenta, Texto, FechaComentario) VALUES (@pIdPublicacion, @pIdUsuarioComenta, @pTexto, @pFechaComentario)";
            connection.Execute(query, new { pIdPublicacion = comentario.IdPublicacion, pIdUsuarioComenta = comentario.IdUsuarioComenta, pTexto = comentario.Texto, pFechaComentario = comentario.FechaComentario });
        }
    }

    public bool publicacionExiste(int idPublicacion)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(1) FROM Publicaciones WHERE Id = @pIdPublicacion";
            int cantidad = connection.ExecuteScalar<int>(query, new { pIdPublicacion = idPublicacion });
            return cantidad > 0;
        }
    }

    public bool usuarioDioLike(int idPublicacion, int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(1) FROM PublicacionesMeGusta WHERE [IdPublicación] = @pIdPublicacion AND IdUsuario = @pIdUsuario";
            int cantidad = connection.ExecuteScalar<int>(query, new { pIdPublicacion = idPublicacion, pIdUsuario = idUsuario });
            return cantidad > 0;
        }
    }

    public void agregarLike(int idPublicacion, int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "INSERT INTO PublicacionesMeGusta ([IdPublicación], IdUsuario) VALUES (@pIdPublicacion, @pIdUsuario)";
            connection.Execute(query, new { pIdPublicacion = idPublicacion, pIdUsuario = idUsuario });
        }
    }

    public void quitarLike(int idPublicacion, int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "DELETE FROM PublicacionesMeGusta WHERE [IdPublicación] = @pIdPublicacion AND IdUsuario = @pIdUsuario";
            connection.Execute(query, new { pIdPublicacion = idPublicacion, pIdUsuario = idUsuario });
        }
    }

    public int obtenerCantidadLikes(int idPublicacion)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT COUNT(1) FROM PublicacionesMeGusta WHERE [IdPublicación] = @pIdPublicacion";
            return connection.ExecuteScalar<int>(query, new { pIdPublicacion = idPublicacion });
        }
    }

    public Dictionary<int, int> obtenerCantidadLikesPorPublicacion()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = @"
                SELECT [IdPublicación] AS IdPublicacion, COUNT(1) AS Cantidad
                FROM PublicacionesMeGusta
                GROUP BY [IdPublicación]";

            return connection.Query<(int IdPublicacion, int Cantidad)>(query)
                .ToDictionary(x => x.IdPublicacion, x => x.Cantidad);
        }
    }

    public HashSet<int> obtenerIdsPublicacionesConLikeDeUsuario(int idUsuario)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT [IdPublicación] FROM PublicacionesMeGusta WHERE IdUsuario = @pIdUsuario";
            return connection.Query<int>(query, new { pIdUsuario = idUsuario }).ToHashSet();
        }
    }
}