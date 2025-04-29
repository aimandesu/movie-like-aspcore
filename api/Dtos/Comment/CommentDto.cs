using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Discussion { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int SeriesId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}