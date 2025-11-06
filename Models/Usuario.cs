using System;
public class Usuario
{
    public int ID_Usuario { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; } // Se guarda el HASH, no la contraseña        

    public string Rol { get; set; }

    public string AvatarURL { get; set; }
}