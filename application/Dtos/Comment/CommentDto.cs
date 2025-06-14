using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace application.Dtos.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Discussion { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        // public int EpisodeId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}