using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace domain.Entities
{
    public class TagCategory
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public int SeriesId { get; set; }
        //navigation property
        public Series? Series { get; set; }
        public Tag? Tag { get; set; }
    }
}