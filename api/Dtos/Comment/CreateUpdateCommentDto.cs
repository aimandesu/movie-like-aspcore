using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class CreateUpdateCommentDto
    {
        [Required]
        public string Discussion { get; set; } = string.Empty;
        [Required]
        public int EpisodeId { get; set; }
    }
}