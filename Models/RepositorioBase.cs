using Microsoft.Extensions.Configuration; 
using System;

namespace VacunacionTPFinal.Models
{
    public abstract class RepositorioBase
    {
        protected readonly IConfiguration configuration;
        protected readonly string connectionString;

        protected RepositorioBase(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration["ConnectionStrings:MySql"]!; 
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'ConnectionStrings:MySql' no está configurada.");
            }
        }
    }
}