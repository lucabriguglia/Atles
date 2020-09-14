using System.Text;

namespace Atles.Data.Extensions
{
    public static class StringExtensions
    {
        public static string InsertSpaceBeforeUpperCase(this string str)
        {
            var sb = new StringBuilder();

            var previousChar = char.MinValue;

            foreach (var c in str)
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