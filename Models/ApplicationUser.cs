using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;

namespace AutoOglasi.Models
{
    [CollectionName("korisnici")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public string ImeIPrezime { get; set; } = string.Empty;
    }
}