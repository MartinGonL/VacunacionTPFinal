using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using VacunacionTPFinal.Models;

namespace VacunacionTPFinal.Models
{
    public class RepositorioAgenteSanitario : RepositorioBase, IRepositorioAgenteSanitario
    {
        public RepositorioAgenteSanitario(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(AgenteSanitario agente)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO AgentesSanitarios (Nombre, Apellido, Dni, Matricula, Telefono, ID_Usuario)
                             VALUES (@Nombre, @Apellido, @Dni, @Matricula, @Telefono, @ID_Usuario);
                             SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", agente.Nombre);
                    command.Parameters.AddWithValue("@Apellido", agente.Apellido);
                    command.Parameters.AddWithValue("@Dni", agente.Dni);
                    command.Parameters.AddWithValue("@Matricula", agente.Matricula);
                    command.Parameters.AddWithValue("@Telefono", agente.Telefono);
                    command.Parameters.AddWithValue("@ID_Usuario", agente.ID_Usuario.HasValue ? (object)agente.ID_Usuario.Value : DBNull.Value);
                    connection.Open();
                    agente.ID_Agente = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return agente.ID_Agente;
        }

        public int Baja(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM AgentesSanitarios WHERE ID_Agente = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificacion(AgenteSanitario agente)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE AgentesSanitarios 
                             SET Nombre = @Nombre, Apellido = @Apellido, Dni = @Dni, 
                                 Matricula = @Matricula, Telefono = @Telefono, ID_Usuario = @ID_Usuario
                             WHERE ID_Agente = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", agente.Nombre);
                    command.Parameters.AddWithValue("@Apellido", agente.Apellido);
                    command.Parameters.AddWithValue("@Dni", agente.Dni);
                    command.Parameters.AddWithValue("@Matricula", agente.Matricula);
                    command.Parameters.AddWithValue("@Telefono", agente.Telefono);
                    command.Parameters.AddWithValue("@ID_Usuario", agente.ID_Usuario.HasValue ? (object)agente.ID_Usuario.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@id", agente.ID_Agente);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public AgenteSanitario? ObtenerPorId(int id)
        {
            AgenteSanitario? agente = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                const string sql = @"SELECT a.ID_Agente, a.Nombre, a.Apellido, a.Dni, a.Matricula, a.Telefono, a.ID_Usuario,
                                     u.Email, u.AvatarURL
                                     FROM AgentesSanitarios a
                                     LEFT JOIN Usuarios u ON a.ID_Usuario = u.ID_Usuario
                                     WHERE a.ID_Agente = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            agente = new AgenteSanitario
                            {
                                ID_Agente = reader.GetInt32("ID_Agente"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Dni = reader.GetString("Dni"),
                                Matricula = reader.GetString("Matricula"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? string.Empty : reader.GetString("Telefono"),
                                ID_Usuario = reader.IsDBNull(reader.GetOrdinal("ID_Usuario")) ? (int?)null : reader.GetInt32("ID_Usuario")
                            };
                            if (agente.ID_Usuario.HasValue)
                            {
                                agente.Usuario = new Usuario
                                {
                                    ID_Usuario = agente.ID_Usuario.Value,
                                    Email = reader.GetString("Email"),
                                    AvatarURL = reader.IsDBNull(reader.GetOrdinal("AvatarURL")) ? string.Empty : reader.GetString("AvatarURL")
                                };
                            }
                        }
                    }
                }
            }
            return agente;
        }

        public IList<AgenteSanitario> ObtenerTodos()
        {
            IList<AgenteSanitario> lista = new List<AgenteSanitario>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT a.ID_Agente, a.Nombre, a.Apellido, a.Dni, a.Matricula, a.Telefono, a.ID_Usuario,
                               u.Email, u.AvatarURL
                               FROM AgentesSanitarios a
                               LEFT JOIN Usuarios u ON a.ID_Usuario = u.ID_Usuario";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var agente = new AgenteSanitario
                        {
                            ID_Agente = reader.GetInt32("ID_Agente"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Dni = reader.GetString("Dni"),
                            Matricula = reader.GetString("Matricula"),
                            Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? string.Empty : reader.GetString("Telefono"),
                            ID_Usuario = reader.IsDBNull(reader.GetOrdinal("ID_Usuario")) ? (int?)null : reader.GetInt32("ID_Usuario")
                        };
                        if (agente.ID_Usuario.HasValue)
                        {
                            agente.Usuario = new Usuario
                            {
                                ID_Usuario = agente.ID_Usuario.Value,
                                Email = reader.GetString("Email"),
                                AvatarURL = reader.IsDBNull(reader.GetOrdinal("AvatarURL")) ? string.Empty : reader.GetString("AvatarURL")
                            };
                        }
                        lista.Add(agente);
                    }
                }
            }
            return lista;
        }

        public AgenteSanitario? ObtenerPorUsuarioId(int idUsuario)
        {
            AgenteSanitario? agente = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                const string sql = @"SELECT a.ID_Agente, a.Nombre, a.Apellido, a.Dni, a.Matricula, a.Telefono, a.ID_Usuario,
                                     u.Email, u.AvatarURL
                                     FROM AgentesSanitarios a
                                     JOIN Usuarios u ON a.ID_Usuario = u.ID_Usuario
                                     WHERE a.ID_Usuario = @idUsuario";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idUsuario", idUsuario);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            agente = new AgenteSanitario
                            {
                                ID_Agente = reader.GetInt32("ID_Agente"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Dni = reader.GetString("Dni"),
                                Matricula = reader.GetString("Matricula"),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? string.Empty : reader.GetString("Telefono"),
                                ID_Usuario = reader.GetInt32("ID_Usuario"),
                                Usuario = new Usuario
                                {
                                    ID_Usuario = reader.GetInt32("ID_Usuario"),
                                    Email = reader.GetString("Email"),
                                    AvatarURL = reader.IsDBNull(reader.GetOrdinal("AvatarURL")) ? string.Empty : reader.GetString("AvatarURL")
                                }
                            };
                        }
                    }
                }
            }
            return agente;
        }
    }
}