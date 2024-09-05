using System.Text;

namespace TWISTServer.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a CamelCase string to it's snake_case equivalent
        /// </summary>
        /// <param name="text">The CamelCase text to translate to snake_case</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ToSnakeCase(this string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (text.Length < 2)
            {
                return text.ToLowerInvariant();
            }
            var sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(text[0]));
            for (int i = 1; i < text.Length; ++i)
            {
                char c = text[i];
                if (char.IsUpper(c))
                {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
