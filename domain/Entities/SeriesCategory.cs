using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class SeriesCategory
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int SeriesId { get; set; }
        //nvagiation propery
        public Series? Series { get; set; }
        public Category? Category { get; set; }

    }
}