using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class SeriesQueryObject
    {
        public string? Category { get; set; }
        public int PageNumber {get; set;} = 1;
        
        private const int MaxPageSize = 50;
        private int _pageSize = 15;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

    }
}