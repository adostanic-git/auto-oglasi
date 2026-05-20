using Microsoft.Extensions.Options;
using AutoOglasi.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutoOglasi.Services
{
    public class VehicleService
    {
        private readonly IMongoCollection<Vehicle> _vehicles;

        public VehicleService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _vehicles = database.GetCollection<Vehicle>(settings.Value.VehiclesCollection);
        }

        // Dohvati sva vozila
        public async Task<List<Vehicle>> GetAllAsync() =>
            await _vehicles.Find(_ => true).ToListAsync();

        // Dohvati jedno vozilo po ID-u
        public async Task<Vehicle?> GetByIdAsync(string id) =>
            await _vehicles.Find(v => v.Id == id).FirstOrDefaultAsync();

        // Dohvati sva vozila jednog korisnika
        public async Task<List<Vehicle>> GetByUserAsync(string korisnikId) =>
            await _vehicles.Find(v => v.KorisnikId == korisnikId).ToListAsync();

        // Dodaj novo vozilo
        public async Task CreateAsync(Vehicle vehicle) =>
            await _vehicles.InsertOneAsync(vehicle);

        // Izmeni vozilo
        public async Task UpdateAsync(string id, Vehicle vehicle) =>
            await _vehicles.ReplaceOneAsync(v => v.Id == id, vehicle);

        // Obrisi vozilo
        public async Task DeleteAsync(string id) =>
            await _vehicles.DeleteOneAsync(v => v.Id == id);
    }
}