public class Vacuna
    {
        public int ID_Vacuna { get; set; }
        public string Nombre { get; set; }
        public string Lote { get; set; }
        public string Descripcion { get; set; }

        // Propiedad de navegación (Muchos Registros)
        public List<RegistroVacunacion> Registros { get; set; } = new List<RegistroVacunacion>();
    }