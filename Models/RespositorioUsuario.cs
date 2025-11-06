using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using VacunacionTPFinal.Models;

namespace VacunacionTPFinal.Models
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Usuario usuario)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Usuarios (Email, PasswordHash, AvatarURL)
                             VALUES (@Email, @PasswordHash, @AvatarURL);
                             SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", usuario.Email);
                    command.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash);
                    command.Parameters.AddWithValue("@AvatarURL", usuario.AvatarURL);
                    connection.Open();
                    usuario.ID_Usuario = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return usuario.ID_Usuario;
        }

        public int Modificacion(Usuario usuario)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Usuarios 
                             SET Email = @Email, PasswordHash = @PasswordHash, AvatarURL = @AvatarURL
                             WHERE ID_Usuario = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Email", usuario.Email);
                    command.Parameters.AddWithValue("@PasswordHash", usuario.PasswordHash);
                    command.Parameters.AddWithValue("@AvatarURL", usuario.AvatarURL);
                    command.Parameters.AddWithValue("@id", usuario.ID_Usuario);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? usuario = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                const string sql = @"SELECT ID_Usuario, Email, PasswordHash, AvatarURL
                                   FROM Usuarios
                                   WHERE ID_Usuario = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                ID_Usuario = reader.GetInt32("ID_Usuario"),
                                Email = reader.GetString("Email"),
                                PasswordHash = reader.GetString("PasswordHash"),
                                AvatarURL = reader.IsDBNull(reader.GetOrdinal("AvatarURL")) ? string.Empty : reader.GetString("AvatarURL")
                            };
                        }
                    }
                }
            }
            return usuario;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? usuario = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                const string sql = @"SELECT ID_Usuario, Email, PasswordHash, AvatarURL
                                   FROM Usuarios
                                   WHERE Email = @email";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                ID_Usuario = reader.GetInt32("ID_Usuario"),
                                Email = reader.GetString("Email"),
                                PasswordHash = reader.GetString("PasswordHash"),
                                AvatarURL = reader.IsDBNull(reader.GetOrdinal("AvatarURL")) ? string.Empty : reader.GetString("AvatarURL")
                            };
                        }
                    }
                }
            }
            return usuario;
        }

        public int ModificarAvatar(int idUsuario, string avatarUrl)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Usuarios 
                             SET AvatarURL = @AvatarURL
                             WHERE ID_Usuario = @idUsuario";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@AvatarURL", avatarUrl);
                    command.Parameters.AddWithValue("@idUsuario", idUsuario);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}