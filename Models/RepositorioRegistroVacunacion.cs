using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using VacunacionTPFinal.Models;

namespace VacunacionTPFinal.Models
{
    public class RepositorioRegistroVacunacion : RepositorioBase, IRepositorioRegistroVacunacion
    {
        public RepositorioRegistroVacunacion(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(RegistroVacunacion registro)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO RegistrosVacunacion (ID_Alumno, ID_Vacuna, ID_Agente, FechaAplicacion, Dosis, Observaciones)
                             VALUES (@ID_Alumno, @ID_Vacuna, @ID_Agente, @FechaAplicacion, @Dosis, @Observaciones);
                             SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ID_Alumno", registro.ID_Alumno);
                    command.Parameters.AddWithValue("@ID_Vacuna", registro.ID_Vacuna);
                    command.Parameters.AddWithValue("@ID_Agente", registro.ID_Agente);
                    command.Parameters.AddWithValue("@FechaAplicacion", registro.FechaAplicacion);
                    command.Parameters.AddWithValue("@Dosis", registro.Dosis);
                    command.Parameters.AddWithValue("@Observaciones", registro.Observaciones);
                    connection.Open();
                    registro.ID_Registro = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return registro.ID_Registro;
        }
        [Authorize(Roles = "Administrador")]
        public int Baja(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM RegistrosVacunacion WHERE ID_Registro = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }
        [Authorize(Roles = "Administrador")]
        public int Modificacion(RegistroVacunacion registro)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE RegistrosVacunacion 
                             SET ID_Alumno = @ID_Alumno, ID_Vacuna = @ID_Vacuna, ID_Agente = @ID_Agente, 
                                 FechaAplicacion = @FechaAplicacion, Dosis = @Dosis, Observaciones = @Observaciones
                             WHERE ID_Registro = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ID_Alumno", registro.ID_Alumno);
                    command.Parameters.AddWithValue("@ID_Vacuna", registro.ID_Vacuna);
                    command.Parameters.AddWithValue("@ID_Agente", registro.ID_Agente);
                    command.Parameters.AddWithValue("@FechaAplicacion", registro.FechaAplicacion);
                    command.Parameters.AddWithValue("@Dosis", registro.Dosis);
                    command.Parameters.AddWithValue("@Observaciones", registro.Observaciones);
                    command.Parameters.AddWithValue("@id", registro.ID_Registro);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        // Método privado para construir el objeto RegistroVacunacion desde un reader
        // Esto evita repetir código en todos los métodos de consulta
        private RegistroVacunacion ConstruirRegistro(MySqlDataReader reader)
        {
            return new RegistroVacunacion
            {
                ID_Registro = reader.GetInt32("ID_Registro"),
                ID_Alumno = reader.GetInt32("ID_Alumno"),
                ID_Vacuna = reader.GetInt32("ID_Vacuna"),
                ID_Agente = reader.GetInt32("ID_Agente"),
                FechaAplicacion = reader.GetDateTime("FechaAplicacion"),
                Dosis = reader.IsDBNull(reader.GetOrdinal("Dosis")) ? string.Empty : reader.GetString("Dosis"),
                Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? string.Empty : reader.GetString("Observaciones"),
                Alumno = new Alumno
                {
                    ID_Alumno = reader.GetInt32("ID_Alumno"),
                    Nombre = reader.GetString("NombreAlumno"),
                    Apellido = reader.GetString("ApellidoAlumno")
                },
                Vacuna = new Vacuna
                {
                    ID_Vacuna = reader.GetInt32("ID_Vacuna"),
                    Nombre = reader.GetString("NombreVacuna")
                },
                Agente = new AgenteSanitario
                {
                    ID_Agente = reader.GetInt32("ID_Agente"),
                    Nombre = reader.GetString("NombreAgente"),
                    Apellido = reader.GetString("ApellidoAgente")
                }
            };
        }

        // SQL base para los JOINS
        private const string SQL_SELECT_BASE = @"
            SELECT 
                r.ID_Registro, r.FechaAplicacion, r.Dosis, r.Observaciones,
                r.ID_Alumno, a.Nombre AS NombreAlumno, a.Apellido AS ApellidoAlumno,
                r.ID_Vacuna, v.Nombre AS NombreVacuna,
                r.ID_Agente, ag.Nombre AS NombreAgente, ag.Apellido AS ApellidoAgente
            FROM RegistrosVacunacion r
            JOIN Alumnos a ON r.ID_Alumno = a.ID_Alumno
            JOIN Vacunas v ON r.ID_Vacuna = v.ID_Vacuna
            JOIN AgentesSanitarios ag ON r.ID_Agente = ag.ID_Agente
        ";

        public RegistroVacunacion? ObtenerPorId(int id)
        {
            RegistroVacunacion? registro = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"{SQL_SELECT_BASE} WHERE r.ID_Registro = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            registro = ConstruirRegistro(reader);
                        }
                    }
                }
            }
            return registro;
        }

        public IList<RegistroVacunacion> ObtenerPorAlumno(int idAlumno)
        {
            IList<RegistroVacunacion> lista = new List<RegistroVacunacion>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"{SQL_SELECT_BASE} WHERE r.ID_Alumno = @idAlumno";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idAlumno", idAlumno);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(ConstruirRegistro(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public IList<RegistroVacunacion> ObtenerPorAgente(int idAgente)
        {
            IList<RegistroVacunacion> lista = new List<RegistroVacunacion>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"{SQL_SELECT_BASE} WHERE r.ID_Agente = @idAgente";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idAgente", idAgente);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(ConstruirRegistro(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public IList<RegistroVacunacion> ObtenerPorEscuela(int idEscuela)
        {
            IList<RegistroVacunacion> lista = new List<RegistroVacunacion>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = $"{SQL_SELECT_BASE} WHERE a.ID_Escuela = @idEscuela";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idEscuela", idEscuela);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(ConstruirRegistro(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public IList<RegistroVacunacion> ObtenerPaginado(int pagina, int cantidad)
        {
            IList<RegistroVacunacion> lista = new List<RegistroVacunacion>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                int offset = (pagina - 1) * cantidad;
                string sql = $"{SQL_SELECT_BASE} ORDER BY r.FechaAplicacion DESC LIMIT @cantidad OFFSET @offset";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@cantidad", cantidad);
                    command.Parameters.AddWithValue("@offset", offset);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(ConstruirRegistro(reader));
                        }
                    }
                }
            }
            return lista;
        }

        public int ContarTotal()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                const string sql = "SELECT COUNT(*) FROM RegistrosVacunacion";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
    }
}