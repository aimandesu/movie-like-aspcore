using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Series;

namespace application.Features.SeriesFeature.Create
{
    public sealed record CreateSeriesResponse
    {
        public SeriesDto? SeriesDto { get; set; }
    }
}