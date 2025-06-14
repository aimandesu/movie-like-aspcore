using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // public TagCategory? TagCategory { get; set; }
    }
}