public class Escuela
    {
        public int ID_Escuela { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Fotos { get; set; } // URL o ruta al archivo
        public string TelefonoInstitucional { get; set; }

        // Propiedad de navegación (Muchos Alumnos)
        public List<Alumno> Alumnos { get; set; } = new List<Alumno>();
    }