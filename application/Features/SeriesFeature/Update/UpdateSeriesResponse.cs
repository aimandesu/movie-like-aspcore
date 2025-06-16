using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using application.Dtos.Series;

namespace application.Features.SeriesFeature.Update
{
    public sealed record UpdateSeriesResponse
    {
        public SeriesDto? SeriesDto { get; set; }
    }
}