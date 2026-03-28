using System.ComponentModel.DataAnnotations;

namespace Abstracciones.Modelos.Seguridad
{
    public class LoginBase
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string NombreUsuario { get; set; } = string.Empty;

        public string? PasswordHash { get; set; }
    }

    public class Login : LoginBase
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class LoginRequest : LoginBase
    {
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;
    }
}