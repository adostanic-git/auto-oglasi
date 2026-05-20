using AutoOglasi.Models;
using AutoOglasi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

        // GET: /Vehicle/Index — lista svih vozila
        public async Task<IActionResult> Index()
        {
            var vehicles = await _vehicleService.GetAllAsync();
            return View(vehicles);
        }

        // GET: /Vehicle/Details/id — detalji jednog vozila
        public async Task<IActionResult> Details(string id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            // ViewBag.IsOwner govori view-u da li da prikaze dugmad Izmeni/Obrisi
            ViewBag.IsOwner = vehicle.KorisnikId == GetUserId();
            return View(vehicle);
        }

        // GET: /Vehicle/Create — forma za novi oglas
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Vehicle/Create — cuvanje novog oglasa
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

        // GET: /Vehicle/Edit/id — forma za izmenu oglasa
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            // Samo vlasnik moze da menja oglas
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

        // POST: /Vehicle/Edit — cuvanje izmena oglasa
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

            // Brisanje oznacenih slika sa diska i iz liste
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

        // POST: /Vehicle/Delete — brisanje oglasa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);
            if (vehicle == null)
                return NotFound();

            if (vehicle.KorisnikId != GetUserId())
                return Forbid();

            // Brisanje svih slika sa diska pre brisanja oglasa
            foreach (var slika in vehicle.Slike)
                ObrisiSliku(slika);

            await _vehicleService.DeleteAsync(id);
            TempData["Success"] = "Oglas je obrisan.";
            return RedirectToAction(nameof(Index));
        }

        // Pomocna metoda — cuva uploadovane slike u wwwroot/uploads/
        private async Task<List<string>> SacuvajSlike(List<IFormFile> fajlovi)
        {
            var uploadDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadDir); // kreira folder ako ne postoji

            var putanje = new List<string>();
            var dozvoljeniTipovi = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };

            foreach (var fajl in fajlovi)
            {
                // Preskoci prazne fajlove i nedozvoljene tipove
                if (fajl.Length == 0 || !dozvoljeniTipovi.Contains(fajl.ContentType))
                    continue;

                // GUID kao ime fajla da ne bi doslo do kolizije
                var ekstenzija = Path.GetExtension(fajl.FileName);
                var imeFajla = $"{Guid.NewGuid()}{ekstenzija}";
                var putanjaFajla = Path.Combine(uploadDir, imeFajla);

                using var stream = new FileStream(putanjaFajla, FileMode.Create);
                await fajl.CopyToAsync(stream);

                putanje.Add($"/uploads/{imeFajla}");
            }

            return putanje;
        }

        // Pomocna metoda — brise sliku sa diska
        private void ObrisiSliku(string relativnaPutanja)
        {
            if (string.IsNullOrEmpty(relativnaPutanja))
                return;

            var putanja = Path.Combine(_env.WebRootPath, relativnaPutanja.TrimStart('/'));
            if (System.IO.File.Exists(putanja))
                System.IO.File.Delete(putanja);
        }

        // Pomocna metoda — vraca ID trenutno ulogovanog korisnika
        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }
}