namespace Atles.Client.Extensions
{
    public static class StringExtensions
    {
        public static string ToSafeLengthText(this string text, int maxLength)
        {
            return text.Length > maxLength ? $"{text.Substring(0, maxLength)}..." : text;
        }
    }
}