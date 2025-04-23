using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int SeriesId { get; set; }
        public int UserId { get; set; }
        //Navigation property
        public User? User { get; set; }
        public Series? Series { get; set; }
    }
}