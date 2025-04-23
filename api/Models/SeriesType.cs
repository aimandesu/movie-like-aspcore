using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class SeriesType
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public int TagId { get; set; }
        //Navigation property
        public List<Tag> Tags { get; set; } = [];
        public List<Category> Categories { get; set; } = [];
    }
}