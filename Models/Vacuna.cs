using System;
using System.ComponentModel.DataAnnotations;

public class Vacuna
{
    [Key]
    public int VacunaID { get; set; }
    
    [Required(ErrorMessage = "El nombre de la vacuna es obligatorio.")]
    public string NombreVacuna { get; set; } = null!;
    
    public string? Descripcion { get; set; }
    
    public DateTime? FechaBaja { get; set; }
    public bool Borrado { get; set; }
}