using AutoOglasi.Models;
using AutoOglasi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AutoOglasi.Controllers
{
    [Authorize]
    public class VehicleController : Controller
    {
        private readonly VehicleService _vehicleService;
        private readonly IWebHostEnvironment _env;

        public VehicleController(VehicleService vehicleService, IWebHostEnvironment env)
        {
            _vehicleService = vehicleService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _vehicleService.GetAllAsync();
            return View(vehicles);
        }

        public async Task<IActionResult> Details(string id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            ViewBag.IsOwner = vehicle.KorisnikId == GetUserId();
            return View(vehicle);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var slike = new List<string>();

            if (vm.Slike != null && vm.Slike.Count > 0)
            {
                slike = await SacuvajSlike(vm.Slike);
            }

            var vehicle = new Vehicle
            {
                Marka = vm.Marka,
                Model = vm.Model,
                Godiste = vm.Godiste,
                Cena = vm.Cena,
                Opis = vm.Opis,
                Slike = slike,
                KorisnikId = GetUserId()
            };

            await _vehicleService.CreateAsync(vehicle);
            TempData["Success"] = "Oglas je uspešno postavljen.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            if (vehicle.KorisnikId != GetUserId())
                return Forbid();

            var model = new VehicleEditViewModel
            {
                Id = vehicle.Id!,
                Cena = vehicle.Cena,
                Opis = vehicle.Opis,
                PostojeceSlike = vehicle.Slike
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VehicleEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var vehicle = await _vehicleService.GetByIdAsync(model.Id);
            if (vehicle == null)
                return NotFound();

            if (vehicle.KorisnikId != GetUserId())
                return Forbid();

            // Uklanjanje označenih slika
            if (model.SlikeZaBrisanje != null && model.SlikeZaBrisanje.Count > 0)
            {
                foreach (var slika in model.SlikeZaBrisanje)
                {
                    ObrisiSliku(slika);
                    vehicle.Slike.Remove(slika);
                }
            }

            // Dodavanje novih slika
            if (model.NoveSlike != null && model.NoveSlike.Count > 0)
            {
                var noveSacuvane = await SacuvajSlike(model.NoveSlike);
                vehicle.Slike.AddRange(noveSacuvane);
            }

            vehicle.Cena = model.Cena;
            vehicle.Opis = model.Opis;

            await _vehicleService.UpdateAsync(vehicle.Id!, vehicle);
            TempData["Success"] = "Oglas je uspešno izmenjen.";
            return RedirectToAction(nameof(Details), new { id = vehicle.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            if (vehicle.KorisnikId != GetUserId())
                return Forbid();

            foreach (var slika in vehicle.Slike)
                ObrisiSliku(slika);

            await _vehicleService.DeleteAsync(id);
            TempData["Success"] = "Oglas je obrisan.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<string>> SacuvajSlike(List<IFormFile> fajlovi)
        {
            var uploadDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadDir);

            var putanje = new List<string>();
            var dozvoljeniTipovi = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };

            foreach (var fajl in fajlovi)
            {
                if (fajl.Length == 0 || !dozvoljeniTipovi.Contains(fajl.ContentType))
                    continue;

                var ekstenzija = Path.GetExtension(fajl.FileName);
                var imeFajla = $"{Guid.NewGuid()}{ekstenzija}";
                var putanjaFajla = Path.Combine(uploadDir, imeFajla);

                using var stream = new FileStream(putanjaFajla, FileMode.Create);
                await fajl.CopyToAsync(stream);

                putanje.Add($"/uploads/{imeFajla}");
            }

            return putanje;
        }

        private void ObrisiSliku(string relativnaPutanja)
        {
            if (string.IsNullOrEmpty(relativnaPutanja))
                return;

            var putanja = Path.Combine(_env.WebRootPath, relativnaPutanja.TrimStart('/'));
            if (System.IO.File.Exists(putanja))
                System.IO.File.Delete(putanja);
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}
