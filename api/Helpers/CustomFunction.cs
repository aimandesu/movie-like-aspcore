using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace api.Helpers
{
    public static class CustomFunction
    {
        public static string SanitizeFolderName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name;
        }

        public static string GenerateSlug(string title)
        {

            // Convert to lowercase
            string slug = title.ToLowerInvariant();

            // Replace spaces with hyphens
            slug = slug.Replace(" ", "-");

            // Remove invalid characters
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Remove multiple hyphens
            slug = Regex.Replace(slug, @"-{2,}", "-");

            // Trim hyphens from beginning and end
            slug = slug.Trim('-');

            // Append the ID to ensure uniqueness
            slug = $"{slug}";

            return slug;
        }

    }
}