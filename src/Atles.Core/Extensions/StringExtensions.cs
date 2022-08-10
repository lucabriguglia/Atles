using System.Security.Cryptography;
using System.Text;

namespace Atles.Core.Extensions;

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

    /// <summary>
    /// https://www.danesparza.net/2010/10/using-gravatar-images-with-c-asp-net/
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static string ToGravatarEmailHash(this string email)
    {
        // Create a new instance of the MD5CryptoServiceProvider object.  
        var md5Hasher = MD5.Create();

        // Convert the input string to a byte array and compute the hash.  
        var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));

        // Create a new string builder to collect the bytes  
        // and create a string.  
        StringBuilder sBuilder = new();

        // Loop through each byte of the hashed data  
        // and format each one as a hexadecimal string.  
        for (var i = 0; i < data.Length; i++)
        {
            sBuilder.Append(i.ToString("x2"));
        }

        return sBuilder.ToString();  // Return the hexadecimal string. 
    }
}