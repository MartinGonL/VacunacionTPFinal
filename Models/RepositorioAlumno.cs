using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
// Nota: La interfaz IRepositorioAlumno ahora también está en el namespace Models
using VacunacionTPFinal.Models;

namespace VacunacionTPFinal.Models
{
    public class RepositorioAlumno : RepositorioBase, IRepositorioAlumno
    {
        public RepositorioAlumno(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Alumno alumno)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Alumnos (Nombre, Apellido, Dni, TelefonoTutor, FechaNacimiento, Grado, ID_Escuela)
                             VALUES (@Nombre, @Apellido, @Dni, @TelefonoTutor, @FechaNacimiento, @Grado, @ID_Escuela);
                             SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", alumno.Nombre);
                    command.Parameters.AddWithValue("@Apellido", alumno.Apellido);
                    command.Parameters.AddWithValue("@Dni", alumno.Dni);
                    command.Parameters.AddWithValue("@TelefonoTutor", alumno.TelefonoTutor);
                    command.Parameters.AddWithValue("@FechaNacimiento", alumno.FechaNacimiento);
                    command.Parameters.AddWithValue("@Grado", alumno.Grado);
                    command.Parameters.AddWithValue("@ID_Escuela", alumno.ID_Escuela);
                    connection.Open();
                    alumno.ID_Alumno = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return alumno.ID_Alumno;
        }

        public int Baja(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM Alumnos WHERE ID_Alumno = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificacion(Alumno alumno)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Alumnos 
                             SET Nombre = @Nombre, Apellido = @Apellido, Dni = @Dni, 
                                 TelefonoTutor = @TelefonoTutor, FechaNacimiento = @FechaNacimiento, 
                                 Grado = @Grado, ID_Escuela = @ID_Escuela
                             WHERE ID_Alumno = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", alumno.Nombre);
                    command.Parameters.AddWithValue("@Apellido", alumno.Apellido);
                    command.Parameters.AddWithValue("@Dni", alumno.Dni);
                    command.Parameters.AddWithValue("@TelefonoTutor", alumno.TelefonoTutor);
                    command.Parameters.AddWithValue("@FechaNacimiento", alumno.FechaNacimiento);
                    command.Parameters.AddWithValue("@Grado", alumno.Grado);
                    command.Parameters.AddWithValue("@ID_Escuela", alumno.ID_Escuela);
                    command.Parameters.AddWithValue("@id", alumno.ID_Alumno);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public Alumno? ObtenerPorId(int id)
        {
            Alumno? alumno = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                const string sql = @"SELECT a.ID_Alumno, a.Nombre, a.Apellido, a.Dni, a.TelefonoTutor, a.FechaNacimiento, a.Grado, a.ID_Escuela,
                                     e.Nombre AS NombreEscuela
                                     FROM Alumnos a
                                     JOIN Escuelas e ON a.ID_Escuela = e.ID_Escuela
                                     WHERE a.ID_Alumno = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            alumno = new Alumno
                            {
                                ID_Alumno = reader.GetInt32("ID_Alumno"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Dni = reader.GetString("Dni"),
                                TelefonoTutor = reader.IsDBNull(reader.GetOrdinal("TelefonoTutor")) ? string.Empty : reader.GetString("TelefonoTutor"),
                                FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                                Grado = reader.IsDBNull(reader.GetOrdinal("Grado")) ? string.Empty : reader.GetString("Grado"),
                                ID_Escuela = reader.GetInt32("ID_Escuela"),
                                Escuela = new Escuela // Propiedad de navegación
                                {
                                    ID_Escuela = reader.GetInt32("ID_Escuela"),
                                    Nombre = reader.GetString("NombreEscuela")
                                }
                            };
                        }
                    }
                }
            }
            return alumno;
        }

        public IList<Alumno> ObtenerTodos()
        {
            IList<Alumno> lista = new List<Alumno>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT a.ID_Alumno, a.Nombre, a.Apellido, a.Dni, a.TelefonoTutor, a.FechaNacimiento, a.Grado, a.ID_Escuela,
                               e.Nombre AS NombreEscuela
                               FROM Alumnos a
                               JOIN Escuelas e ON a.ID_Escuela = e.ID_Escuela";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new Alumno
                        {
                            ID_Alumno = reader.GetInt32("ID_Alumno"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Dni = reader.GetString("Dni"),
                            TelefonoTutor = reader.IsDBNull(reader.GetOrdinal("TelefonoTutor")) ? string.Empty : reader.GetString("TelefonoTutor"),
                            FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                            Grado = reader.IsDBNull(reader.GetOrdinal("Grado")) ? string.Empty : reader.GetString("Grado"),
                            ID_Escuela = reader.GetInt32("ID_Escuela"),
                            Escuela = new Escuela
                            {
                                ID_Escuela = reader.GetInt32("ID_Escuela"),
                                Nombre = reader.GetString("NombreEscuela")
                            }
                        });
                    }
                }
            }
            return lista;
        }

        public IList<Alumno> ObtenerPorEscuela(int idEscuela)
        {
            IList<Alumno> lista = new List<Alumno>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT a.ID_Alumno, a.Nombre, a.Apellido, a.Dni, a.TelefonoTutor, a.FechaNacimiento, a.Grado, a.ID_Escuela,
                               e.Nombre AS NombreEscuela
                               FROM Alumnos a
                               JOIN Escuelas e ON a.ID_Escuela = e.ID_Escuela
                               WHERE a.ID_Escuela = @idEscuela";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idEscuela", idEscuela);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new Alumno
                        {
                            ID_Alumno = reader.GetInt32("ID_Alumno"),
                            Nombre = reader.GetString("Nombre"),
                            Apellido = reader.GetString("Apellido"),
                            Dni = reader.GetString("Dni"),
                            TelefonoTutor = reader.IsDBNull(reader.GetOrdinal("TelefonoTutor")) ? string.Empty : reader.GetString("TelefonoTutor"),
                            FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                            Grado = reader.IsDBNull(reader.GetOrdinal("Grado")) ? string.Empty : reader.GetString("Grado"),
                            ID_Escuela = reader.GetInt32("ID_Escuela"),
                            Escuela = new Escuela
                            {
                                ID_Escuela = reader.GetInt32("ID_Escuela"),
                                Nombre = reader.GetString("NombreEscuela")
                            }
                        });
                    }
                }
            }
            return lista;
        }

        public IList<Alumno> ObtenerPaginado(int pagina, int cantidad)
        {
            IList<Alumno> lista = new List<Alumno>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                int offset = (pagina - 1) * cantidad;
                string sql = @"SELECT a.ID_Alumno, a.Nombre, a.Apellido, a.Dni, a.TelefonoTutor, a.FechaNacimiento, a.Grado, a.ID_Escuela,
                               e.Nombre AS NombreEscuela
                               FROM Alumnos a
                               JOIN Escuelas e ON a.ID_Escuela = e.ID_Escuela
                               ORDER BY a.ID_Alumno
                               LIMIT @cantidad OFFSET @offset";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@cantidad", cantidad);
                    command.Parameters.AddWithValue("@offset", offset);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Alumno
                            {
                                ID_Alumno = reader.GetInt32("ID_Alumno"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Dni = reader.GetString("Dni"),
                                TelefonoTutor = reader.IsDBNull(reader.GetOrdinal("TelefonoTutor")) ? string.Empty : reader.GetString("TelefonoTutor"),
                                FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                                Grado = reader.IsDBNull(reader.GetOrdinal("Grado")) ? string.Empty : reader.GetString("Grado"),
                                ID_Escuela = reader.GetInt32("ID_Escuela"),
                                Escuela = new Escuela
                                {
                                    ID_Escuela = reader.GetInt32("ID_Escuela"),
                                    Nombre = reader.GetString("NombreEscuela")
                                }
                            });
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
                const string sql = "SELECT COUNT(*) FROM Alumnos";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }
    }
}