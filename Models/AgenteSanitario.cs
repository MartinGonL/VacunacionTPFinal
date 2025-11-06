public class AgenteSanitario
    {
        public int ID_Agente { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Matricula { get; set; }
        public string Telefono { get; set; }
        
        // Clave foránea para la relación 1 a 1
        public int? ID_Usuario { get; set; }

        // Propiedad de navegación (Un Usuario)
        public Usuario Usuario { get; set; }

        // Propiedad de navegación (Muchos Registros)
        public List<RegistroVacunacion> Registros { get; set; } = new List<RegistroVacunacion>();
    }