using System.ComponentModel.DataAnnotations;

namespace AutoOglasi.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Unesite ispravan email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;
    }
}