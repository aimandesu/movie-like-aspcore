using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domain.Entities;

namespace application.Features.CommentFeature.Create
{
    public class CreateCommentResponse
    {
        public Comment? Comment { get; set; }
    }
}