using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
// Corregido: Apuntamos al namespace 'Models' donde ahora residen las interfaces
using VacunacionTPFinal.Models; 

namespace VacunacionTPFinal.Models
{
    public class RepositorioEscuela : RepositorioBase, IRepositorioEscuela
    {
        public RepositorioEscuela(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Escuela escuela)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Escuelas (Nombre, Direccion, Fotos, TelefonoInstitucional)
                             VALUES (@Nombre, @Direccion, @Fotos, @Telefono);
                             SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", escuela.Nombre);
                    command.Parameters.AddWithValue("@Direccion", escuela.Direccion);
                    command.Parameters.AddWithValue("@Fotos", escuela.Fotos);
                    command.Parameters.AddWithValue("@Telefono", escuela.TelefonoInstitucional);
                    connection.Open();
                    escuela.ID_Escuela = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return escuela.ID_Escuela;
        }

        public int Baja(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM Escuelas WHERE ID_Escuela = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificacion(Escuela escuela)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Escuelas 
                             SET Nombre = @Nombre, Direccion = @Direccion, Fotos = @Fotos, TelefonoInstitucional = @Telefono
                             WHERE ID_Escuela = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", escuela.Nombre);
                    command.Parameters.AddWithValue("@Direccion", escuela.Direccion);
                    command.Parameters.AddWithValue("@Fotos", escuela.Fotos);
                    command.Parameters.AddWithValue("@Telefono", escuela.TelefonoInstitucional);
                    command.Parameters.AddWithValue("@id", escuela.ID_Escuela);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public Escuela? ObtenerPorId(int id)
        {
            Escuela? escuela = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                const string sql = @"SELECT ID_Escuela, Nombre, Direccion, Fotos, TelefonoInstitucional
                                   FROM Escuelas
                                   WHERE ID_Escuela = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            escuela = new Escuela
                            {
                                ID_Escuela = reader.GetInt32("ID_Escuela"),
                                Nombre = reader.GetString("Nombre"),
                                Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? string.Empty : reader.GetString("Direccion"),
                                Fotos = reader.IsDBNull(reader.GetOrdinal("Fotos")) ? string.Empty : reader.GetString("Fotos"),
                                TelefonoInstitucional = reader.IsDBNull(reader.GetOrdinal("TelefonoInstitucional")) ? string.Empty : reader.GetString("TelefonoInstitucional")
                            };
                        }
                    }
                }
            }
            return escuela;
        }

        public IList<Escuela> ObtenerTodos()
        {
            IList<Escuela> lista = new List<Escuela>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT ID_Escuela, Nombre, Direccion, Fotos, TelefonoInstitucional
                             FROM Escuelas";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new Escuela
                        {
                            ID_Escuela = reader.GetInt32("ID_Escuela"),
                            Nombre = reader.GetString("Nombre"),
                            Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? string.Empty : reader.GetString("Direccion"),
                            Fotos = reader.IsDBNull(reader.GetOrdinal("Fotos")) ? string.Empty : reader.GetString("Fotos"),
                            TelefonoInstitucional = reader.IsDBNull(reader.GetOrdinal("TelefonoInstitucional")) ? string.Empty : reader.GetString("TelefonoInstitucional")
                        });
                    }
                }
            }
            return lista;
        }

        public IList<Escuela> BuscarPorNombre(string nombre)
        {
            IList<Escuela> lista = new List<Escuela>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT ID_Escuela, Nombre, Direccion, Fotos, TelefonoInstitucional
                             FROM Escuelas
                             WHERE Nombre LIKE @nombre";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", $"%{nombre}%");
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new Escuela
                        {
                            ID_Escuela = reader.GetInt32("ID_Escuela"),
                            Nombre = reader.GetString("Nombre"),
                            Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? string.Empty : reader.GetString("Direccion"),
                            Fotos = reader.IsDBNull(reader.GetOrdinal("Fotos")) ? string.Empty : reader.GetString("Fotos"),
                            TelefonoInstitucional = reader.IsDBNull(reader.GetOrdinal("TelefonoInstitucional")) ? string.Empty : reader.GetString("TelefonoInstitucional")
                        });
                    }
                }
            }
            return lista;
        }
    }
}