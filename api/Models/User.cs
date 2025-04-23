using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class User //import IdentityUser later
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<Wishlist> Wishlists { get; set; } = [];
        public List<Impression> Impressions { get; set; } = [];
        public List<History> Histories { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];
    }
}