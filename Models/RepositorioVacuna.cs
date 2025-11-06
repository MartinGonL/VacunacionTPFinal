using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using VacunacionTPFinal.Models;

namespace VacunacionTPFinal.Models
{
    public class RepositorioVacuna : RepositorioBase, IRepositorioVacuna
    {
        public RepositorioVacuna(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Vacuna vacuna)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Vacunas (Nombre, Lote, Descripcion)
                             VALUES (@Nombre, @Lote, @Descripcion);
                             SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", vacuna.Nombre);
                    command.Parameters.AddWithValue("@Lote", vacuna.Lote);
                    command.Parameters.AddWithValue("@Descripcion", vacuna.Descripcion);
                    connection.Open();
                    vacuna.ID_Vacuna = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
            }
            return vacuna.ID_Vacuna;
        }

        public int Baja(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM Vacunas WHERE ID_Vacuna = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Modificacion(Vacuna vacuna)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Vacunas 
                             SET Nombre = @Nombre, Lote = @Lote, Descripcion = @Descripcion
                             WHERE ID_Vacuna = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", vacuna.Nombre);
                    command.Parameters.AddWithValue("@Lote", vacuna.Lote);
                    command.Parameters.AddWithValue("@Descripcion", vacuna.Descripcion);
                    command.Parameters.AddWithValue("@id", vacuna.ID_Vacuna);
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public Vacuna? ObtenerPorId(int id)
        {
            Vacuna? vacuna = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                const string sql = @"SELECT ID_Vacuna, Nombre, Lote, Descripcion
                                   FROM Vacunas
                                   WHERE ID_Vacuna = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            vacuna = new Vacuna
                            {
                                ID_Vacuna = reader.GetInt32("ID_Vacuna"),
                                Nombre = reader.GetString("Nombre"),
                                Lote = reader.IsDBNull(reader.GetOrdinal("Lote")) ? string.Empty : reader.GetString("Lote"),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? string.Empty : reader.GetString("Descripcion")
                            };
                        }
                    }
                }
            }
            return vacuna;
        }

        public IList<Vacuna> ObtenerTodos()
        {
            IList<Vacuna> lista = new List<Vacuna>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT ID_Vacuna, Nombre, Lote, Descripcion FROM Vacunas";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new Vacuna
                        {
                            ID_Vacuna = reader.GetInt32("ID_Vacuna"),
                            Nombre = reader.GetString("Nombre"),
                            Lote = reader.IsDBNull(reader.GetOrdinal("Lote")) ? string.Empty : reader.GetString("Lote"),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? string.Empty : reader.GetString("Descripcion")
                        });
                    }
                }
            }
            return lista;
        }

        public IList<Vacuna> BuscarPorNombre(string nombre)
        {
            IList<Vacuna> lista = new List<Vacuna>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT ID_Vacuna, Nombre, Lote, Descripcion
                             FROM Vacunas
                             WHERE Nombre LIKE @nombre";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", $"%{nombre}%");
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new Vacuna
                        {
                            ID_Vacuna = reader.GetInt32("ID_Vacuna"),
                            Nombre = reader.GetString("Nombre"),
                            Lote = reader.IsDBNull(reader.GetOrdinal("Lote")) ? string.Empty : reader.GetString("Lote"),
                            Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? string.Empty : reader.GetString("Descripcion")
                        });
                    }
                }
            }
            return lista;
        }
    }
}