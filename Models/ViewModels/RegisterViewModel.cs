using System.ComponentModel.DataAnnotations;

namespace AutoOglasi.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ime i prezime je obavezno")]
        public string ImeIPrezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Unesite ispravan email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [MinLength(6, ErrorMessage = "Lozinka mora imati minimum 6 karaktera")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrdite lozinku")]
        [DataType(DataType.Password)]
        [Compare("Lozinka", ErrorMessage = "Lozinke se ne poklapaju")]
        public string PotvrdaLozinke { get; set; } = string.Empty;
    }
}