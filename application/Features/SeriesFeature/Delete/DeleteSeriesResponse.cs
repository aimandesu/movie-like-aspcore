using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;

namespace application.Features.SeriesFeature.Delete
{
    public sealed record DeleteSeriesResponse
    {
        public Series? Series { get; set; }
    }
}