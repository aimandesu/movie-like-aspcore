using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Discussion { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User? User { get; set; }
        public Series? Series { get; set; }
    }
}