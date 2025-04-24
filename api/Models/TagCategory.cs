using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class TagCategory
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public int SeriesId { get; set; }
        //navigation property
        required public Series Series { get; set; }
        required public Tag Tag { get; set; }
    }
}