// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text.RegularExpressions;
// using System.Threading.Tasks;

// namespace application.Common
// {
//     public static class RegexSanitizer
//     {
//         private static readonly Regex InvalidChars = new("[^a-z0-9_]", RegexOptions.IgnoreCase);

//         public static string Sanitize(string input)
//         {
//             return InvalidChars.Replace(
//                 input.Trim().ToLower().Replace(" ", "_"), "");
//         }
//     }

// }