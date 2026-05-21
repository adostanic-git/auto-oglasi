using System.ComponentModel.DataAnnotations;

namespace AutoOglasi.Models
{
    public class VehicleEditViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cena je obavezna.")]
        [Range(0, double.MaxValue, ErrorMessage = "Cena mora biti pozitivna.")]
        [Display(Name = "Cena (€)")]
        public decimal Cena { get; set; }

        [Required(ErrorMessage = "Opis je obavezan.")]
        [Display(Name = "Opis")]
        public string Opis { get; set; } = string.Empty;

        public List<string> PostojeceSlike { get; set; } = new();

        [Display(Name = "Ukloni slike")]
        public List<string>? SlikeZaBrisanje { get; set; }

        [Display(Name = "Dodaj slike")]
        public List<IFormFile>? NoveSlike { get; set; }
    }
}
