using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;

namespace application.Features.TagFeature.Create
{
    public sealed record class CreateTagResponse
    {
        public Tag? Tag { get; set; }
    }
}