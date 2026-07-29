using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

public class Usuario
{
    [Key]
    public int UsuarioID { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe contener 7 u 8 dígitos numéricos.")]
    public string DNI { get; set; } = null!;

    [Required(ErrorMessage = "La matrícula es obligatoria.")]
    public string Matricula { get; set; } = null!;
    
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico debe tener un formato válido (ejemplo@dominio.com).")]
    public string Email { get; set; } = null!;
    
    [Required]
    public string PasswordHash { get; set; } = null!;
    
    public string Rol { get; set; } = null!;
    public string? AvatarURL { get; set; }

    [RegularExpression(@"^$|^\d{10,}$", ErrorMessage = "El teléfono debe contener al menos 10 dígitos.")]
    public string? Telefono { get; set; }
    
    public DateTime? FechaBaja { get; set; }
    public bool Borrado { get; set; }

    [NotMapped]
    public IFormFile? AvatarFile { get; set; }
}