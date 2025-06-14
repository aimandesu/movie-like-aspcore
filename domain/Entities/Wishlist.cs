using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int SeriesId { get; set; }
        public string UserId { get; set; } = string.Empty;
        //Navigation property
        public User? User { get; set; }
        public Series? Series { get; set; }
    }
}