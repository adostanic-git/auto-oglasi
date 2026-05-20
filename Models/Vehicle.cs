using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace AutoOglasi.Models
{
    public class Vehicle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Marka { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Godiste { get; set; }
        public decimal Cena { get; set; }
        public string Opis { get; set; } = string.Empty;

        // Lista putanja do slika
        public List<string> Slike { get; set; } = new();

        // ID korisnika koji je postavio oglas
        public string KorisnikId { get; set; } = string.Empty;
    }
}