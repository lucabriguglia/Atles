using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Atlas.Domain
{
    public static class SlugExtensions
    {
        public static string ToSlug(this string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
            {
                throw new ArgumentNullException(nameof(phrase));
            }

            phrase = Regex.Replace(phrase, @"\s+", " "); // Remove multiple spaces from phrase

            if (phrase.Length > 50)
            {
                phrase = phrase.Substring(0, 50);
            }

            var stringBuilder = new StringBuilder();

            foreach (var c in phrase.ToArray())
            {
                if (char.IsLetterOrDigit(c))
                {
                    stringBuilder.Append(c);
                }
                else if (c == ' ')
                {
                    stringBuilder.Append("-");
                }
            }

            return stringBuilder.ToString().ToLower();
        }
    }
}
