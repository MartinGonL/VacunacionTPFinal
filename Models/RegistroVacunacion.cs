public class RegistroVacunacion
    {
        public int ID_Registro { get; set; }
        public DateTime FechaAplicacion { get; set; }
        public string Dosis { get; set; }
        public string Observaciones { get; set; }

        // Claves foráneas
        public int ID_Alumno { get; set; }
        public int ID_Vacuna { get; set; }
        public int ID_Agente { get; set; }

        // Propiedades de navegación (El "lado 1" de la relación)
        public Alumno Alumno { get; set; }
        public Vacuna Vacuna { get; set; }
        public AgenteSanitario Agente { get; set; }
    }