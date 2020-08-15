using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Atlas.Domain
{
    /// <summary>
    /// Reference at https://stackoverflow.com/questions/2920744/url-slugify-algorithm-in-c
    /// </summary>
    public static class SlugExtensions
    {
        public static string GenerateSlug(this string phrase)
        {
            var str = phrase.RemoveAccents().ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        private static string RemoveAccents(this string text)
        {
            var sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (var letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                {
                    sbReturn.Append(letter);
                }
            }
            return sbReturn.ToString();
        }
    }
}
