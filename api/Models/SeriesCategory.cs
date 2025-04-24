using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class SeriesCategory
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int SeriesId { get; set; }
        //nvagiation propery
        required public Series Series { get; set; }
        required public Category Category { get; set; }

    }
}