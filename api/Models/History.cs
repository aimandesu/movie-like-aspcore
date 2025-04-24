using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class History
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VideoId { get; set; }
        //navigation propery
        public List<Video> Videos { get; set; } = [];
        public User? User { get; set; }
    }
}