using System.ComponentModel.DataAnnotations;

namespace AutoOglasi.Models
{
    public class VehicleCreateViewModel
    {
        [Required(ErrorMessage = "Marka je obavezna.")]
        [Display(Name = "Marka")]
        public string Marka { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model je obavezan.")]
        [Display(Name = "Model")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Godište je obavezno.")]
        [Range(1900, 2026, ErrorMessage = "Unesite ispravno godište (1900–2026).")]
        [Display(Name = "Godište")]
        public int Godiste { get; set; }

        [Required(ErrorMessage = "Cena je obavezna.")]
        [Range(0, double.MaxValue, ErrorMessage = "Cena mora biti pozitivna.")]
        [Display(Name = "Cena (€)")]
        public decimal Cena { get; set; }

        [Required(ErrorMessage = "Opis je obavezan.")]
        [Display(Name = "Opis")]
        public string Opis { get; set; } = string.Empty;

        [Display(Name = "Slike")]
        public List<IFormFile>? Slike { get; set; }
    }
}
