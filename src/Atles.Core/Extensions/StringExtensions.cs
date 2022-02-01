using System.Text;

namespace Atles.Core.Extensions
{
    /// <summary>
    /// String Extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Insert Space Before Upper Case
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string InsertSpaceBeforeUpperCase(this string text)
        {
            var sb = new StringBuilder();

            var previousChar = char.MinValue;

            foreach (var c in text)
            {
                if (char.IsUpper(c))
                {
                    if (sb.Length != 0 && previousChar != ' ')
                    {
                        sb.Append(' ');
                    }
                }

                sb.Append(c);

                previousChar = c;
            }

            return sb.ToString();
        }
    }
}
