using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Alumno
{
    [Key]
    public int AlumnoID { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe contener 7 u 8 dígitos numéricos.")]
    public string DNI { get; set; } = null!;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateTime FechaNacimiento { get; set; }

    [RegularExpression(@"^$|^\d{10,}$", ErrorMessage = "El teléfono debe contener al menos 10 dígitos.")]
    public string? TelefonoTutor { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una escuela obligatoriamente.")]
    public int EscuelaID { get; set; }

    [ForeignKey(nameof(EscuelaID))]
    public Escuela? Escuela { get; set; }
    public bool Borrado { get; set; }
    public DateTime? FechaBaja { get; set; }
}