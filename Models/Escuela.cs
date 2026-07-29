using System.ComponentModel.DataAnnotations;

public class Escuela
{
    [Key]
    public int EscuelaID { get; set; }

    [Required(ErrorMessage = "El nombre de la escuela es obligatorio.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El número de la escuela es obligatorio.")]
    [Range(1, 9999, ErrorMessage = "El número de la escuela debe ser un valor numérico válido.")]
    public int? Numero { get; set; }

    [Required(ErrorMessage = "La dirección de la escuela es obligatoria.")]
    public string Direccion { get; set; } = null!;

    // Propiedades de navegación
    public ICollection<FotoEscuela> Fotos { get; set; } = new List<FotoEscuela>();
    public ICollection<Alumno> Alumnos { get; set; } = new List<Alumno>();
}