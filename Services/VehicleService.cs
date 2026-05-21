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

        public async Task<List<Vehicle>> GetAllAsync() =>
            await _vehicles.Find(_ => true).ToListAsync();

        public async Task<Vehicle?> GetByIdAsync(string id) =>
            await _vehicles.Find(v => v.Id == id).FirstOrDefaultAsync();

        public async Task<List<Vehicle>> GetByUserAsync(string korisnikId) =>
            await _vehicles.Find(v => v.KorisnikId == korisnikId).ToListAsync();

        public async Task CreateAsync(Vehicle vehicle) =>
            await _vehicles.InsertOneAsync(vehicle);

        public async Task UpdateAsync(string id, Vehicle vehicle) =>
            await _vehicles.ReplaceOneAsync(v => v.Id == id, vehicle);

        public async Task DeleteAsync(string id) =>
            await _vehicles.DeleteOneAsync(v => v.Id == id);
    }
}
