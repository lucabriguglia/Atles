using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Atles.Core.Extensions;

/// <summary>
/// SlugExtensions
/// </summary>
public static class SlugExtensions
{
    /// <summary>
    /// Convert text to slug
    /// </summary>
    /// <param name="text"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string ToSlug(this string text, int maxLength = 50)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentNullException(nameof(text));
        }

        text = Regex.Replace(text, @"\s+", " "); // Remove multiple spaces from text

        var stringBuilder = new StringBuilder();

        foreach (var c in text.ToArray())
        {
            if (char.IsLetterOrDigit(c))
            {
                stringBuilder.Append(c);
            }
            else if (c == ' ')
            {
                stringBuilder.Append('-');
            }
        }

        var result = stringBuilder.ToString().ToLower();

        if (result.Length > maxLength)
        {
            result = result.Substring(0, maxLength);
        }

        return result;
    }
}