using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
{
    public RepositorioUsuario(string connectionString) : base(connectionString) { }

    public int Alta(Usuario usuario)
    {
        int id = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                INSERT INTO usuarios (Nombre, Apellido, DNI, Matricula, Email, PasswordHash, Rol, AvatarURL, Telefono, Borrado)
                VALUES (@Nombre, @Apellido, @DNI, @Matricula, @Email, @PasswordHash, @Rol, @AvatarURL, @Telefono, 0);
                SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Nombre", usuario.Nombre ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Apellido", usuario.Apellido ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DNI", usuario.DNI ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Matricula", usuario.Matricula ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Email", usuario.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Rol", usuario.Rol ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@AvatarURL", usuario.AvatarURL ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Telefono", usuario.Telefono ?? (object)DBNull.Value);

                id = Convert.ToInt32(command.ExecuteScalar());
            }
            connection.Close();
        }
        return id;
    }

    // Baja lógica (Soft Delete) para preservar la integridad referencial de vacunación
    public int Baja(int id)
    {
        int rows = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "UPDATE usuarios SET Borrado = 1, FechaBaja = NOW() WHERE UsuarioID = @Id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                rows = command.ExecuteNonQuery();
            }
            connection.Close();
        }
        return rows;
    }

    public int Modificar(Usuario usuario)
    {
        int rows = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = @"
                UPDATE usuarios SET
                    Nombre = @Nombre,
                    Apellido = @Apellido,
                    DNI = @DNI,
                    Matricula = @Matricula,
                    Email = @Email,
                    Rol = @Rol,
                    Telefono = @Telefono";

            if (!string.IsNullOrEmpty(usuario.PasswordHash))
            {
                sql += ", PasswordHash = @PasswordHash";
            }

            if (!string.IsNullOrEmpty(usuario.AvatarURL))
            {
                sql += ", AvatarURL = @AvatarURL";
            }

            sql += " WHERE UsuarioID = @UsuarioID";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Nombre", usuario.Nombre ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Apellido", usuario.Apellido ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DNI", usuario.DNI ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Matricula", usuario.Matricula ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Email", usuario.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Rol", usuario.Rol ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Telefono", usuario.Telefono ?? (object)DBNull.Value);

                if (!string.IsNullOrEmpty(usuario.PasswordHash))
                {
                    command.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash);
                }

                if (!string.IsNullOrEmpty(usuario.AvatarURL))
                {
                    command.Parameters.AddWithValue("@AvatarURL", usuario.AvatarURL);
                }

                command.Parameters.AddWithValue("@UsuarioID", usuario.UsuarioID);

                rows = command.ExecuteNonQuery();
            }

            connection.Close();
        }
        return rows;
    }

    public Usuario? ObtenerPorId(int id)
    {
        Usuario? usuario = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "SELECT * FROM usuarios WHERE UsuarioID = @Id AND (Borrado = 0 OR Borrado IS NULL) LIMIT 1";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = MapearUsuario(reader);
                    }
                }
            }
            connection.Close();
        }
        return usuario;
    }

    public Usuario? ObtenerPorEmail(string email)
    {
        Usuario? usuario = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "SELECT * FROM usuarios WHERE Email = @Email AND (Borrado = 0 OR Borrado IS NULL) LIMIT 1";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = MapearUsuario(reader);
                    }
                }
            }
            connection.Close();
        }
        return usuario;
    }

    public IEnumerable<Usuario> ObtenerTodos()
    {
        var lista = new List<Usuario>();
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            var sql = "SELECT * FROM usuarios WHERE (Borrado = 0 OR Borrado IS NULL)";
            using (var command = new MySqlCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(MapearUsuario(reader));
                }
            }
            connection.Close();
        }
        return lista;
    }

    private Usuario MapearUsuario(IDataRecord r)
    {
        return new Usuario
        {
            UsuarioID = r["UsuarioID"] == DBNull.Value ? 0 : Convert.ToInt32(r["UsuarioID"]),
            Nombre = r["Nombre"] == DBNull.Value ? null : r["Nombre"].ToString(),
            Apellido = r["Apellido"] == DBNull.Value ? null : r["Apellido"].ToString(),
            DNI = r["DNI"] == DBNull.Value ? null : r["DNI"].ToString(),
            Matricula = r["Matricula"] == DBNull.Value ? null : r["Matricula"].ToString(),
            Email = r["Email"] == DBNull.Value ? null : r["Email"].ToString(),
            PasswordHash = r["PasswordHash"] == DBNull.Value ? null : r["PasswordHash"].ToString(),
            Rol = r["Rol"] == DBNull.Value ? null : r["Rol"].ToString(),
            AvatarURL = r["AvatarURL"] == DBNull.Value ? null : r["AvatarURL"].ToString(),
            Telefono = r["Telefono"] == DBNull.Value ? null : r["Telefono"].ToString(),
            FechaBaja = HasColumn(r, "FechaBaja") && r["FechaBaja"] != DBNull.Value ? Convert.ToDateTime(r["FechaBaja"]) : null,
            Borrado = HasColumn(r, "Borrado") && r["Borrado"] != DBNull.Value && Convert.ToBoolean(r["Borrado"])
        };
    }

    private static bool HasColumn(IDataRecord dr, string columnName)
    {
        for (int i = 0; i < dr.FieldCount; i++)
        {
            if (dr.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
