public class Alumno
    {
        public int ID_Alumno { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string TelefonoTutor { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Grado { get; set; }

        // Clave foránea para la relación Muchos a 1
        public int ID_Escuela { get; set; }

        // Propiedad de navegación (Una Escuela)
        public Escuela Escuela { get; set; }

        // Propiedad de navegación (Muchos Registros)
        public List<RegistroVacunacion> Registros { get; set; } = new List<RegistroVacunacion>();
    }